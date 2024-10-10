using System;
using Nemlogin.QualifiedSigning.SDK.Core.Enums;
using Nemlogin.QualifiedSigning.SDK.Core.Fundamental;
using Nemlogin.QualifiedSigning.SDK.Pades.Logic;
using Xunit;

namespace Nemlogin.QualifiedSigning.SDK.Tests.Pades
{
    public class PdfSourceAttacherTests
    {
        [Theory]
        [InlineData(DocumentFormat.XML, SignatureFormat.PAdES, ViewFormat.PDF)]
        public void CanAttach_ForXMLAndPAdES_ReturnsTrue(DocumentFormat sdFormat, SignatureFormat signatureFormat, ViewFormat viewFormat)
        {
            // Arrange
            var attacher = new PdfSourceAttacher();
            var transformation = new Transformation(sdFormat, signatureFormat, viewFormat);

            // Act
            bool canAttach = attacher.CanAttach(transformation);

            // Assert
            Assert.True(canAttach);
        }

        [Fact]
        public void CanAttach_ForNullTransformation_Returns_False()
        {
            // Arrange
            var attacher = new PdfSourceAttacher();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => attacher.CanAttach(null));
        }

        [Theory]
        [InlineData(DocumentFormat.PDF, SignatureFormat.PAdES, ViewFormat.PDF)]
        [InlineData(DocumentFormat.XML, SignatureFormat.XAdES, ViewFormat.TEXT)]
        public void CanAttach_ForNonXmlOrNonPAdES_ReturnsFalse(DocumentFormat sdFormat, SignatureFormat signatureFormat, ViewFormat viewFormat)
        {
            // Arrange
            var attacher = new PdfSourceAttacher();
            var transformation = new Transformation(sdFormat, signatureFormat, viewFormat);

            // Act
            bool canAttach = attacher.CanAttach(transformation);

            // Assert
            Assert.False(canAttach);
        }
    }
}