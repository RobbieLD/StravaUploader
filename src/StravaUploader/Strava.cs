using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StravaUploader.Models;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Web;

namespace StravaUploader
{
    public class Strava : IStrava
    {
        private const string authUrl = "https://www.strava.com/oauth/token";
        private const string uploadUrl = "https://www.strava.com/api/v3/uploads";
        private const int totalAttempts = 3;
        private readonly Config _config;
        private readonly IKeyRepository _authRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IAuthListener _authListener;
        private readonly ILogger<Strava> _logger;

        public Strava(IOptions<Config> options, IKeyRepository repository, IHttpClientFactory httpClientFactory, IAuthListener authListener, ILogger<Strava> logger)
        {
            _config = options.Value;
            _authRepository = repository;
            _httpClientFactory = httpClientFactory;
            _authListener = authListener;
            _logger = logger;
        }

        public async Task<long[]> UploadActivitiesAsync(string deviceRoot)
        {
            AuthTokenResponse auth;

            if (_authRepository.Exists())
            {
                _logger.LogInformation("Reading auth from file");
                auth = await _authRepository.GetAsync();
            }
            else
            {
                auth = await GetAuthTokenAsync();
                _logger.LogInformation("Saving auth to file");
                await _authRepository.SetAsync(auth);
            }
                        
            if (auth.IsExpired)
            {
                _logger.LogInformation("Auth token has expired, getting a new one");
                auth = await GetRefreshTokenAsync(auth);
                await _authRepository.SetAsync(auth);
            }

            IEnumerable<string> files = Directory.EnumerateFiles(Path.Combine(deviceRoot, _config.Device.Path));
            return await Task.WhenAll(files.Select(path => UploadActivityAsync(path, auth.AccessToken)));
        }

        private async Task<long> UploadActivityAsync(string fileName, string token)
        {
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            using var stream = new FileStream(fileName, FileMode.Open);
            var form = new MultipartFormDataContent
            {
                { new StringContent(_config.Device.FileType), "data_type" },
                { new StreamContent(stream), "file", Path.GetFileName(fileName) }
            };
            
            var httpResponseMessage = await httpClient.PostAsync(uploadUrl, form);
            
            if (httpResponseMessage.IsSuccessStatusCode)
            {
                using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
                UploadResponse response = await JsonSerializer.DeserializeAsync<UploadResponse>(contentStream) ?? throw new("Upload response not valid");
                _logger.LogInformation("{fileName} has been uploaded with status: {Status}", fileName, response.Status);

                if (string.IsNullOrEmpty(response.Error))
                {
                    long activityId = await WaitForUploadAsync(response.Id, token);
                    _logger.LogInformation("{fileName} uploaded with activity id {activityId} and will now be deleted", fileName, activityId);
                    File.Delete(fileName);
                    return activityId;
                }
                else
                {
                    throw new($"Upload failed with error {response.Error}");
                }
            }
            else
            {
                throw new($"Upload activity failed with status code: {httpResponseMessage.StatusCode}");
            }
        }

        private async Task<long> WaitForUploadAsync(long id, string token)
        {
            int attempts = 1;
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            while (attempts <= totalAttempts)
            {
                var httpResponseMessage = await httpClient.GetAsync($"{uploadUrl}/{id}");

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
                    UploadResponse response = await JsonSerializer.DeserializeAsync<UploadResponse>(contentStream) ?? throw new("Status check response not valid");

                    if (response.Status == "Your activity is ready.")
                    {
                        return response.ActivityId ?? 0;
                    }
                    else if (!string.IsNullOrEmpty(response.Error))
                    {
                        throw new(response.Error);
                    }
                    else
                    {
                        attempts++;
                    }
                }
                else
                {
                    throw new($"Checking upload status failed with status: {httpResponseMessage.StatusCode}");
                }

                // Wait for 5 seconds and try again
                await Task.Delay(5000);
            }

            throw new($"Checking upload failed with max attempts ({totalAttempts}) reached");
        }

        private async Task<AuthTokenResponse> GetRefreshTokenAsync(AuthTokenResponse auth)
        {
            var body = new AuthTokenRefreshRequest(_config.Auth, auth.RefreshToken);
            return await MakeTokenRequestAsync(body);
        }

        private async Task<AuthTokenResponse> GetAuthTokenAsync()
        {
            string code = await GetAuthCodeAsync();
            var body = new AuthTokenRequest(_config.Auth, code);
            return await MakeTokenRequestAsync(body);            
        }

        private async Task<AuthTokenResponse> MakeTokenRequestAsync(object body)
        {
            var request = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, System.Net.Mime.MediaTypeNames.Application.Json);
            var httpClient = _httpClientFactory.CreateClient();
            var httpResponseMessage = await httpClient.PostAsync(authUrl, request);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
                AuthTokenResponse response = await JsonSerializer.DeserializeAsync<AuthTokenResponse>(contentStream) ?? throw new("AuthTokenResponse not valid");
                return response;
            }
            else
            {
                throw new($"GetAuthToken returned with non sucess status code: {httpResponseMessage.StatusCode}");
            }
        }

        private async Task<string> GetAuthCodeAsync()
        {
            string url = $"https://www.strava.com/oauth/authorize?client_id=" +
                $"{_config.Auth.ClientId}" +
                $"&redirect_uri={_config.Auth.CallBackUrl}" +
                $"&scope={HttpUtility.UrlEncode(string.Join(',', _config.Auth.Scope))}" +
                $"&response_type={_config.Auth.ResponseType}";

            _logger.LogInformation("Opening browser for user confirmation");

            Process.Start(new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = url
            });

            return await _authListener.GetAuthCodeAsync();
        }
    }
}
