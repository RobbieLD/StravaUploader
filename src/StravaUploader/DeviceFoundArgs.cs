namespace StravaUploader
{
    public class DeviceFoundArgs
    {
        public string Path { get; }

        public DeviceFoundArgs(string path)
        {
            Path = path;
        }
    }
}
