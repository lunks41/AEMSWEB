using AMESWEB.Data;
using AMESWEB.Extensions;
using AMESWEB.Helpers;
using AMESWEB.Hubs;
using AMESWEB.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Add Response Caching
builder.Services.AddResponseCaching();

// Add Response Compression
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

// Authorization Configuration
builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();

// Add these for session and HTTP context access
builder.Services.AddHttpContextAccessor();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true; // Enhanced security
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Strict; // Enhanced security
    options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.None; // Allow HTTP
});

// Enhanced Authorization
builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    options.AddPolicy("PermissionPolicy", policy =>
        policy.Requirements.Add(new PermissionRequirement(0, 0, "")));

    // Add role-based policies
    options.AddPolicy("HRPolicy", policy =>
       policy.RequireRole("HR", "Admin"));

    options.AddPolicy("ManagerPolicy", policy =>
        policy.RequireRole("Manager", "HR", "Admin"));
});

builder.Services.RegisterService(builder.Configuration);

// Enhanced Authentication & Authorization
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Path = "/";
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.HttpOnly = true; // Enhanced security
        options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.None;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
    });

// Database Configuration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// Enhanced Password Policy
builder.Services.AddIdentity<AdmUser, AdmUserGroup>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
    options.Lockout.MaxFailedAccessAttempts = 5;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Cookie Settings for Identity
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true; // Enhanced security
    options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.None; // Use HTTP
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.Cookie.SameSite = SameSiteMode.Lax;
});

builder.Services.AddSignalR();

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.File("Logs/Debug/debug-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File("Logs/Error/error-.log",
        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error,
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddAntiforgery(options =>
{
    options.Cookie.Path = "/";
    options.HeaderName = "X-CSRF-TOKEN";
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}

// Add Response Compression
app.UseResponseCompression();

// Add Response Caching
app.UseResponseCaching();

app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.MapHub<NotificationHub>("/notificationHub");

// Add cache control middleware
app.Use(async (context, next) =>
{
    context.Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate";
    context.Response.Headers["Pragma"] = "no-cache";
    context.Response.Headers["Expires"] = "-1";
    await next();
});

// Custom middleware after auth
app.Use(async (context, next) =>
{
    // Bypass middleware for login and logout endpoints
    var path = context.Request.Path.Value.ToLower();
    if (path.Contains("/login") || path.Contains("/logout"))
    {
        await next();
        return;
    }

    // Continue with companyId check if applicable
    var companyIdV1 = context.GetRouteValue("companyId")?.ToString();
    var session = context.Session;
    var availableCompanies = session.GetObject<List<AdmCompany>>("AvailableCompanies");

    var companyId = context.GetRouteValue("companyId")?.ToString()
              ?? context.Request.Query["companyId"].ToString();
    if (!string.IsNullOrEmpty(companyId))
    {
        if (availableCompanies == null ||
            !availableCompanies.Any(c => c.CompanyId.ToString() == companyId))
        {
            context.Response.Redirect("/Error/Forbidden");
            return;
        }
    }

    await next();
});

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionHandlerPathFeature =
            context.Features.Get<IExceptionHandlerPathFeature>();

        Log.Error("Unhandled Exception: {Exception}",
            exceptionHandlerPathFeature.Error.ToString());

        context.Response.Redirect("/Home/Error");
        await Task.CompletedTask;
    });
});

// Routing
// 1. Company-specific non-area route
app.MapControllerRoute(
    name: "companyRoute",
    pattern: "{companyId:int}/{controller=Dashboard}/{action=Index}/{id?}");

// 2. Area route
app.MapControllerRoute(
    name: "areas",
    pattern: "{companyId:int}/{area:exists}/{controller=Home}/{action=Index}/{id?}");

// 3. Default route (lowest priority)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();