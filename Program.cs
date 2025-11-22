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

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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

// Add API Controllers (REST API)
builder.Services.AddControllers();

// Add JWT Authentication
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
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

// Register Repositories (Infrastructure Layer)
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICampsiteRepository, CampsiteRepository>();

// Register Unit of Work (Application Layer - Transaction Management)
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register Kafka Producer (Event-Driven Architecture)
builder.Services.AddSingleton<IKafkaProducer, KafkaProducer>();

// Register Kafka Consumer as Hosted Service (Background Processing)
// Temporarily disabled to allow app to start
// builder.Services.AddHostedService<KafkaConsumer>();

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
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Map API Controllers
app.MapControllers();

// Apply database migrations automatically
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CampsiteBookingDbContext>();
    db.Database.Migrate();
}

app.Run();