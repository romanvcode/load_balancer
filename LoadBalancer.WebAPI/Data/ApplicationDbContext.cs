using LoadBalancer.WebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LoadBalancer.WebAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<TaskState> TaskStates { get; set; }
    }
}
