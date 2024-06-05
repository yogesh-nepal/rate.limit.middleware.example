using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace rate.limit.middleware.example.TokenBucketMiddlewares
{
    /// <summary>
    /// Middleware that applies rate limiting using the Token Bucket algorithm.
    /// </summary>
    public class TokenBucketMiddleware
    {
        private readonly RequestDelegate _next;

        // ConcurrentDictionary to store token buckets for each IP address
        private readonly ConcurrentDictionary<string, TokenBucket> _buckets = new ConcurrentDictionary<string, TokenBucket>();

        // Token bucket configuration
        private readonly double _capacity = 5; // Maximum number of tokens in the bucket
        private readonly double _tokensPerSecond = 1; // Rate at which tokens are added per second

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenBucketMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next middleware in the pipeline.</param>
        public TokenBucketMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Processes an incoming HTTP request and applies rate limiting.
        /// </summary>
        /// <param name="context">The HTTP context for the current request.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task InvokeAsync(HttpContext context)
        {
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();

            // If IP address is null, continue to the next middleware
            if (ipAddress == null)
            {
                await _next(context);
                return;
            }

            // Retrieve or create a token bucket for the IP address
            var bucket = _buckets.GetOrAdd(ipAddress, _ => new TokenBucket(_capacity, _tokensPerSecond));

            // Try to consume a token from the bucket
            if (bucket.TryConsume(1))
            {
                // If successful, continue to the next middleware
                await _next(context);
            }
            else
            {
                // If rate limit is exceeded, return a 429 Too Many Requests response
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsync("Rate limit exceeded. using token bucket");
            }
        }
    }
}
