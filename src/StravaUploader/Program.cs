using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using StravaUploader;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("strava_uploader.log")
    .CreateLogger();

IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddJsonFile("appsettings.local.json", true)
    .Build();

using IHost host = Host.CreateDefaultBuilder(args)
    .UseSerilog()
    .UseWindowsService(options =>
    {
        options.ServiceName = "Strava Upload Service";
    })
    .ConfigureServices(services =>
    {
        
        services.Configure<Config>(config);
        services.AddScoped<IAuthListener, AuthListener>();
        services.AddScoped<IKeyRepository, KeyRepository>();
        services.AddScoped<IStrava, Strava>();
        services.AddSingleton<DeviceListener>();
        services.AddHostedService<StravaBackgroundService>();
        services.AddHttpClient();
    })
    .Build();

try
{
    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Service faield to start");
}
finally
{
    Log.CloseAndFlush();
}