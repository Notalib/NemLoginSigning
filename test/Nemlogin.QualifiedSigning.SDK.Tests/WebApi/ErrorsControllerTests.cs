using System;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Nemlogin.QualifiedSigning.Example.WebApi.Controllers;
using Nemlogin.QualifiedSigning.Example.WebApi.Model;
using Nemlogin.QualifiedSigning.SDK.Core.Enums;
using Nemlogin.QualifiedSigning.SDK.Core.Exceptions;
using Xunit;

namespace Nemlogin.QualifiedSigning.SDK.Tests.WebApi;

public class ErrorsControllerTests
{
    [Fact]
    public void Error_WithApiErrorResponseForNemLoginException_ReturnsBadRequest()
    {
        // Arrange
        var controller = new ErrorsController();
        var exceptionHandlerFeatureMock = new Mock<IExceptionHandlerFeature>();
        var nemLoginException = new NemLoginException("NemLoginException occurred", ErrorCode.SDK001, new Exception("Error description"));

        exceptionHandlerFeatureMock.SetupGet(m => m.Error).Returns(nemLoginException);
        var httpContext = new DefaultHttpContext();
        httpContext.Features.Set(exceptionHandlerFeatureMock.Object);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        var result = controller.Error();

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        var apiErrorResponse = Assert.IsType<ApiErrorResponse>(badRequestResult.Value);
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.Equal("SDK001", apiErrorResponse.ErrorCode);
        Assert.Equal("Error loading SD", apiErrorResponse.ErrorMessage);
    }

    [Fact]
    public void Error_ForGenericException_ReturnsBadRequest()
    {
        // Arrange
        var controller = new ErrorsController();
        var exceptionHandlerFeatureMock = new Mock<IExceptionHandlerFeature>();
        var genericException = new System.Exception("Generic exception occurred");

        exceptionHandlerFeatureMock.SetupGet(m => m.Error).Returns(genericException);
        var httpContext = new DefaultHttpContext();
        httpContext.Features.Set(exceptionHandlerFeatureMock.Object);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        var result = controller.Error();

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.Equal(genericException, badRequestResult.Value);
    }
}