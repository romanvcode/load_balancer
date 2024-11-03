using LoadBalancer.WebAPI.Data;
using LoadBalancer.WebAPI.Helpers;
using LoadBalancer.WebAPI.Hubs;
using LoadBalancer.WebAPI.Models;
using LoadBalancer.WebAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace LoadBalancer.WebAPI.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/tasks")]
    public class TasksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly EquationsService _equationsService;
        private readonly IHubContext<TaskHub> _hubContext;
        private const int MAX_UNKNOWN_LIMIT = 10;

        public TasksController(ApplicationDbContext context, EquationsService equationsService, IHubContext<TaskHub> hubContext)
        {
            _context = context;
            _equationsService = equationsService;
            _hubContext = hubContext;

            _equationsService.OnProgressUpdate += (progress) => SendProgressUpdate(progress);
        }

        [HttpPost("start")]
        public async Task<ActionResult<TaskState>> StartTask([FromBody] TaskRequest request)
        {
            if (request.A.Length != request.b.Length)
            {
                return BadRequest("Matrix A and vector b dimensions do not match.");
            }

            if (request.A.GetLength(0) > MAX_UNKNOWN_LIMIT)
            {
                return BadRequest("Exceeded maximum number of unknowns.");
            }

            var taskStatus = new TaskState { State = "In Progress", Progress = 0 };
            _context.TaskStates.Add(taskStatus);
            await _context.SaveChangesAsync();

            double[] result;
            try
            {
                result = _equationsService.Solve(request.A, request.b);
                taskStatus.State = "Completed";
                taskStatus.Result = string.Join(", ", result);
            }
            catch (Exception ex)
            {
                taskStatus.State = "Error: " + ex.Message;
                return StatusCode(500, taskStatus);
            }

            taskStatus.Progress = 100;
            await _context.SaveChangesAsync();

            return Ok(taskStatus);
        }

        private async void SendProgressUpdate(int progress)
        {
            var taskId = _context.TaskStates.OrderByDescending(ts => ts.Id).FirstOrDefault()?.Id;
            if (taskId.HasValue)
            {
                await _hubContext.Clients.All.SendAsync("ReceiveTaskProgress", taskId.Value, progress);
            }
        }

        [HttpPost("cancel/{id}")]
        public async Task<IActionResult> CancelTask(int id)
        {
            var task = await _context.TaskStates.FindAsync(id);
            if (task == null)
                return NotFound();

            task.State = "Canceled";
            await _context.SaveChangesAsync();

            return Ok("Task canceled.");
        }

        [HttpGet("history")]
        public async Task<ActionResult<List<TaskState>>> GetTaskHistory()
        {
            var history = await _context.TaskStates.ToListAsync();
            return Ok(history);
        }
    }
}
