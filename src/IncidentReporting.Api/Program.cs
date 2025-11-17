using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using IncidentReporting.Infrastructure;
using IncidentReporting.Application.Requests;
using IncidentReporting.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// --------------------------------------------------------
// Add Services
// --------------------------------------------------------

// Add controllers
builder.Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS (allow all for development)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyMethod()
              .AllowAnyHeader()
              .AllowAnyOrigin();
    });
});

// Add Application + Infrastructure layers
builder.Services.AddInfrastructure(builder.Configuration);

// Add MediatR (Application assembly)
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<CreateIncidentCommand>();
});

// Add FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<CreateIncidentCommand>();
builder.Services.AddFluentValidationAutoValidation();

// --------------------------------------------------------
// Build Application
// --------------------------------------------------------

var app = builder.Build();

// Use global exception middleware (handles validation, concurrency, etc.)
app.UseMiddleware<ExceptionMiddleware>();

// Swagger (enable only in development)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
