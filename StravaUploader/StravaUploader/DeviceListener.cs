using System.Management;
using Microsoft.Extensions.Options;
using System.Runtime.Versioning;

namespace StravaUploader
{
    [SupportedOSPlatform("windows")]
    internal class DeviceListener
    {
        public event EventHandler<DeviceFoundArgs>? DeviceFound;

        private const string deviceQuery = "SELECT * FROM Win32_VolumeChangeEvent WHERE EventType = 2";
        private readonly ManagementEventWatcher _watcher;
        private readonly Config _config;

        internal DeviceListener(IOptions<Config> options)
        {
            _config = options.Value;
            _watcher = new();
            WqlEventQuery query = new(deviceQuery);
            _watcher.EventArrived += new(DeviceAttached);
            _watcher.Query = query;
            _watcher.Start();
            _watcher.WaitForNextEvent();
        }

        public void DeviceAttached(object sender, EventArrivedEventArgs e)
        {
            var info = DriveInfo.GetDrives();

            foreach (var drive in info)
            {
                if (drive.Name.StartsWith(e.NewEvent.Properties["DriveName"].Value.ToString() ?? string.Empty) && drive.VolumeLabel == _config.Device.Name)
                {
                    OnDeviceFound(new DeviceFoundArgs(drive.RootDirectory.FullName));
                }
            }
        }

        protected virtual void OnDeviceFound(DeviceFoundArgs e)
        {
            DeviceFound?.Invoke(this, e);
        }
    }
}
