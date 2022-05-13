namespace StravaUploader
{
    internal class DeviceFoundArgs
    {
        public string Path { get; }

        internal DeviceFoundArgs(string path)
        {
            Path = path;
        }
    }
}
