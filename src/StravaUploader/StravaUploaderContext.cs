using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Reflection;

namespace StravaUploader
{
    public class StravaUploaderContext : ApplicationContext
    {
        private readonly NotifyIcon trayIcon; 
        private readonly DeviceListener _listener;
        private readonly Config _config;
        private readonly IStrava _strava;
        private readonly INotificationService _notificationService;
        private readonly ILogger<StravaUploaderContext> _logger;
        private readonly string _logName, _configName;

        public StravaUploaderContext(
            ILogger<StravaUploaderContext> logger,
            IStrava strava,
            DeviceListener deviceListener,
            INotificationService notificationService,
            IOptions<Config> options,
            string logName,
            string configName)
        {
            _config = options.Value;
            _logger = logger;
            _listener = deviceListener;
            _strava = strava;
            _configName = configName;
            _logName = logName;
            _notificationService = notificationService;

            // Get the icon for the system tray
            Icon icon;
            var assembly = Assembly.GetExecutingAssembly(); ;
            string? version = assembly?.GetName()?.Version?.ToString();
            string resourcePath = assembly!.GetManifestResourceNames().Single(str => str.EndsWith("app.ico"));
            using (Stream stream = assembly.GetManifestResourceStream(resourcePath) ?? throw new("No icon found"))
            {
                icon = new(stream);
            }

            trayIcon = new NotifyIcon()
            {
                Icon = icon,
                ContextMenuStrip = new ContextMenuStrip()
                {
                    Items =
                    {
                        new ToolStripLabel($"Strava Uploader {version}"),
                        new ToolStripSeparator(),
                        new ToolStripMenuItem("View Log", null, OpenLog),
                        new ToolStripMenuItem("Open Config", null, OpenConfig),
                        new ToolStripMenuItem("Exit", null, Exit)
                    }
                },
                Visible = true,
                Text = "Strava Uploader",
            };

            HookupEventListener();

            if (_config.Device.CheckOnStart)
            {
                _listener.CheckForDevice();
            }
        }

        private void HookupEventListener()
        {
            _listener.DeviceFound += async (s, e) =>
            {
                _logger.LogInformation("Device found at {Path}", e.Path);

                try
                {
                    var activitieIds = await _strava.UploadActivitiesAsync(e.Path);
                    string message = $"All files uploaded with activity ids {string.Join(',', activitieIds)}";
                    _logger.LogInformation(message);
                    _notificationService.Show(message, "http://strava.com");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unhandled exception caught");
                    _notificationService.Show(ex.Message);
                }

            };
        }

        private void OpenLog(object? sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = _logName
            });
        }

        private void OpenConfig(object? sender, EventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = _configName
            });
        }

        private void Exit(object? sender, EventArgs e)
        {
            trayIcon.Visible = false;
            Application.Exit();
        }
    }
}
