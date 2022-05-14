using System.Text.Json.Serialization;

namespace StravaUploader.Models
{
    public class AuthTokenRefreshRequest : AuthTokenRequestBase
    {
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; }

        public AuthTokenRefreshRequest(Auth auth, string token) : base(auth, "refresh_token")
        {
            RefreshToken = token;
        }
    }
}
