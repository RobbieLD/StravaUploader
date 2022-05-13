namespace StravaUploader
{
    internal class Config
    {
        public Auth Auth { get; set; } = new Auth();
        public Device Device { get; set; } = new Device();
    }

    internal class Auth
    {
        public string Code { get; set; } = string.Empty;
        public string CallBackUrl { get; set; } = string.Empty;
        public string[] Scope { get; set; } = Array.Empty<string>();
        public string ResponseType { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
    }

    internal class Device
    {
        public string Name { get; set; } = string.Empty;
    }
}
