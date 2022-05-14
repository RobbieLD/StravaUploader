
namespace StravaUploader
{
    public interface IStrava
    {
        Task<long[]> UploadActivitiesAsync(string deviceRoot);
    }
}