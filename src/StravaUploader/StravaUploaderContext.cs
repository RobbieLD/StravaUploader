using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace StravaUploader
{
    public class StravaUploaderContext : ApplicationContext
    {
        private readonly NotifyIcon trayIcon; 
        private readonly DeviceListener _listener;
        private readonly IStrava _strava;
        private readonly INotificationService _notificationService;
        private readonly ILogger<StravaUploaderContext> _logger;
        private readonly string _logName, _configName;

        public StravaUploaderContext(ILogger<StravaUploaderContext> logger, IStrava strava, DeviceListener deviceListener, INotificationService notificationService, string logName, string configName)
        {
            _logger = logger;
            _listener = deviceListener;
            _strava = strava;
            _configName = configName;
            _logName = logName;
            _notificationService = notificationService;

            trayIcon = new NotifyIcon()
            {
                Icon = new Icon("app.ico"),
                ContextMenuStrip = new ContextMenuStrip()
                {
                    Items =
                    {
                        new ToolStripMenuItem("View Log", null, OpenLog),
                        new ToolStripMenuItem("Open Config", null, OpenConfig),
                        new ToolStripMenuItem("Exit", null, Exit)
                    }
                },
                Visible = true,
                Text = "Strava Uploader",
            };

            HookupEventListener();
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
