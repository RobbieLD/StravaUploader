using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text;

namespace StravaUploader
{
    public class AuthListener : IAuthListener
    {
        private const string responseString = "<HTML><BODY>You are now authorized and can close this window</BODY></HTML>";
        private readonly HttpListener _listener;
        private readonly ILogger<AuthListener> _logger;

        public AuthListener(IOptions<Config> options, ILogger<AuthListener> logger)
        {
            if (string.IsNullOrEmpty(options.Value.Auth.CallBackUrl))
            {
                throw new($"No url prefix provided");
            }

            _listener = new();
            _listener.Prefixes.Add(options.Value.Auth.CallBackUrl);
            _logger = logger;
        }

        public async Task<string> GetAuthCodeAsync()
        {
            _listener.Start();
            HttpListenerContext context = await _listener.GetContextAsync();
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;
            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
            _listener.Stop();
            string? code = request.QueryString.Get("code");

            if (code == null)
            {
                throw new($"Auth code not found in response query");
            }

            _logger.LogInformation("Auth code received from call back url");

            return code;
        }
    }
}
