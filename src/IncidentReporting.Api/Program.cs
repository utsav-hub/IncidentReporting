using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using IncidentReporting.Api.Middleware;
using IncidentReporting.Application.Requests;
using IncidentReporting.Infrastructure;
using IncidentReporting.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// --------------------------------------------------------
// Add Services
// --------------------------------------------------------

// Add controllers
builder.Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        // Use PascalCase to match C# property names
        opt.JsonSerializerOptions.PropertyNamingPolicy = null;
        opt.JsonSerializerOptions.PropertyNameCaseInsensitive = true; // Allow case-insensitive binding
    });

// Add Swagger with JWT support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Incident Reporting API", Version = "v1" });
    
    // Add JWT authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Add JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? "YourSuperSecretKeyThatShouldBeAtLeast32CharactersLongForProduction!";
var issuer = jwtSettings["Issuer"] ?? "IncidentReportingApi";
var audience = jwtSettings["Audience"] ?? "IncidentReportingClient";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

builder.Services.AddAuthorization();

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

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    
    // Set command timeout for migrations
    db.Database.SetCommandTimeout(30);
    
    try
    {
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        // Log migration error but continue startup
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error applying migrations. The application will continue but may not function correctly.");
    }

    // Seed categories if none exist
    if (!db.Categories.Any())
    {
        db.Categories.AddRange(
            new IncidentReporting.Domain.Entities.Category
            {
                Name = "Hardware",
                Description = "Hardware-related incidents",
                IsActive = true
            },
            new IncidentReporting.Domain.Entities.Category
            {
                Name = "Software",
                Description = "Software-related incidents",
                IsActive = true
            },
            new IncidentReporting.Domain.Entities.Category
            {
                Name = "Network",
                Description = "Network connectivity issues",
                IsActive = true
            },
            new IncidentReporting.Domain.Entities.Category
            {
                Name = "Security",
                Description = "Security-related incidents",
                IsActive = true
            },
            new IncidentReporting.Domain.Entities.Category
            {
                Name = "Other",
                Description = "Other types of incidents",
                IsActive = true
            }
        );
        db.SaveChanges();
    }
}


// Use global exception middleware (handles validation, concurrency, etc.)
app.UseMiddleware<ExceptionMiddleware>();

// Swagger - enable in Development or if explicitly enabled via configuration
var enableSwagger = app.Environment.IsDevelopment() || 
                    builder.Configuration.GetValue<bool>("EnableSwagger", false);
if (enableSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
