namespace StravaUploader
{
    public class Config
    {
        public Auth Auth { get; set; } = new Auth();
        public Device Device { get; set; } = new Device();
        public bool CheckForUpdates { get; set; }
    }

    public class Auth
    {
        public string CallBackUrl { get; set; } = string.Empty;
        public string[] Scope { get; set; } = Array.Empty<string>();
        public string ResponseType { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
    }

    public class Device
    {
        public string Name { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public bool CheckOnStart { get; set; }
    }
}
