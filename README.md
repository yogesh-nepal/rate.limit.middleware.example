# Rate Limit Middleware for .NET 8

Rate Limit Middleware for .NET 8 is a middleware implementation designed to enforce rate limits on incoming requests to ASP.NET Core applications. This middleware provides a flexible solution for controlling the rate of incoming requests based on IP address.

## Description

Rate limiting is an essential aspect of building robust web applications, preventing abuse, and ensuring fair usage of resources. This middleware offers multiple implementations:

### Rate Limit Middleware

The Rate Limit Middleware is a straightforward implementation that uses an in-memory data structure (ConcurrentDictionary) to track request counts per identifier (e.g., IP address) within a specified time interval. It blocks further requests if the limit is exceeded.

### Rate Limit Middleware Using Database

The Rate Limit Middleware Using Database extends the basic rate limiting functionality by storing request counts in a database. This enables persistence across application restarts and scalability across multiple instances of the application. It demonstrates how to integrate Entity Framework Core to manage request counts in a relational database.

### Rate Limit Middleware Using Token Bucket Algorithm

The Rate Limit Middleware Using Token Bucket Algorithm regulates the flow of requests using a token bucket mechanism. This algorithm provides smooth request rate management by allowing a certain burst of requests followed by a constant rate.

### Rate Limit Middleware Using Third-Party Libraries

This implementation uses third-party libraries such as AspNetCoreRateLimit and Polly to handle rate limiting with ease. These libraries offer robust and configurable rate limiting mechanisms that can be easily integrated into your application.

## Usage

To test the different rate limit implementations, you should comment out the not needed implementations in the `Program.cs` file and use only one implementation at a time.
