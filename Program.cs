using AstraSoftCR.Services.Interfaces;
using AstraSoftCR.Services;

var builder = WebApplication.CreateBuilder(args);

// ------------------------------------------------------------
// Services
// ------------------------------------------------------------

// Typed HttpClient for the chat service (DeepSeek calls)
builder.Services.AddHttpClient<IChatService, ChatService>();

// MVC controllers
builder.Services.AddControllers();

// OpenAPI/Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS policy
// NOTE: AllowAnyOrigin is acceptable for a public demo. For production,
//       prefer explicit origins: .WithOrigins("https://your-frontend.com", ...)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// ------------------------------------------------------------
// Middleware pipeline
// ------------------------------------------------------------

// Swagger enabled in all environments (including Production)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "AstraSoftCR API v1");
    // Serve Swagger UI at root ("/") to avoid 404 on Render when hitting the base URL.
    c.RoutePrefix = string.Empty;
});

// HTTPS redirection:
// On Render, TLS is terminated at the edge proxy. Enabling UseHttpsRedirection
// inside the container often produces the warning:
//   "Failed to determine the https port for redirect."
// Guard it by environment or disable it entirely in production.
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// CORS (place before endpoints)
app.UseCors("AllowFrontend");

// If you add authentication later, enable the auth middlewares here.
// app.UseAuthentication();
app.UseAuthorization();

// Map attribute-routed controllers
app.MapControllers();

// Optional: If you keep Swagger UI under /swagger instead of root,
// you could redirect "/" to "/swagger" like this:
// app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

app.Run();
