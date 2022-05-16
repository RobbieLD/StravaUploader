namespace StravaUploader
{
    public interface IUpdateService
    {
        Task CheckForUpdates(Version version);
    }
}