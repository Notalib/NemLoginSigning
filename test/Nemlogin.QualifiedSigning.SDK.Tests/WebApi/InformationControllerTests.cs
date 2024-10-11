using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Nemlogin.QualifiedSigning.Example.WebApi.Configuration;
using Nemlogin.QualifiedSigning.Example.WebApi.Controllers;
using Newtonsoft.Json;
using Xunit;

namespace Nemlogin.QualifiedSigning.SDK.Tests.WebApi;

public class InformationControllerTests
{
    [Fact]
    public void GetEntityIds_ValidCall_ReturnsOkResultWithEntityIds_List()
    {
        // Arrange
        var signingWebApiConfigurationMock = new Mock<IOptions<SigningWebApiConfiguration>>();
        var loggerMock = new Mock<ILogger<InformationController>>();

        var entityIds = new List<string> { "https://signsdk-demo.nemlog-in.dk" };
        var json = File.ReadAllText("appsettings.json");
        var signingWebApiConfiguration = JsonConvert.DeserializeObject<SigningWebApiConfiguration>(json);;

        signingWebApiConfigurationMock.Setup(m => m.Value).Returns(signingWebApiConfiguration);

        var controller = new InformationController(signingWebApiConfigurationMock.Object, loggerMock.Object);

        // Act
        var result = controller.GetEntityIds() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(200, result.StatusCode);
        Assert.IsAssignableFrom<List<string>>(result.Value);
        var returnedEntityIds = (List<string>)result.Value;
        Assert.Equal(entityIds.Count, returnedEntityIds.Count);
        foreach (var entityId in entityIds)
        {
            Assert.Contains(entityId, returnedEntityIds);
        }
    }

    [Fact]
    public void Constructor_WhenSigningWebApiConfigurationIsNull_Throws_Exception()
    {
        // Arrange
        IOptions<SigningWebApiConfiguration> signingWebApiConfiguration = null ;
        var loggerMock = new Mock<ILogger<InformationController>>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new InformationController(signingWebApiConfiguration, loggerMock.Object));
    }
}