using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace rate.limit.middleware.example.RateLimitingMiddleware
{
    public class RateLimitMiddleware
    {
        private readonly RequestDelegate _next;

        // ConcurrentDictionary to store request counts for each identifier (e.g., IP address)
        // The value tuple stores the count of requests and the timestamp of the last reset
        private readonly ConcurrentDictionary<string, (int Count, DateTime LastReset)> _requestCounts = new ConcurrentDictionary<string, (int, DateTime)>();

        // Time interval after which the request counts are reset
        private readonly TimeSpan _resetInterval = TimeSpan.FromSeconds(10); // Adjust as needed

        public RateLimitMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        // Middleware pipeline entry point
        public async Task InvokeAsync(HttpContext context)
        {
            string ipAddress = context.Connection.RemoteIpAddress?.ToString();
            string identifier = ipAddress;

            // Reset counts if it's time to do so
            foreach (var entry in _requestCounts.ToArray())
            {
                if (DateTime.UtcNow - entry.Value.LastReset > _resetInterval)
                {
                    _requestCounts.TryUpdate(entry.Key, (0, DateTime.UtcNow), entry.Value);
                }
            }

            // Adjust limit based on your requirement
            int limit = 5;

            // Increment request count for the current identifier
            var (count, lastReset) = _requestCounts.GetOrAdd(identifier, (_) => (0, DateTime.UtcNow));
            _requestCounts[identifier] = (count + 1, lastReset);

            // Check if the request count exceeds the limit
            if (count >= limit)
            {
                // If limit is exceeded, return a 429 Too Many Requests response
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsync("Rate limit exceeded.");
                return; // Stop further processing of the request
            }

            // If limit is not exceeded, call the next middleware in the pipeline
            await _next(context);
        }
    }

    // Extension method to use the RateLimitMiddleware in the middleware pipeline
    public static class RateLimitMiddlewareExtensions
    {
        public static IApplicationBuilder UseRateLimitMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RateLimitMiddleware>();
        }

        // We can use our database or other sources to get the information for limiting the rate and count.
        // To use a database:
        // 1. Create a database table/schema to store request counts, along with identifiers (e.g., IP addresses).
        // 2. Modify the RateLimitMiddleware to query and update the database instead of using a ConcurrentDictionary.
        // 3. Inject your database context into the middleware constructor and use it to perform database operations.
        // 4. Ensure proper error handling and concurrency management in database operations.
        // 5. Adjust the rate limit and reset interval according to your application's requirements.
    }
}
