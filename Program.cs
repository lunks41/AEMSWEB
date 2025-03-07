using AEMSWEB.Data;
using AEMSWEB.Extension;
using AEMSWEB.Extensions;
using AEMSWEB.Helpers;
using AEMSWEB.Hubs;
using AEMSWEB.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Authorization Configuration
builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();

// Add these for session and HTTP context access
builder.Services.AddHttpContextAccessor();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = false;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.None; // Allow HTTP
});

builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    options.AddPolicy("PermissionPolicy", policy =>
        policy.Requirements.Add(new PermissionRequirement(0, 0, "")));
});

builder.Services.RegisterService(builder.Configuration);

// Authentication & Authorization
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Path = "/";
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.HttpOnly = false;
        options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.None; // Use HTTP
        options.Cookie.SameSite = SameSiteMode.Strict; // Adjust as needed
    });

// Database Configuration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

// Identity Configuration
builder.Services.AddIdentity<AdmUser, AdmUserGroup>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Cookie Settings for Identity
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = false;
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

// Database Seeding
// await SeedService.SeedDatabase(app.Services);

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // app.UseHsts(); // Removed for HTTP-only
}

// Do not use HTTPS redirection when using only HTTP
// app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.MapHub<NotificationHub>("/notificationHub");

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

//using AEMSWEB.Data;
//using AEMSWEB.Extension;
//using AEMSWEB.Extensions;
//using AEMSWEB.Helpers;
//using AEMSWEB.Hubs;
//using AEMSWEB.Models;
//using Microsoft.AspNetCore.Authentication.Cookies;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Diagnostics;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Serilog;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container
//builder.Services.AddControllersWithViews();

//// Authorization Configuration
//builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();

//// Add these for session and HTTP context access
//builder.Services.AddHttpContextAccessor();

//builder.Services.AddSession(options =>
//{
//    options.IdleTimeout = TimeSpan.FromMinutes(30);
//    options.Cookie.HttpOnly = false;
//    options.Cookie.IsEssential = true;
//    options.Cookie.SameSite = SameSiteMode.Lax;
//});

//builder.Services.AddAuthorization(options =>
//{
//    options.DefaultPolicy = new AuthorizationPolicyBuilder()
//        .RequireAuthenticatedUser()
//        .Build();

//    options.AddPolicy("PermissionPolicy", policy =>
//        policy.Requirements.Add(new PermissionRequirement(0, 0, ""))); ;
//});

//builder.Services.RegisterService(builder.Configuration);

//// Authentication & Authorization
//builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//    .AddCookie(options =>
//    {
//        options.Cookie.Path = "/";
//        options.LoginPath = "/Account/Login";
//        options.LogoutPath = "/Account/Logout";
//        options.AccessDeniedPath = "/Account/AccessDenied";
//        options.Cookie.HttpOnly = false;
//        options.Cookie.SameSite = SameSiteMode.Strict; // Adjust based on your setup
//    });

//// Database Configuration
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

////builder.Services.AddScoped<INotificationHub, NotificationHub>();

//// Identity Configuration
//builder.Services.AddIdentity<AdmUser, AdmUserGroup>(options =>
//{
//    options.Password.RequireDigit = true;
//    options.Password.RequireLowercase = true;
//    options.Password.RequiredLength = 6;
//    options.User.RequireUniqueEmail = true;
//    options.SignIn.RequireConfirmedAccount = false;
//    options.SignIn.RequireConfirmedEmail = false;
//    options.SignIn.RequireConfirmedPhoneNumber = false;
//})
//    .AddEntityFrameworkStores<ApplicationDbContext>()
//    .AddDefaultTokenProviders();

//// Cookie Settings
//builder.Services.ConfigureApplicationCookie(options =>
//{
//    options.Cookie.HttpOnly = false;
//    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
//    options.SlidingExpiration = true;
//    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
//    options.Cookie.SameSite = SameSiteMode.Lax;
//});

//builder.Services.AddSignalR();

//// Configure Serilog
//Log.Logger = new LoggerConfiguration()
//    .MinimumLevel.Debug()
//    .WriteTo.File("Logs/Debug/debug-.log",
//        rollingInterval: RollingInterval.Day,
//        retainedFileCountLimit: 7,
//        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
//    .WriteTo.File("Logs/Error/error-.log",
//        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error,
//        rollingInterval: RollingInterval.Day,
//        retainedFileCountLimit: 30,
//        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
//    .CreateLogger();

//builder.Host.UseSerilog((context, configuration) =>
//    configuration.ReadFrom.Configuration(context.Configuration));

//builder.Services.AddAntiforgery(options =>
//{
//    options.Cookie.Path = "/";
//    options.HeaderName = "X-CSRF-TOKEN";
//});

//var app = builder.Build();

//// Database Seeding
////await SeedService.SeedDatabase(app.Services);

//if (app.Environment.IsDevelopment())
//{
//    app.UseDeveloperExceptionPage();
//}

//// Configure the HTTP pipeline
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Home/Error");
//    app.UseHsts();
//}

////app.UseHttpsRedirection();
//app.UseStaticFiles();
//app.UseRouting();
//app.UseSession();
//app.UseAuthentication();
//app.UseAuthorization();
//app.MapHub<NotificationHub>("/notificationHub");

//// Custom middleware after auth
//app.Use(async (context, next) =>
//{
//    // Bypass middleware for login and logout endpoints
//    var path = context.Request.Path.Value.ToLower();
//    if (path.Contains("/login") || path.Contains("/logout"))
//    {
//        await next();
//        return;
//    }

//    // Continue with companyId check if applicable
//    var companyIdV1 = context.GetRouteValue("companyId")?.ToString();
//    var session = context.Session;
//    var availableCompanies = session.GetObject<List<AdmCompany>>("AvailableCompanies");

//    // Only redirect if a companyId is present and availableCompanies is not null
//    //var companyId = context.Request.Query["companyId"].ToString();
//    var companyId = context.GetRouteValue("companyId")?.ToString()
//              ?? context.Request.Query["companyId"].ToString();
//    if (!string.IsNullOrEmpty(companyId))
//    {
//        // Get from cache/database if session unavailable
//        //var availableCompanies = session.GetObject<List<AdmCompany>>("AvailableCompanies")
//        //                      ?? await GetCompaniesFromDatabase(context.User);

//        if (availableCompanies == null ||
//            !availableCompanies.Any(c => c.CompanyId.ToString() == companyId))
//        {
//            context.Response.Redirect("/Error/Forbidden");
//            return;
//        }
//    }

//    await next();

//    //// Refresh session if valid
//    //if (context.User.Identity.IsAuthenticated)
//    //    context.Session.Refresh();
//});

//app.UseExceptionHandler(errorApp =>
//{
//    errorApp.Run(async context =>
//    {
//        var exceptionHandlerPathFeature =
//            context.Features.Get<IExceptionHandlerPathFeature>();

//        // Log full error
//        Log.Error("Unhandled Exception: {Exception}",
//            exceptionHandlerPathFeature.Error.ToString());

//        context.Response.Redirect("/Home/Error");
//        await Task.CompletedTask;
//    });
//});

//// Routing
//// 1. Company-specific non-area route
//app.MapControllerRoute(
//    name: "companyRoute",
//    pattern: "{companyId:int}/{controller=Dashboard}/{action=Index}/{id?}");

//// 2. Area route
//app.MapControllerRoute(
//    name: "areas",
//    pattern: "{companyId:int}/{area:exists}/{controller=Home}/{action=Index}/{id?}");

//// 3. Default route (lowest priority)
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Account}/{action=Login}/{id?}");

//app.Run();