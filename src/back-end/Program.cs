using Microsoft.EntityFrameworkCore;
using back_end.domain.DbContexts;
using back_end.domain.Seeders;
using back_end.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using System.Security.Claims;
using Microsoft.Identity.Web;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    //Prevent circular references/infinite loops when reading the JSON
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
// Removed invalid field declaration. If you need the directory name, use the following line directly:

Console.WriteLine(Path.GetPathRoot(Directory.GetCurrentDirectory()));

// Add Swagger/OpenAPI services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Sushi Toshi API",
        Version = "v1",
        Description = "API for Sushi Toshi Restaurant Management System"
    });
    var scopes = new Dictionary<string, string>()
    { };
    scopes.Add($"{builder.Configuration["ApiScopeUrl"]}user_impersonation", "Access application on user behalf");
    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            Implicit = new OpenApiOAuthFlow()
            {
                AuthorizationUrl = new Uri("https://renovationstationexsm3943.ciamlogin.com/e90f24c7-0844-464a-a0b1-aab345a6adff/oauth2/v2.0/authorize"),
                //TokenUrl = new Uri("https://renovationstationexsm3943.ciamlogin.com/e90f24c7-0844-464a-a0b1-aab345a6adff/oauth2/v2.0/token"),
                Scopes = scopes,
                
            }
        }
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "oauth2"
                },
                Scheme = "oauth2",
                Name = "oauth2",
                In = ParameterLocation.Header
            },
            new List<string> { $"{builder.Configuration["ApiScopeUrl"]}user_impersonation" }
        }
    });
    c.EnableAnnotations();
});

// Add Entity Framework and MySQL/MariaDB connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("MySqlConnection");
    // Use static server version to avoid connection attempt during startup (important for testing)
    // Using MariaDB 10.5 as a reasonable baseline version
    options.UseMySql(connectionString, new MariaDbServerVersion(new Version(10, 5, 0)));
});

// Seeders registration
builder.Services.AddDatabaseSeeders();

// QR Code Generation Service - Single cross-platform registration
builder.Services.AddScoped<QrGeneratorService>();

// JWT Auth - Microsoft Identity Web API for Azure AD
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(options =>
    {
        builder.Configuration.Bind("AzureAd", options);

        // Configure token validation parameters
        options.TokenValidationParameters.ValidateIssuer = true;
        options.TokenValidationParameters.ValidateAudience = true;
        options.TokenValidationParameters.ValidateLifetime = true;
        options.TokenValidationParameters.ValidateIssuerSigningKey = true;

        // Accept both the ClientId and the api:// format for audience
        options.TokenValidationParameters.ValidAudiences = new[]
        {
            builder.Configuration["AzureAd:ClientId"],
            builder.Configuration["AzureAd:Audience"],
            $"api://{builder.Configuration["AzureAd:ClientId"]}"
        };

        // Log token validation failures
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"=== Authentication Failed ===");
                Console.WriteLine($"Exception: {context.Exception.Message}");
                Console.WriteLine($"Exception Type: {context.Exception.GetType().Name}");
                if (context.Exception.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {context.Exception.InnerException.Message}");
                }
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("=== Token Validated Successfully ===");
                var claims = context.Principal?.Claims;
                if (claims != null)
                {
                    Console.WriteLine("Claims:");
                    foreach (var claim in claims)
                    {
                        Console.WriteLine($"  {claim.Type}: {claim.Value}");
                    }
                }
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                Console.WriteLine($"=== OnChallenge ===");
                Console.WriteLine($"Error: {context.Error ?? "null"}");
                Console.WriteLine($"ErrorDescription: {context.ErrorDescription ?? "null"}");
                Console.WriteLine($"ErrorUri: {context.ErrorUri ?? "null"}");
                return Task.CompletedTask;
            },
            OnMessageReceived = context =>
            {
                var token = context.Request.Headers["Authorization"].ToString();
                if (!string.IsNullOrEmpty(token))
                {
                    Console.WriteLine($"=== Token Received ===");
                    Console.WriteLine($"Auth Header (first 50 chars): {token.Substring(0, Math.Min(50, token.Length))}...");
                }
                return Task.CompletedTask;
            }
        };
    },
    options => { builder.Configuration.Bind("AzureAd", options); });

builder.Services.AddAuthorization(options =>
{

    options.AddPolicy("adminOnly", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole("user.Admin");
    });
    options.AddPolicy("staffOnly", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole(["user.Staff","user.Admin"]);
    });

});

// Add rate limiting for API protection
builder.Services.AddRateLimiter(options =>
{
    // Fixed window rate limiter for general API calls
    options.AddFixedWindowLimiter("api", config =>
    {
        config.PermitLimit = 100; // 100 requests
        config.Window = TimeSpan.FromMinutes(1); // per minute
        config.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        config.QueueLimit = 5;
    });

    // Stricter rate limiting for authentication endpoints
    options.AddFixedWindowLimiter("auth", config =>
    {
        config.PermitLimit = 10; // 10 requests
        config.Window = TimeSpan.FromMinutes(1); // per minute
        config.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        config.QueueLimit = 2;
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

// Add CORS policy (environment-based configuration)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        if (builder.Environment.IsDevelopment())
        {
            // Development: Allow local frontend ports
            policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        }
        else
        {
            // Production: Use specific origins from configuration
            var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                ?? Array.Empty<string>();

            policy.WithOrigins(allowedOrigins)
                  .WithMethods("GET", "POST", "PUT", "DELETE", "PATCH")
                  .WithHeaders("Content-Type", "Authorization")
                  .AllowCredentials();
        }
    });
});

// Services
builder.Services.AddScoped<IPricingService, PricingService>();

// Temporary //**
Console.WriteLine(">>> USING CONNECTION STRING:");
Console.WriteLine(builder.Configuration.GetConnectionString("MySqlConnection"));

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    // Swagger UI found at http://localhost:5264/swagger
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sushi Toshi API v1");
        c.RoutePrefix = "swagger"; // Access Swagger UI at /swagger
    });
}
else
{
    // Global exception handler for production
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var exceptionHandlerPathFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();
            var exception = exceptionHandlerPathFeature?.Error;

            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError(exception, "Unhandled exception occurred");

            await context.Response.WriteAsJsonAsync(new
            {
                error = "An unexpected error occurred. Please try again later.",
                requestId = context.TraceIdentifier
            });
        });
    });

    // Enable HTTPS redirection in production
    app.UseHttpsRedirection();
}

app.UseRateLimiter();

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

// Map controllers
app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => "API is running!");

// Test database connection endpoint
app.MapGet("/db-test", async (ApplicationDbContext context) =>
{
    try
    {
        await context.Database.CanConnectAsync();
        return Results.Ok("Database connection successful!");
    }
    catch (Exception ex)
    {
        return Results.Problem($"Database connection failed: {ex.Message}");
    }
});

// Database initialization on startup
// This section handles:
// 1. Testing database connection
// 2. Applying EF Core migrations
// 3. Seeding initial data using the DatabaseSeeder service
// Note: The application will start even if database connection fails
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        // Test the connection first
        var canConnect = await context.Database.CanConnectAsync();
        if (canConnect)
        {
            logger.LogInformation("Database connection successful");

            // Apply pending migrations to update database schema
            await context.Database.MigrateAsync();
            logger.LogInformation("Database migrations applied successfully");

            // Seed initial data (users, roles, default settings, etc.)
            // The DatabaseSeeder should be idempotent and check if data already exists

            //Check to see if the DB has been seeded already
            if (!context.MenuItems.Any())
            {
                await seeder.SeedDatabase();
                logger.LogInformation("Database seed completed.");
            }
            else
            {
                logger.LogInformation("Database has already been seeded.");
            }
        }
        else
        {
            logger.LogWarning("Cannot connect to database");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred during database initialization: {Message}", ex.Message);
        // Don't throw - allow the application to start even if DB is not ready
        // This is useful for containerized environments where DB might start after the app
    }
}

app.Run();

// Make Program class accessible to integration tests
public partial class Program { }
