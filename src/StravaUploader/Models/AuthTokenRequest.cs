using System.Text.Json.Serialization;

namespace StravaUploader.Models
{
    public class AuthTokenRequest : AuthTokenRequestBase
    {
        [JsonPropertyName("code")]
        public string ClientCode { get; }

        public AuthTokenRequest(Auth auth, string clientCode) : base(auth, "authorization_code")
        {
            ClientCode = clientCode;
        }
    }
}
