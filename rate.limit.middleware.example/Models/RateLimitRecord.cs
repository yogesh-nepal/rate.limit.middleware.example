namespace rate.limit.middleware.example.Models
{
    // Sample model representing rate limit records in the database
    public class RateLimitRecord
    {
        public string Identifier { get; set; } // E.g., IP address
        public int Count { get; set; }
        public DateTime LastReset { get; set; }
        public TimeSpan ResetInterval { get; set; } // Time interval after which counts are reset
    }
}
