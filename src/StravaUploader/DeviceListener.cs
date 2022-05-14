﻿using System.Management;
using Microsoft.Extensions.Options;
using System.Runtime.Versioning;

namespace StravaUploader
{
    [SupportedOSPlatform("windows")]
    public class DeviceListener
    {
        public event EventHandler<DeviceFoundArgs>? DeviceFound;

        private const string deviceQuery = "SELECT * FROM Win32_VolumeChangeEvent WHERE EventType = 2";
        private readonly ManagementEventWatcher _watcher;
        private readonly Config _config;

        public DeviceListener(IOptions<Config> options)
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
                    DeviceFound?.Invoke(this, new DeviceFoundArgs(drive.RootDirectory.FullName));
                    break;
                }
            }
        }
    }
}
