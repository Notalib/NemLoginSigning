using System;
using System.IO;
using Nemlogin.QualifiedSigning.Example.WebApi.Configuration;
using Newtonsoft.Json;
using Xunit;

namespace Nemlogin.QualifiedSigning.SDK.Tests.WebApi;

public class SigningWebApiConfigurationTests
{
    [Fact]
    public void GetSignatureKeys_WhenSignatureKeysIsNotLoaded_ThrowsException()
    {
        // Arrange
        var configuration = new SigningWebApiConfiguration();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => configuration.GetSignatureKeys("entityId", null));
    }

    [Fact]
    public void GetSignatureKeys_WhenNoSignatureKeysIsLoaded_Throws_Exception()
    {
        // Arrange
        var configuration = new SigningWebApiConfiguration();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => configuration.GetSignatureKeys("entityId", null));
    }

    [Fact]
    public void GetSignatureKeys_WhenInvalidEntityId_Throws_Exception()
    {
        // Arrange
        var json = File.ReadAllText("appsettings.json");
        var configuration = JsonConvert.DeserializeObject<SigningWebApiConfiguration>(json);;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => configuration.GetSignatureKeys("invalidEntityId", null));
    }

    [Fact]
    public void GetSignatureKeys_WhenEntityIdIsValid_ReturnsSignatureKeysFromCache()
    {
        // Arrange
        var json = File.ReadAllText("appsettings.json");
        var configuration = JsonConvert.DeserializeObject<SigningWebApiConfiguration>(json);;
        configuration.GetSignatureKeys("https://signsdk-demo.nemlog-in.dk", null); // Load signature keys

        // Act
        var result = configuration.GetSignatureKeys("https://signsdk-demo.nemlog-in.dk", null);

        // Assert
        Assert.NotNull(result);
    }
}