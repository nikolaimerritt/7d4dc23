using System.Runtime.CompilerServices;
using CTFWhodunnit.Database;
using Hangfire;
using Hangfire.Storage.SQLite;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore;
using PirateConquest.Repositories;
using PirateConquest.Services;
using PirateConquest.ViewModels;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var mvcBuilder = builder.Services.AddControllersWithViews();
if (builder.Environment.IsDevelopment())
{
    mvcBuilder.AddRazorRuntimeCompilation();
}
builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

builder
    .Services.AddAuthentication("CookieAuth")
    .AddCookie(
        "CookieAuth",
        config =>
        {
            config.Cookie.Name = "User.Cookie";
            config.LoginPath = "/Login/Index";
        }
    );

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("IsAdminPolicy", policy => policy.RequireClaim("IsAdmin", "True"));
});

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor
        | ForwardedHeaders.XForwardedProto
        | ForwardedHeaders.XForwardedHost;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

builder.Services.AddHangfire(options => options.UseSQLiteStorage());

builder.Services.AddCoreAdmin();
builder.Services.AddTransient<SeaRepository>();
builder.Services.AddTransient<OutcomeRepository>();
builder.Services.AddTransient<PurchaseRepository>();
builder.Services.AddTransient<MoveRepository>();
builder.Services.AddTransient<RoundRepository>();
builder.Services.AddTransient<TeamRepository>();
builder.Services.AddTransient<OutcomeService>();
builder.Services.AddTransient<BackgroundJobClient>();
builder.Services.AddTransient<DatabaseInitialiser>();

var app = builder.Build();
app.UseForwardedHeaders();
app.UseCoreAdminCustomAuth(
    async (serviceProvider) =>
    {
        // Get IHttpContextAccessor from serviceProvider
        var httpContextAccessor =
            serviceProvider.GetService(typeof(IHttpContextAccessor)) as IHttpContextAccessor;

        // Get the HttpContext
        var httpContext = httpContextAccessor.HttpContext;

        // Extract the user from the HttpContext
        var user = httpContext.User;

        // Check if the user is authenticated
        if (user.Identity.IsAuthenticated)
        {
            // Assume you have a custom claim or a property that marks the user as admin
            // Replace 'IsAdmin' with whatever you use to mark a user as admin
            if (user.HasClaim("IsAdmin", "True"))
            {
                return await Task.FromResult(true);
            }
        }

        return await Task.FromResult(false);
    }
);

using (var scope = app.Services.CreateScope())
{
    var databaseInitialiser = scope.ServiceProvider.GetService<DatabaseInitialiser>();
    await databaseInitialiser.Initialise();
}

app.UseHangfireServer();

// TO SELF: debug
app.UseHangfireDashboard("/hangfire");

using (var scope = app.Services.CreateScope())
{
    var backgroundJobClient = scope.ServiceProvider.GetService<BackgroundJobClient>();
    var roundRepository = scope.ServiceProvider.GetService<RoundRepository>();
    var outcomeService = scope.ServiceProvider.GetService<OutcomeService>();

    foreach (var round in await roundRepository.AllPlayableRounds())
    {
        backgroundJobClient.Schedule(
            () => outcomeService.WriteOutcomes(round),
            round.StartFighting
        );
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseAuthentication();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapDefaultControllerRoute();
app.Run();
