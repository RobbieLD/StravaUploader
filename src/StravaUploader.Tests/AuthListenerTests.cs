using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StravaUploader.Tests
{
    public class AuthListenerTests
    {
        [TestCase]
        public async Task ListensOnEndPoint()
        {
            var options = Options.Create(new Config
            {
                Auth = new Auth
                {
                    CallBackUrl = "http://localhost:1234/"
                }
            });

            var logger = Mock.Of<ILogger<AuthListener>>();


            var listener = new AuthListener(options, logger);

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                Thread.Sleep(2000);
                using var client = new HttpClient();
                client.GetAsync($"{options.Value.Auth.CallBackUrl}/?code=123").Wait();                
            }).Start();

            var code = await listener.GetAuthCodeAsync();

            Assert.AreEqual(code, "123");
        }
    }
}
