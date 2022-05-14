using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace StravaUploader
{
    public class StravaBackgroundService : BackgroundService
    {
        private readonly DeviceListener _listener;
        private readonly IStrava _strava;
        private readonly ILogger<StravaBackgroundService> _logger;

        public StravaBackgroundService(DeviceListener deviceListener, IStrava strava, ILogger<StravaBackgroundService> logger)
        {
            _logger = logger;
            _listener = deviceListener;
            _strava = strava;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                if (!stoppingToken.IsCancellationRequested)
                {
                    _listener.DeviceFound += async (s, e) =>
                    {
                        _logger.LogInformation("Device found at {Path}", e.Path);

                        try
                        {
                            var activitieIds = await _strava.UploadActivitiesAsync(e.Path);
                            _logger.LogInformation($"All files uploaded with activity ids {string.Join(',', activitieIds)}");
                            // TODO: Notify the user
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Unhandled exception caught");
                            // TODO: Notify the user
                        }
                        
                    };

                    await Task.CompletedTask;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception encountered when starting service");
                Environment.Exit(1);
            }
        }
    }
}
