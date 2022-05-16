using Microsoft.Extensions.Logging;
using StravaUploader.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace StravaUploader
{
    public class UpdateService : IUpdateService
    {
        private const string releaseUrl = "https://api.github.com/repos/RobbieLD/StravaUploader/releases";
        private readonly ILogger<UpdateService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly INotificationService _notificationService;

        public UpdateService(ILogger<UpdateService> logger, IHttpClientFactory httpClientFactory, INotificationService notificationService)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _notificationService = notificationService;
        }

        public async Task CheckForUpdates(Version version)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.UserAgent.TryParseAdd("StravaUploader");

            var resposne = await client.GetAsync(releaseUrl);

            if (resposne.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    Converters =
                    {
                        new VersionConverter()
                    }
                };

                using var contentStream = await resposne.Content.ReadAsStreamAsync();
                var response = await JsonSerializer.DeserializeAsync<GitHubReleasesResponse[]>(contentStream, options) ?? throw new("Released response not valid");
                var latest = response.OrderByDescending(r => r.Version).First();
                if (latest.Version > version)
                {
                    _notificationService.Show($"This version ({version}) is out of date and a new version ({latest.Version}) is avaliable", latest.HtmlUrl);
                };
            }
            else
            {
                throw new($"Checking for updates returned status code: {resposne.StatusCode}");
            }
        }
    }
}
