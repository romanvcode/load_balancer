using LoadBalancer.WebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LoadBalancer.WebAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<TaskState> TaskStates { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaskState>().HasKey(x => x.Id);

            modelBuilder.Entity<User>().HasData(
               new User
               {
                   Id = 1,
                   FirstName = "System",
                   LastName = "",
                   Username = "System",
                   Password = "System",
               }
           );
        }
    }
}
