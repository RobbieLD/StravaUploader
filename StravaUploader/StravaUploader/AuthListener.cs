using System.Net;
using System.Text;

namespace StravaUploader
{
    internal class AuthListener : IAuthListener
    {
        private const string responseString = "<HTML><BODY>You are now authorized and can close this window</BODY></HTML>";
        private readonly HttpListener _listener;

        internal AuthListener(string prefix)
        {
            if (string.IsNullOrEmpty(prefix))
            {
                throw new($"No url prefix provided");
            }

            _listener = new();
            _listener.Prefixes.Add(prefix);
        }

        public async Task<string> GetAuthCode()
        {
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

            return code;
        }
    }
}
