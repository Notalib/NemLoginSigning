using Nemlogin.QualifiedSigning.Common.Helpers;
using Xunit;

namespace Nemlogin.QualifiedSigning.SDK.Tests.Common
{
    public class SerializationExtensionsTests
    {
        public class TestEntity
        {
            public string Property1 { get; set; }
            public int Property2 { get; set; }
        }

        [Fact]
        public void ToJson_SerializesObjectToJson()
        {
            // Arrange
            var entity = new TestEntity { Property1 = "Value1", Property2 = 42 };
            var expectedJson = "{\"property1\":\"Value1\",\"property2\":42}";

            // Act
            var json = entity.ToJson();

            // Assert
            Assert.Equal(expectedJson, json);
        }
    }
}