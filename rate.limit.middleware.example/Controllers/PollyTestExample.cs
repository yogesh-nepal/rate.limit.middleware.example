using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Polly.RateLimit;

namespace rate.limit.middleware.example.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PollyTestExample : ControllerBase
    {
        private readonly AsyncRateLimitPolicy _rateLimitPolicy;

        public PollyTestExample(AsyncRateLimitPolicy rateLimitPolicy)
        {
            _rateLimitPolicy = rateLimitPolicy;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                await _rateLimitPolicy.ExecuteAsync(async () =>
                {
                    // Your action logic here
                    await Task.Delay(100); // Simulate some async work
                });

                return Ok("Request processed successfully.");
            }
            catch (RateLimitRejectedException)
            {
                return StatusCode(429, "Rate limit exceeded.");
            }
        }
    }
}
