namespace LoadBalancer.WebAPI.Models
{
    public class TaskState
    {
        public int Id { get; set; }
        public double Progress { get; set; }
        public string State { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? Result { get; set; }
    }
}
