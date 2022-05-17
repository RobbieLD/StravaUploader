using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using StravaUploader.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace StravaUploader.Tests
{
    public class UpdateServiceTests
    {
        [TestCase]
        public void CheckForUpdates_ThrowsException()
        {
            var logger = Mock.Of<ILogger<UpdateService>>();
            var mockNotificationService = new Mock<INotificationService>();
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound,
                });


            var client = new HttpClient(mockHttpMessageHandler.Object);
            mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(client);

            var updateService = new UpdateService(logger, mockHttpClientFactory.Object, mockNotificationService.Object);

            Assert.ThrowsAsync<Exception>(async () => await updateService.CheckForUpdates(It.IsAny<Version>()));
        }
        
        [TestCase]
        public async Task CheckForUpdates_ReturnsLatestVersion()
        {
            // Arrange
            var logger = Mock.Of<ILogger<UpdateService>>();
            var mockNotificationService = new Mock<INotificationService>();
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            string updateUrl = "http://localhost";

            var content = new GitHubReleasesResponse[]
            {
                new GitHubReleasesResponse
                {
                    Version = new Version("2.0.0.0"),
                    HtmlUrl = updateUrl
                }
            };

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(content)),
                });


            var client = new HttpClient(mockHttpMessageHandler.Object);
            mockHttpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(client);

            var updateService = new UpdateService(logger, mockHttpClientFactory.Object, mockNotificationService.Object);

            // Act
            var version = new Version("1.0.0.0");
            await updateService.CheckForUpdates(version);

            // Assert
            mockNotificationService.Verify(c => c.Show(It.IsAny<string>(), updateUrl), Times.Once);
        }
    }
}
