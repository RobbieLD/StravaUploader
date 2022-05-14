using System.Text.Json.Serialization;

namespace StravaUploader.Models
{
    public class UploadResponse
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("error")]
        public string Error { get; set; } = string.Empty;

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("activity_id")]
        public long? ActivityId { get; set; }
    }
}
