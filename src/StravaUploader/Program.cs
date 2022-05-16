using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace StravaUploader
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            var services = new ServiceCollection();
            ConfigureServices(services);
            using ServiceProvider serviceProvider = services.BuildServiceProvider();
            var ctx = serviceProvider.GetRequiredService<StravaUploaderContext>();
            Application.Run(ctx);
        }

        public static void ConfigureServices(ServiceCollection services)
        {
            string logName  = Path.Combine(Directory.GetCurrentDirectory(), "strava_uploader.log");
            string configName = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(logName)
                .CreateLogger();

            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile(configName)
                // For local development
                .AddJsonFile("appsettings.local.json", true)
                .Build();

            services.Configure<Config>(config);
            services.AddScoped<IAuthListener, AuthListener>();
            services.AddScoped<IKeyRepository, KeyRepository>();
            services.AddSingleton<INotificationService, NotificationService>();
            services.AddScoped<IStrava, Strava>();
            services.AddSingleton<DeviceListener>();
            services.AddScoped(a => ActivatorUtilities.CreateInstance<StravaUploaderContext>(a, logName, configName));
            services.AddHttpClient();
            services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
            services.AddScoped<IUpdateService, UpdateService>();
        }
    }
}