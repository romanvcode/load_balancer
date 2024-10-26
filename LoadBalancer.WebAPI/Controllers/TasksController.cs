using LoadBalancer.WebAPI.Data;
using LoadBalancer.WebAPI.Models;
using LoadBalancer.WebAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace LoadBalancer.WebAPI.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    public class TasksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly EquationsService _equationsService;

        public TasksController(ApplicationDbContext context, EquationsService equationsService)
        {
            _context = context;
            _equationsService = equationsService;
        }

        [HttpPost]
        public ActionResult<TaskState> StartTask([FromBody] TaskRequest request)
        {
            if (request.A.Length != request.b.Length)
            {
                return BadRequest("Matrix A and vector b dimensions do not match.");
            }

            var taskStatus = new TaskState { State = "In Progress", Progress = 0 };
            _context.TaskStates.Add(taskStatus);
            _context.SaveChanges();

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
            _context.SaveChanges();

            return Ok(taskStatus);
        }

        [HttpGet("history")]
        public ActionResult<List<TaskState>> GetTaskHistory()
        {
            var history = _context.TaskStates.ToList();
            return Ok(history);
        }
    }
}
