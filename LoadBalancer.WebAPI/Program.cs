using LoadBalancer.WebAPI.Data;
using LoadBalancer.WebAPI.Helpers;
using LoadBalancer.WebAPI.Hubs;
using LoadBalancer.WebAPI.Middlewares;
using LoadBalancer.WebAPI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<EquationsService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<JwtMiddleware>();

app.MapControllers();

app.MapHub<TaskHub>("/taskHub");

app.Run();