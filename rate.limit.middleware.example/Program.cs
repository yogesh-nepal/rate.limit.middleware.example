
using Microsoft.EntityFrameworkCore;
using rate.limit.middleware.example.DatabaseContext;
using rate.limit.middleware.example.RateLimitingMiddleware;

namespace rate.limit.middleware.example
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Read connection string from appsettings.json
            var connectionString = builder.Configuration.GetConnectionString("YourDatabaseConnectionString");

            // Register database context with connection string
            builder.Services.AddDbContext<MyDatabaseContext>(options =>
            {
                options.UseSqlServer(connectionString); // Replace with your database provider (e.g., UseMySQL, UseNpgsql, etc.)
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            // Use the rate limit middleware
            app.UseRateLimitMiddleware();
            app.UseRateLimitMiddlewareUsingDatabase();


            app.MapControllers();

            app.Run();
        }
    }
}
