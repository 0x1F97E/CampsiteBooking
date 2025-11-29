using CampsiteBooking.Application.Common;
using CampsiteBooking.Components;
using CampsiteBooking.Data;
using CampsiteBooking.Data.Repositories;
using CampsiteBooking.Infrastructure.Kafka;
using CampsiteBooking.Models.Repositories;
using CampsiteBooking.Services;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using CampsiteBooking.Models;
using CampsiteBooking.Models.ValueObjects;
using CampsiteBooking.Models.Common;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages(); // Add Razor Pages support for login/logout pages
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(options =>
    {
        options.DetailedErrors = builder.Environment.IsDevelopment();
    });

// Add MudBlazor services
builder.Services.AddMudServices();

// Add localization services
builder.Services.AddLocalization();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<LocalizationService>();
builder.Services.AddScoped<GeoLocationService>();

// Add HttpClient for Blazor components
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(sp.GetRequiredService<NavigationManager>().BaseUri)
});

// Configure supported cultures
var supportedCultures = new[]
{
    new CultureInfo("en"),
    new CultureInfo("en-GB"),
    new CultureInfo("da-DK"),
    new CultureInfo("de-DE"),
    new CultureInfo("sv-SE"),
    new CultureInfo("nb-NO"),
    new CultureInfo("nl-NL")
};

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("en");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

// Add HttpClient for Blazor components to call REST API
builder.Services.AddHttpClient("BookMyHomeAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:7001/"); // HTTPS port
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// Add HttpClient for geolocation service
builder.Services.AddHttpClient();

// Register ApiService for Blazor components
builder.Services.AddScoped<ApiService>();

// Register Stripe Payment Service
builder.Services.AddScoped<StripePaymentService>();

// Add API Controllers (REST API)
builder.Services.AddControllers();

// Add Cookie Authentication for Blazor Server
builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {
        options.Cookie.Name = "CampsiteBooking.Auth";
        options.LoginPath = "/login";
        options.LogoutPath = "/logout";
        options.AccessDeniedPath = "/access-denied";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
            ? CookieSecurePolicy.SameAsRequest
            : CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax; // Changed from Strict to Lax to allow cookie on redirects
        options.ExpireTimeSpan = TimeSpan.FromHours(8); // Default session timeout
        options.SlidingExpiration = true; // Extend session on activity
    })
    // Add JWT Authentication for REST API
    .AddJwtBearer("Bearer", options =>
    {
        var jwtSettings = builder.Configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? "BookMyHome-SuperSecret-Key-For-3Semester-Exam-Project-2025";

        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"] ?? "BookMyHome",
            ValidAudience = jwtSettings["Audience"] ?? "BookMyHomeUsers",
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(secretKey))
        };
    });

builder.Services.AddAuthorization();

// Add Circuit Handler to capture authentication state from HTTP request
builder.Services.AddScoped<CampsiteBooking.Services.AuthenticationCircuitHandler>();
builder.Services.AddScoped<Microsoft.AspNetCore.Components.Server.Circuits.CircuitHandler>(
    sp => sp.GetRequiredService<CampsiteBooking.Services.AuthenticationCircuitHandler>());

// Add Blazor Server authentication state provider
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<CampsiteBooking.Services.ServerAuthenticationStateProvider>();
builder.Services.AddScoped<Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider>(
    sp => sp.GetRequiredService<CampsiteBooking.Services.ServerAuthenticationStateProvider>());

// Register Authentication Service
builder.Services.AddScoped<CampsiteBooking.Services.AuthenticationService>();

// Add Anti-forgery for CSRF protection (Blazor Server already has this, but explicit for API)
builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
    options.Cookie.Name = "X-CSRF-TOKEN";
    options.Cookie.HttpOnly = true;
    // Allow HTTP in development, require HTTPS in production
    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
        ? CookieSecurePolicy.SameAsRequest
        : CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Strict; // CSRF protection
});

// Add Swagger/OpenAPI documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "BookMyHome API",
        Version = "v1",
        Description = "REST API for campsite booking system - 3. semester exam project",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "BookMyHome Team"
        }
    });

    // Add JWT authentication to Swagger
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token.",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
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
});

// Add Entity Framework DbContext with MySQL
builder.Services.AddDbContext<CampsiteBookingDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 21))));

// Add DbContextFactory for Blazor components (thread-safe)
// Create a simple factory implementation
builder.Services.AddSingleton<IDbContextFactory<CampsiteBookingDbContext>>(sp =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    return new SimpleDbContextFactory(connectionString!);
});

// Register Repositories (Infrastructure Layer)
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICampsiteRepository, CampsiteRepository>();

// Register Unit of Work (Application Layer - Transaction Management)
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register Email and SMS Services
builder.Services.AddScoped<CampsiteBooking.Services.IEmailService, CampsiteBooking.Services.EmailService>();
builder.Services.AddScoped<CampsiteBooking.Services.ISMSService, CampsiteBooking.Services.SMSService>();

// Register Event Handlers
builder.Services.AddScoped<CampsiteBooking.Infrastructure.EventHandlers.IEventHandler, CampsiteBooking.Infrastructure.EventHandlers.NotificationEventHandler>();

// Register Kafka Producer (Event-Driven Architecture)
builder.Services.AddSingleton<IKafkaProducer, KafkaProducer>();

// Register Kafka Consumer as Hosted Service (Background Processing)
builder.Services.AddHostedService<KafkaConsumer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Enable Swagger in development
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "BookMyHome API v1");
        options.RoutePrefix = "swagger"; // Access at /swagger
    });
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

// Add localization middleware
app.UseRequestLocalization();

// Add Authentication & Authorization middleware (MUST be before MapControllers!)
app.UseAuthentication();
app.UseAuthorization();

// Health check endpoint for YARP
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorPages(); // Add Razor Pages routing
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Map API Controllers
app.MapControllers();

// Add login endpoint for cookie-based authentication
app.MapPost("/api/auth/login", async (LoginRequest request, IDbContextFactory<CampsiteBookingDbContext> dbContextFactory, HttpContext httpContext) =>
{
    try
    {
        using var context = await dbContextFactory.CreateDbContextAsync();

        // Find user by email (load all users first for client-side evaluation)
        var allUsers = await context.Users.ToListAsync();
        var user = allUsers.FirstOrDefault(u => u.Email.Value.Equals(request.Email, StringComparison.OrdinalIgnoreCase));

        if (user == null)
        {
            return Results.Json(new { success = false, error = "Invalid email or password." });
        }

        // Verify password using ASP.NET Core Identity password hasher
        var passwordHasher = new PasswordHasher<User>();
        var verificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

        if (verificationResult == PasswordVerificationResult.Failed)
        {
            Console.WriteLine($"❌ Failed login attempt for {request.Email} - Invalid password");
            return Results.Json(new { success = false, error = "Invalid email or password." });
        }

        Console.WriteLine($"✅ Password verified for {user.Email.Value}");

        // Update last login time
        user.UpdateLastLogin();
        await context.SaveChangesAsync();

        // Create claims for the user
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.Value.ToString()),
            new Claim(ClaimTypes.Email, user.Email.Value),
            new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new Claim("FirstName", user.FirstName),
            new Claim("LastName", user.LastName)
        };

        // Add role claim based on user type
        var userType = user.GetType().Name;
        claims.Add(new Claim(ClaimTypes.Role, userType));

        var identity = new ClaimsIdentity(claims, "CookieAuth");
        var principal = new ClaimsPrincipal(identity);

        // Sign in the user with cookie authentication
        var authProperties = new Microsoft.AspNetCore.Authentication.AuthenticationProperties
        {
            IsPersistent = request.RememberMe, // Remember Me functionality
            ExpiresUtc = request.RememberMe
                ? DateTimeOffset.UtcNow.AddDays(30) // 30 days if "Remember Me" is checked
                : DateTimeOffset.UtcNow.AddHours(8)  // 8 hours session if not checked
        };

        await httpContext.SignInAsync("CookieAuth", principal, authProperties);

        Console.WriteLine($"✅ User {user.Email.Value} logged in successfully (UserId: {user.Id.Value}, RememberMe: {request.RememberMe})");

        return Results.Json(new
        {
            success = true,
            userId = user.Id.Value,
            email = user.Email.Value,
            fullName = $"{user.FirstName} {user.LastName}",
            userType = userType
        });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Login error: {ex.Message}");
        return Results.Json(new { success = false, error = "An error occurred during login. Please try again." });
    }
});

// Add logout endpoint
app.MapPost("/api/auth/logout", async (HttpContext httpContext) =>
{
    await httpContext.SignOutAsync("CookieAuth");
    Console.WriteLine("✅ User logged out successfully");
    return Results.Json(new { success = true });
});

// Add change password endpoint
app.MapPost("/api/auth/change-password", async (HttpContext httpContext, IDbContextFactory<CampsiteBookingDbContext> dbContextFactory) =>
{
    try
    {
        // Get authenticated user from claims (server-side, cannot be tampered with)
        var user = httpContext.User;
        if (user?.Identity?.IsAuthenticated != true)
        {
            Console.WriteLine("❌ Change password failed: User not authenticated");
            return Results.Json(new { success = false, error = "You must be logged in to change your password." });
        }

        // Get user ID from claims (cryptographically signed, cannot be forged)
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            Console.WriteLine("❌ Change password failed: Invalid user ID claim");
            return Results.Json(new { success = false, error = "Invalid authentication state." });
        }

        // Parse request body
        var request = await httpContext.Request.ReadFromJsonAsync<ChangePasswordRequest>();
        if (request == null || string.IsNullOrWhiteSpace(request.CurrentPassword) || string.IsNullOrWhiteSpace(request.NewPassword))
        {
            Console.WriteLine("❌ Change password failed: Invalid request data");
            return Results.Json(new { success = false, error = "Current password and new password are required." });
        }

        // Validate new password
        if (request.NewPassword.Length < 6)
        {
            Console.WriteLine("❌ Change password failed: New password too short");
            return Results.Json(new { success = false, error = "New password must be at least 6 characters long." });
        }

        // Load user from database
        using var context = await dbContextFactory.CreateDbContextAsync();
        var dbUser = await context.Users.FirstOrDefaultAsync(u => u.Id == UserId.Create(userId));

        if (dbUser == null)
        {
            Console.WriteLine($"❌ Change password failed: User {userId} not found in database");
            return Results.Json(new { success = false, error = "User not found." });
        }

        // Verify current password using ASP.NET Core Identity password hasher
        var passwordHasher = new PasswordHasher<User>();
        var verificationResult = passwordHasher.VerifyHashedPassword(dbUser, dbUser.PasswordHash, request.CurrentPassword);

        if (verificationResult == PasswordVerificationResult.Failed)
        {
            Console.WriteLine($"❌ Change password failed for {dbUser.Email?.Value} - Incorrect current password");
            return Results.Json(new { success = false, error = "Current password is incorrect." });
        }

        // Hash the new password
        var newPasswordHash = passwordHasher.HashPassword(dbUser, request.NewPassword);

        // Update password hash in database
        dbUser.SetPasswordHash(newPasswordHash);
        await context.SaveChangesAsync();

        Console.WriteLine($"✅ Password changed successfully for user {dbUser.Email?.Value} (UserId: {userId})");
        return Results.Json(new { success = true });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Change password error: {ex.Message}");
        return Results.Json(new { success = false, error = "An error occurred while changing your password. Please try again." });
    }
});

// Add update profile endpoint
app.MapPost("/api/auth/update-profile", async (HttpContext httpContext, IDbContextFactory<CampsiteBookingDbContext> dbContextFactory) =>
{
    try
    {
        // Get authenticated user from claims (server-side, cannot be tampered with)
        var user = httpContext.User;
        if (user?.Identity?.IsAuthenticated != true)
        {
            Console.WriteLine("❌ Update profile failed: User not authenticated");
            return Results.Json(new { success = false, error = "You must be logged in to update your profile." });
        }

        // Get user ID from claims (cryptographically signed, cannot be forged)
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
        {
            Console.WriteLine("❌ Update profile failed: Invalid user ID claim");
            return Results.Json(new { success = false, error = "Invalid authentication state." });
        }

        // Parse request body
        var request = await httpContext.Request.ReadFromJsonAsync<UpdateProfileRequest>();
        if (request == null)
        {
            Console.WriteLine("❌ Update profile failed: Invalid request data");
            return Results.Json(new { success = false, error = "Invalid request data." });
        }

        // Validate required fields
        if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
        {
            Console.WriteLine("❌ Update profile failed: First name and last name are required");
            return Results.Json(new { success = false, error = "First name and last name are required." });
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            Console.WriteLine("❌ Update profile failed: Email is required");
            return Results.Json(new { success = false, error = "Email is required." });
        }

        // Validate preferred communication
        var validCommunicationOptions = new[] { "Email", "SMS", "Both" };
        if (!validCommunicationOptions.Contains(request.PreferredCommunication))
        {
            Console.WriteLine("❌ Update profile failed: Invalid preferred communication");
            return Results.Json(new { success = false, error = "Invalid preferred communication option." });
        }

        // Load user from database
        using var context = await dbContextFactory.CreateDbContextAsync();
        var dbUser = await context.Users.FirstOrDefaultAsync(u => u.Id == UserId.Create(userId));

        if (dbUser == null)
        {
            Console.WriteLine($"❌ Update profile failed: User {userId} not found in database");
            return Results.Json(new { success = false, error = "User not found." });
        }

        // Check if email is being changed and if new email already exists
        var newEmail = Email.Create(request.Email);
        if (dbUser.Email.Value != newEmail.Value)
        {
            var emailExists = await context.Users.AnyAsync(u => u.Id != UserId.Create(userId) && EF.Property<string>(u, "_email") == newEmail.Value);
            if (emailExists)
            {
                Console.WriteLine($"❌ Update profile failed: Email {request.Email} already exists");
                return Results.Json(new { success = false, error = "This email address is already in use by another account." });
            }

            // Update email
            dbUser.UpdateEmail(newEmail);
        }

        // Update user information using domain methods
        dbUser.UpdateName(request.FirstName, request.LastName);
        dbUser.UpdateContactInfo(request.Phone ?? "", request.Country ?? "");

        // Update preferred communication if user is a Guest
        if (dbUser is Guest guest)
        {
            guest.UpdatePreferredCommunication(request.PreferredCommunication);
        }

        // Save changes to database
        await context.SaveChangesAsync();

        Console.WriteLine($"✅ Profile updated successfully for user {dbUser.Email?.Value} (UserId: {userId})");
        return Results.Json(new { success = true });
    }
    catch (DomainException ex)
    {
        Console.WriteLine($"❌ Update profile validation error: {ex.Message}");
        return Results.Json(new { success = false, error = ex.Message });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Update profile error: {ex.Message}");
        return Results.Json(new { success = false, error = "An error occurred while updating your profile. Please try again." });
    }
});

// Apply database migrations automatically and seed data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CampsiteBookingDbContext>();
    db.Database.Migrate();

    // Seed initial data
    await DatabaseSeeder.SeedAsync(db);
}

app.Run();

// Login request model
public record LoginRequest(string Email, string Password, bool RememberMe);

// Change password request model
public record ChangePasswordRequest(string CurrentPassword, string NewPassword);

// Update profile request model
public record UpdateProfileRequest(string FirstName, string LastName, string Email, string Phone, string Country, string PreferredCommunication);

// Simple DbContext Factory implementation for Blazor components
class SimpleDbContextFactory : IDbContextFactory<CampsiteBookingDbContext>
{
    private readonly string _connectionString;

    public SimpleDbContextFactory(string connectionString)
    {
        _connectionString = connectionString;
    }
    public CampsiteBookingDbContext CreateDbContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<CampsiteBookingDbContext>();
        optionsBuilder.UseMySql(_connectionString, new MySqlServerVersion(new Version(8, 0, 21)));
        return new CampsiteBookingDbContext(optionsBuilder.Options);
    }
}