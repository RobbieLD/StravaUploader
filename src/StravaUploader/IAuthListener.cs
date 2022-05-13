namespace StravaUploader
{
    internal interface IAuthListener
    {
        internal Task<string> GetAuthCode();
    }
}
