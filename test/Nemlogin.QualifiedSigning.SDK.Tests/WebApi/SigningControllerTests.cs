using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Nemlogin.QualifiedSigning.Example.WebApi.Controllers;
using Nemlogin.QualifiedSigning.Example.WebApi.Model;
using Nemlogin.QualifiedSigning.SDK.Core.DTO;
using Xunit;

namespace Nemlogin.QualifiedSigning.SDK.Tests.WebApi;

public class SigningControllerTests
{
    [Fact]
    public void Test_WithMessage_ReturnsOkResult()
    {
        // Arrange
        var prepareSigningServiceMock = new Mock<IPrepareSigningService>();
        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        var loggerMock = new Mock<ILogger<SigningController>>();

        var controller = new SigningController(prepareSigningServiceMock.Object, httpContextAccessorMock.Object, loggerMock.Object);

        // Act
        var result = controller.Test() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal("Hello SignSDK.Net WebAPI", result.Value);
    }

    [Fact]
    public void PrepareSigningPayload_WithSigningPayloadDto_ReturnsOkResult()
    {
        // Arrange
        var prepareSigningServiceMock = new Mock<IPrepareSigningService>();
        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        var loggerMock = new Mock<ILogger<SigningController>>();

        var prepareSigningRequestDto = new PrepareSigningRequestDto(); // Assuming PrepareSigningRequestDto is a simple DTO with public properties
        var expectedSigningPayloadDto = new SigningPayloadDTO(); // Assuming this is the expected result from the service
        prepareSigningServiceMock.Setup(m => m.PrepareSigningRequest(prepareSigningRequestDto)).Returns(expectedSigningPayloadDto);

        var controller = new SigningController(prepareSigningServiceMock.Object, httpContextAccessorMock.Object, loggerMock.Object);

        // Act
        var result = controller.PrepareSigningPayload(prepareSigningRequestDto) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.Equal(expectedSigningPayloadDto, result.Value);
    }

    [Fact]
    public void Constructor_WhenPropertiesAreNull_ThrowsException()
    {
        // Arrange
        IPrepareSigningService prepareSigningService = null;
        IHttpContextAccessor httpContextAccessor = null;
        ILogger<SigningController> logger = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new SigningController(prepareSigningService, httpContextAccessor, logger));
    }
}