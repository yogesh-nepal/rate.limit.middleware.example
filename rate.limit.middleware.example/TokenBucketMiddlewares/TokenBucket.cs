namespace rate.limit.middleware.example.TokenBucketMiddlewares
{
    /// <summary>
    /// Represents a token bucket used for rate limiting.
    /// The bucket has a capacity and refills tokens at a specified rate.
    /// </summary>
    public class TokenBucket
    {
        // Lock object to ensure thread safety when accessing tokens
        private readonly object _lock = new object();

        // Maximum capacity of the token bucket
        private readonly double _capacity;

        // Rate at which tokens are added to the bucket per second
        private readonly double _tokensPerSecond;

        // Current number of tokens available in the bucket
        private double _tokens;

        // Timestamp of the last time tokens were refilled
        private DateTime _lastRefillTimestamp;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenBucket"/> class.
        /// </summary>
        /// <param name="capacity">The maximum number of tokens the bucket can hold.</param>
        /// <param name="tokensPerSecond">The rate at which tokens are added to the bucket per second.</param>
        public TokenBucket(double capacity, double tokensPerSecond)
        {
            _capacity = capacity;
            _tokensPerSecond = tokensPerSecond;
            _tokens = capacity; // Initially, the bucket is full
            _lastRefillTimestamp = DateTime.UtcNow; // Set the initial refill timestamp
        }

        /// <summary>
        /// Tries to consume a specified number of tokens from the bucket.
        /// </summary>
        /// <param name="tokens">The number of tokens to consume.</param>
        /// <returns>True if the tokens were successfully consumed; otherwise, false.</returns>
        public bool TryConsume(double tokens)
        {
            lock (_lock)
            {
                // Refill tokens before attempting to consume
                Refill();

                // Check if there are enough tokens to consume
                if (_tokens >= tokens)
                {
                    _tokens -= tokens; // Consume the tokens
                    return true; // Successfully consumed
                }

                return false; // Not enough tokens to consume
            }
        }

        /// <summary>
        /// Refills the token bucket based on the elapsed time since the last refill.
        /// </summary>
        private void Refill()
        {
            var now = DateTime.UtcNow;
            var delta = (now - _lastRefillTimestamp).TotalSeconds; // Calculate the time elapsed since last refill
            var tokensToAdd = delta * _tokensPerSecond; // Calculate the tokens to add based on the elapsed time

            // Add the calculated tokens to the bucket, ensuring it does not exceed capacity
            _tokens = Math.Min(_capacity, _tokens + tokensToAdd);
            _lastRefillTimestamp = now; // Update the last refill timestamp
        }
    }
}
