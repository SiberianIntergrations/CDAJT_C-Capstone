using Microsoft.EntityFrameworkCore;
using back_end.domain.DbContexts;
using back_end.domain.Seeders;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using System.Security.Claims;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    //Prevent circular references/infinite loops when reading the JSON
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
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
    c.AddSecurityRequirement(new OpenApiSecurityRequirement() {
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
            new List<string> ()
        }
    });
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
});

// Add Entity Framework and MySQL/MariaDB connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("MySqlConnection");
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

// Seeders registration
builder.Services.AddDatabaseSeeders();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddMicrosoftIdentityWebApi(builder.Configuration, "AzureAd");
builder.Services.AddAuthorization(options =>
{
    //example policy based on role claims
    options.AddPolicy("projectManagersOnly", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole("projectmanager");
    });
});


builder.Services.AddAuthorization();

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
        c.OAuthClientId("99ffb099-af80-41d1-9c16-e844f8ed5308");
        c.OAuthAppName("Sushi Toshi App API");
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
            if (!context.Users.Any())
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
