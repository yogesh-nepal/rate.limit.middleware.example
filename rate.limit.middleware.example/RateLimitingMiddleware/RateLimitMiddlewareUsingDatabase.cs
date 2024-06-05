using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using rate.limit.middleware.example.DatabaseContext;
using rate.limit.middleware.example.Models;

namespace rate.limit.middleware.example.RateLimitingMiddleware
{
    public class RateLimitMiddlewareUsingDatabase
    {
        private readonly RequestDelegate _next;

        // Constructor injection of database context or repository
        private readonly MyDatabaseContext _dbContext; // Replace YourDatabaseContext with your actual database context

        public RateLimitMiddlewareUsingDatabase(RequestDelegate next, MyDatabaseContext dbContext)
        {
            _next = next;
            _dbContext = dbContext;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Step 1: Get the identifier (e.g., IP address)
            string ipAddress = context.Connection.RemoteIpAddress?.ToString();
            string identifier = ipAddress;

            // Step 2: Query the database to get the rate limit record for the identifier
            var rateLimitRecord = await _dbContext.RateLimitRecords.FindAsync(identifier);

            // Step 3: Reset the request count if needed
            if (rateLimitRecord != null && DateTime.UtcNow - rateLimitRecord.LastReset > rateLimitRecord.ResetInterval)
            {
                rateLimitRecord.Count = 0;
                rateLimitRecord.LastReset = DateTime.UtcNow;
            }

            // Step 4: Increment the request count
            rateLimitRecord ??= new RateLimitRecord { Identifier = identifier, Count = 0, LastReset = DateTime.UtcNow };
            rateLimitRecord.Count++;
            _dbContext.RateLimitRecords.Update(rateLimitRecord);
            await _dbContext.SaveChangesAsync();

            // Step 5: Check if the request count exceeds the limit
            int limit = 5; // Adjust as needed
            if (rateLimitRecord.Count >= limit)
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsync("Rate limit exceeded.");
                return;
            }

            // If limit is not exceeded, call the next middleware in the pipeline
            await _next(context);
        }
    }

    public static class RateLimitMiddlewareUsingDatabaseExtensions
    {
        public static IApplicationBuilder UseRateLimitMiddlewareUsingDatabase(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RateLimitMiddlewareUsingDatabase>();
        }
    }
}
