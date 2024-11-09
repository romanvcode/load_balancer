using LoadBalancer.WebAPI.Data;
using LoadBalancer.WebAPI.Helpers;
using LoadBalancer.WebAPI.Models;
using LoadBalancer.WebAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LoadBalancer.WebAPI.Controllers
{
    using System.Threading;

    [ApiController]
    [Authorize]
    [Route("api/tasks")]
    public class TasksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly EquationsService _equationsService;
        private readonly IServiceProvider _serviceProvider;
        private static readonly Dictionary<int, CancellationTokenSource> _cancellationTokens = new Dictionary<int, CancellationTokenSource>();

        public TasksController(ApplicationDbContext context, EquationsService equationsService, IServiceProvider serviceProvider)
        {
            _context = context;
            _equationsService = equationsService;
            _serviceProvider = serviceProvider;

            _equationsService.OnProgressUpdate += (progress) => SendProgressUpdate(progress);
        }

        [HttpPost("start")]
        public async Task<ActionResult<TaskState>> StartTask([FromBody] int size)
        {
            using var scope = _serviceProvider.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var taskStatus = new TaskState { State = "In Progress", Progress = 0 };
            db.TaskStates.Add(taskStatus);
            await db.SaveChangesAsync();

            var cancellationTokenSource = new CancellationTokenSource();
            _cancellationTokens[taskStatus.Id] = cancellationTokenSource;

            double[] result;
            try
            {
                result = _equationsService.Solve(size, cancellationTokenSource.Token);
                taskStatus.State = "Completed";
                taskStatus.Result = string.Join(", ", result);
            }
            catch (OperationCanceledException)
            {
                taskStatus.State = "Canceled";
            }
            catch (Exception ex)
            {
                taskStatus.State = "Error: " + ex.Message;
                return StatusCode(500, taskStatus);
            }

            await db.SaveChangesAsync();
            return Ok(taskStatus);
        }

        private async Task SendProgressUpdate(int progress)
        {
            using var scope = _serviceProvider.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var task = db.TaskStates.OrderByDescending(ts => ts.Id).FirstOrDefault();
            if (task != null)
            {
                task.Progress = progress;
                await db.SaveChangesAsync();
            }
        }

        [HttpPost("cancel/{id}")]
        public async Task<IActionResult> CancelTask(int id)
        {
            var task = await _context.TaskStates.FindAsync(id);
            if (task == null)
                return NotFound();

            if (_cancellationTokens.ContainsKey(id))
            {
                var cancellationTokenSource = _cancellationTokens[id];
                cancellationTokenSource.Cancel();
                task.State = "Canceled";
                await _context.SaveChangesAsync();
                return Ok("Task canceled.");
            }

            return NotFound("Cancellation token not found.");
        }

        [HttpGet("history")]
        public async Task<ActionResult<List<TaskState>>> GetTaskHistory()
        {
            var history = await _context.TaskStates.ToListAsync();
            return Ok(history);
        }
    }
}
