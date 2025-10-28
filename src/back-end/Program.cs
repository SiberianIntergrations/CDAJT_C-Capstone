using Microsoft.EntityFrameworkCore;
using back_end.domain.DbContexts;
using back_end.domain.Seeders;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using System.Security.Claims;
using System.Text.Json.Serialization;


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
    
    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\""
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
    c.EnableAnnotations();
});

// Add Entity Framework and MySQL/MariaDB connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("MySqlConnection");
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

// Seeders registration
builder.Services.AddDatabaseSeeders();

// QR Code Generation Service (Windows-only due to System.Drawing dependency)
if (OperatingSystem.IsWindows())
{
    builder.Services.AddScoped<back_end.Services.QrCode.QrGeneratorService>();
}

// JWT Auth
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.IncludeErrorDetails = builder.Environment.IsDevelopment();
    options.TokenValidationParameters = new TokenValidationParameters
    {
        //We'll need to set these to true - but okay now for testing
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
        NameClaimType = ClaimTypes.NameIdentifier,
        RoleClaimType = ClaimTypes.Role,
        ClockSkew = TimeSpan.Zero
    };

    // Force JwtSecurityTokenHandler
    options.TokenHandlers.Clear();
    options.TokenHandlers.Add(new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler());
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