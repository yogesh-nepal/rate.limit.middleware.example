# Rate Limit Middleware for .NET 8

Rate Limit Middleware for .NET 8 is a middleware implementation designed to enforce rate limits on incoming requests to ASP.NET Core applications. This middleware provides a flexible solution for controlling the rate of incoming requests based on IP address.

## Description

Rate limiting is an essential aspect of building robust web applications, preventing abuse, and ensuring fair usage of resources. This middleware offers two implementations:

### Rate Limit Middleware

The Rate Limit Middleware is a straightforward implementation that uses an in-memory data structure (ConcurrentDictionary) to track request counts per identifier (e.g., IP address) within a specified time interval. It blocks further requests if the limit is exceeded.

### Rate Limit Middleware Using Database

The Rate Limit Middleware Using Database extends the basic rate limiting functionality by storing request counts in a database. This enables persistence across application restarts and scalability across multiple instances of the application. It demonstrates how to integrate Entity Framework Core to manage request counts in a relational database.
