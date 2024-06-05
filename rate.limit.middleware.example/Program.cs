
using AspNetCoreRateLimit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Polly;
using Polly.RateLimit;
using rate.limit.middleware.example.DatabaseContext;
using rate.limit.middleware.example.RateLimitingMiddleware;
using rate.limit.middleware.example.TokenBucketMiddlewares;

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

            #region Rate limit using polly
            // Define a rate limit policy: 1 request per five second
            var rateLimitPolicy = Policy.RateLimitAsync(1, TimeSpan.FromSeconds(5));

            builder.Services.AddSingleton(rateLimitPolicy);
            #endregion
            // Read connection string from appsettings.json
            var connectionString = builder.Configuration.GetConnectionString("YourDatabaseConnectionString");

            // Register database context with connection string
            builder.Services.AddDbContext<MyDatabaseContext>(options =>
            {
                options.UseSqlServer(connectionString); // Replace with your database provider (e.g., UseMySQL, UseNpgsql, etc.)
            });

            #region for ratelimit using aspnetcoreratelimit
            // Load configuration from appsettings.json
            builder.Services.AddOptions();
            builder.Services.AddMemoryCache();
            // Load general configuration
            builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));

            // Load IP rules from appsettings.json
            builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

            builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            // Add rate limiting
            builder.Services.AddInMemoryRateLimiting();
            #endregion


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

            //// Enable IP rate limiting middleware for ratelimit using aspnetcoreratelimit
            app.UseIpRateLimiting();

            // Use the Token Bucket Middleware
            app.UseMiddleware<TokenBucketMiddleware>();


            app.MapControllers();

            app.Run();
        }
    }
}
