using Microsoft.Extensions.Hosting;

namespace StravaUploader
{
    internal class StravaBackgroundService : BackgroundService
    {
        private readonly DeviceListener _listener;
        internal StravaBackgroundService(DeviceListener deviceListener)
        {
            _listener = deviceListener;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    _listener.DeviceFound += DeviceFound;
                }
            }
            catch (Exception ex)
            {
                // TODO: Log exception
                Environment.Exit(1);
            }
        }

        private void DeviceFound(object? sender, DeviceFoundArgs e)
        {
            // TODO: Make this work
            Console.WriteLine(e.Path);
        }
    }
}
