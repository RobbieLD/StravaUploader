namespace StravaUploader
{
    public interface IAuthListener
    {
        Task<string> GetAuthCodeAsync();
    }
}
