using AstraSoftCR.Services.Interfaces;
using AstraSoftCR.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient<IChatService, ChatService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS Policy
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowFrontend", policy =>
	{
		policy.AllowAnyOrigin()
			  .AllowAnyHeader()
			  .AllowAnyMethod();
	});
});

var app = builder.Build();

// ✅ Swagger habilitado en todos los entornos (incluye producción)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
	c.SwaggerEndpoint("/swagger/v1/swagger.json", "AstraSoftCR API v1");
	c.RoutePrefix = "swagger"; // acceder con /swagger
});

app.UseHttpsRedirection();

// CORS Middleware
app.UseCors("AllowFrontend");

app.UseAuthorization();

app.MapControllers();

app.Run();
