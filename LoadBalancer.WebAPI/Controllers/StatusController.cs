using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LoadBalancer.WebAPI.Controllers;
[Route("api/status")]
[ApiController]
public class StatusController : ControllerBase
{
    [HttpGet]
    public IActionResult GetStatus()
    {
        var cpuUsage = GetCpuUsage(); 
        var activeRequests = HttpContext.Items["ActiveRequests"] ?? 0;

        return Ok(new { CpuUsage = cpuUsage, ActiveRequests = activeRequests });
    }

    private double GetCpuUsage()
    {
        var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        return cpuCounter.NextValue();
    }
}
