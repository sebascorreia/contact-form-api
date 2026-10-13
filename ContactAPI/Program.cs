using ContactAPI.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<ContactDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ContactDb")));

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
var app = builder.Build();

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
