using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Nemlogin.QualifiedSigning.Example.WebApi.Configuration;
using Nemlogin.QualifiedSigning.Example.WebApi.Model;
using Nemlogin.QualifiedSigning.SDK.Core.Services;
using Xunit;

namespace Nemlogin.QualifiedSigning.SDK.Tests.WebApi;

public class PrepareSigningServiceTests
{
    [Fact]
    public void PrepareSigningRequest_WhenPrepareSigningRequestDtoIsNull_ThrowsException()
    {
        // Arrange
        var signingPayloadServiceMock = new Mock<ISigningPayloadService>();
        var signingWebApiConfigurationMock = new Mock<IOptions<SigningWebApiConfiguration>>();
        var loggerMock = new Mock<ILogger<PrepareSigningService>>();

        var service = new PrepareSigningService(signingPayloadServiceMock.Object, signingWebApiConfigurationMock.Object, loggerMock.Object);

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => service.PrepareSigningRequest(null));
    }

    // You can add more tests to cover other scenarios as per your requirements
}