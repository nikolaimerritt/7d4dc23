using Hangfire;
using Hangfire.Storage.SQLite;
using Microsoft.AspNetCore.HttpOverrides;
using PirateConquest.Database;
using PirateConquest.Models;
using PirateConquest.Repositories;
using PirateConquest.Services;
using PirateConquest.Utils;

var builder = WebApplication.CreateBuilder(args);
Console.WriteLine("Built app. Configuring.");
var app = await InitializeServicesAsync(builder);
Console.WriteLine(
    "Configured app. Waiting for an admin to write a configuration entry in the database."
);
var configuration = await WaitForConfigurationAsync(app);
Console.WriteLine($"Found a configuration entry. Initializing the database.");
await InitializeDatabaseAsync(app, configuration);
Console.WriteLine("Database initialized. Running the web server.");
builder.WebHost.UseUrls("http://*:5000");
app.Run();

static async Task<WebApplication> InitializeServicesAsync(WebApplicationBuilder builder)
{
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

    builder.Services.AddHangfire(options =>
    {
        options.UseSQLiteStorage();
        options.UseFilter(new AutomaticRetryAttribute() { Attempts = 1 });
    });

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
    builder.Services.AddTransient<PointsService>();
    builder.Services.AddTransient<ConfigurationRepository>();
    builder.Services.AddTransient<MessageRepository>();

    builder.Services.AddControllers(options =>
        options.InputFormatters.Add(new PlainTextInputFormatter())
    );
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

    app.UseHangfireServer();
    JobStorage.Current?.GetMonitoringApi().PurgeJobs();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/ErrorViewModel");
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
    return app;
}

static async Task<Configuration> WaitForConfigurationAsync(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        var configurationRepository = scope.ServiceProvider.GetService<ConfigurationRepository>();

        await configurationRepository.WriteDefaultAsync();
    }
    var configuration = null as Configuration;
    do
    {
        // Entity framework will not detect that a change has been made to the database
        // unless a new database context is used.
        using (var scope = app.Services.CreateScope())
        {
            var configurationRepository =
                scope.ServiceProvider.GetService<ConfigurationRepository>();
            configuration = await configurationRepository.GetNonEmptyAsync();
        }
        if (configuration is null)
        {
            Console.WriteLine(
                "Waiting for all configuration entries to be present in the database."
            );
            await Task.Delay(TimeSpan.FromMinutes(1));
        }
    } while (configuration is null);
    return configuration;
}

static async Task InitializeDatabaseAsync(WebApplication app, Configuration configuration)
{
    using var scope = app.Services.CreateScope();
    var databaseInitialiser = scope.ServiceProvider.GetService<DatabaseInitialiser>();
    var backgroundJobClient = scope.ServiceProvider.GetService<BackgroundJobClient>();
    var roundRepository = scope.ServiceProvider.GetService<RoundRepository>();
    var outcomeService = scope.ServiceProvider.GetService<OutcomeService>();

    await databaseInitialiser.Initialise(configuration);

    foreach (var round in await roundRepository.AllPlayableRoundsAsync())
    {
        backgroundJobClient.Schedule(
            () => outcomeService.WriteOutcomesAtEndOfRound(round),
            round.End - TimeSpan.FromMinutes(1)
        );
    }
}
