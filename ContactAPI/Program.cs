using ContactAPI.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using ContactAPI.Validation;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var fieldErrors = new Dictionary<string, string>();
            foreach (var (key, entry) in context.ModelState)
            {
                if (entry.Errors.Count == 0) continue;
                var field = FieldErrors.ToFieldName(key);
                fieldErrors.TryAdd(field, FieldErrors.MessageFor(field));
            }
            return new BadRequestObjectResult(new
            {
                ok = false,
                error = "Invalid request body",
                fieldErrors
            });
        };
    });
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<ContactDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ContactDb")));

//CORS Whitelisting
var allowedOrigins = builder.Configuration
    .GetSection("AllowedOrigins")
    .Get<string[]>() ?? [];
builder.Services.AddCors(options =>
{
    options.AddPolicy("ContactForm", policy =>
    policy.WithOrigins(allowedOrigins)
    .WithMethods("POST")
    .WithHeaders("Content-Type")
    .SetPreflightMaxAge(TimeSpan.FromMinutes(10)));
});

//Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("ContactForm", httpContext =>
    RateLimitPartition.GetFixedWindowLimiter(
        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        factory: _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 5,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0
        }));
    options.OnRejected = async (context, token) =>
    {
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
        {
            context.HttpContext.Response.Headers.RetryAfter =
            ((int)retryAfter.TotalSeconds).ToString();
        }
        await context.HttpContext.Response.WriteAsJsonAsync(
            new { ok = false, error = "Too many requests" }, token);
    };
});

var app = builder.Build();
app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/api/contact"))
    {
        var origin = context.Request.Headers.Origin.ToString();
        if (string.IsNullOrEmpty(origin) || !allowedOrigins.Contains(origin))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new { ok = false, error = "Forbidden" });
            return;
        }
    }
    await next();

});
app.UseCors();

app.UseRateLimiter();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


app.UseAuthorization();

app.MapControllers();

app.Run();
