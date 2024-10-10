using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using Nemlogin.QualifiedSigning.SDK.Core.Exceptions;
using Nemlogin.QualifiedSigning.SDK.Core.Model;
using Nemlogin.QualifiedSigning.SDK.Core.Services;
using Xunit;

namespace Nemlogin.QualifiedSigning.SDK.Tests.Core
{
    public class SigningValidationServiceTests
    {
        [Fact]
        public async Task Validate_OnFailure_Throws_NemLoginException()
        {
            // Arrange
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var mockHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            var template = new SignatureValidationContext(null);
            
            var httpClient = new HttpClient(mockHandler.Object)
            {
                BaseAddress = new Uri("http://example.com")
            };
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            mockHttpClientFactory.Setup(x => x.CreateClient("ValidationServiceClient")).Returns(httpClient);

            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Throws(new HttpRequestException("Invalid request"));

            var service = new SigningValidationService(mockHttpClientFactory.Object);

            // Act & Assert
            await Assert.ThrowsAsync<NemLoginException>(() => service.Validate(template));
        }

        // Add more tests to cover other scenarios like timeout, unsuccessful response, etc.
    }
}
