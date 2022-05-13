using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StravaUploader;

IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("config.json")
    .Build();

using IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = "Strava Upload Service";
    })
    .ConfigureServices(services =>
    {
        services.Configure<Config>(config);
        services.AddScoped<IAuthListener, AuthListener>();
        services.AddSingleton<DeviceListener>();
        services.AddHostedService<StravaBackgroundService>();
    })
    .Build();

await host.RunAsync();