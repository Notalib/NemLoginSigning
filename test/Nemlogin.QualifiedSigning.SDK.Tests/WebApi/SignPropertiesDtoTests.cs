using System.Collections.Generic;
using Nemlogin.QualifiedSigning.Example.WebApi.Model;
using Nemlogin.QualifiedSigning.SDK.Core.Fundamental;
using Xunit;

namespace Nemlogin.QualifiedSigning.SDK.Tests.WebApi
{
    public class SignPropertiesDtoTests
    {
        [Fact]
        public void SignPropertiesDto_Constructor_Succeeds()
        {
            // Arrange
            Dictionary<string, SignPropertyValue> dictionary = new();
            dictionary.Add("Key1", new SignPropertyValue("Value1", SignPropertyValue.SignPropertyValueType.StringValue));
            var signProperties = new SignProperties(dictionary);

            // Act
            var signPropertiesDto = new SignPropertiesDto(signProperties);

            // Assert
            Assert.NotNull(signPropertiesDto);
        }


    }
}
