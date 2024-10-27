using Microsoft.AspNetCore.SignalR;

namespace LoadBalancer.WebAPI.Hubs;

public class TaskHub : Hub
{
    public async Task SendTaskUpdate(string taskId, double progress, string state)
    {
        await Clients.All.SendAsync("ReceiveTaskUpdate", taskId, progress, state);
    }
}
