namespace StravaUploader
{
    public interface INotificationService
    {
        void Show(string message, string? url = null);
    }
}