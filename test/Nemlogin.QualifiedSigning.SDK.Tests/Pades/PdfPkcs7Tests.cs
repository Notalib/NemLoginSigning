// using System;
// using System.Collections.Generic;
// using Moq;
// using Nemlogin.QualifiedSigning.SDK.Pades.Logic;
// using Org.BouncyCastle.Crypto;
// using Org.BouncyCastle.Crypto.Parameters;
// using Org.BouncyCastle.Math;
// using Org.BouncyCastle.X509;
// using Xunit;
//
// namespace Nemlogin.QualifiedSigning.SDK.Tests
// {
//     public class PdfPkcs7Tests
//     {
//         
//         [Fact]
//         public void PdfPkcs7_ConstructsWithoutPrivateKey_ThrowsArgumentException()
//         {
//             // Arrange
//             var data = Convert.FromBase64String(
//                 "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");
//             X509Certificate cert = new X509Certificate(data);
//             ICollection<X509Certificate> certs = new List<X509Certificate>();
//             certs.Add(cert);
//             string hashAlgorithm = "SHA256";
//             bool hasRsaData = true;
//
//             // Act & Assert
//             Assert.Throws<ArgumentException>(() =>
//             {
//                 new PdfPkcs7(null, certs, hashAlgorithm, hasRsaData);
//             });
//         }
//
//         [Fact]
//         public void PdfPkcs7_ConstructsWithPrivateKeyAndCertificates_CreatesInstance()
//         {
//             // Arrange
//             var privateKeyMock = new Mock<ICipherParameters>();
//             // Wrapping ICipherParameters with a mockable wrapper
//             var wrapper = new CipherParametersWrapper(privateKeyMock.Object);
//
//             // Configure the mock to return an instance of RsaKeyParameters when accessed
//             privateKeyMock.Setup(p => p as RsaKeyParameters).Returns(() => wrapper.AsRsaKeyParameters());
//
//             var data = Convert.FromBase64String(
//                 "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");
//             X509Certificate cert = new X509Certificate(data);
//             ICollection<X509Certificate> certs = new List<X509Certificate>();
//             certs.Add(cert);
//             string hashAlgorithm = "SHA256";
//             bool hasRsaData = true;
//
//             // Act
//             var pdfPkcs7 = new PdfPkcs7(privateKeyMock.Object, certs, hashAlgorithm, hasRsaData);
//
//             // Assert
//             Assert.NotNull(pdfPkcs7);
//         }
//
//         [Fact]
//         public void GetHashAlgorithm_WithValidAlgorithm_ReturnsCorrectAlgorithm()
//         {
//             // Arrange
//             ICipherParameters privateKey = new RsaKeyParameters(true, new BigInteger("123456789"), new BigInteger("123456789"));
//             ICollection<X509Certificate> certs = new List<X509Certificate>();
//             string hashAlgorithm = "SHA256";
//             bool hasRsaData = true;
//             var pdfPkcs7 = new PdfPkcs7(privateKey, certs, hashAlgorithm, hasRsaData);
//
//             // Act
//             var result = pdfPkcs7.GetHashAlgorithm();
//
//             // Assert
//             Assert.Equal("SHA256", result);
//         }
//
//         [Fact]
//         public void GetEncryptionAlgorithm_WithValidAlgorithm_ReturnsCorrectAlgorithm()
//         {
//             // Arrange
//             ICipherParameters privateKey = new RsaKeyParameters(true, new BigInteger("123456789"), new BigInteger("123456789"));
//             ICollection<X509Certificate> certs = new List<X509Certificate>();
//             string hashAlgorithm = "SHA256";
//             bool hasRsaData = true;
//             var pdfPkcs7 = new PdfPkcs7(privateKey, certs, hashAlgorithm, hasRsaData);
//
//             // Act
//             var result = pdfPkcs7.GetEncryptionAlgorithm();
//
//             // Assert
//             Assert.Equal("RSA", result);
//         }
//
//         private RsaKeyParameters CreateRsaKeyParameters()
//         {
//             // Provide necessary parameters for RsaKeyParameters constructor
//             // For demonstration, we are providing dummy values
//             var modulus = new byte[] { 1, 2, 3 };
//             var exponent = new byte[] { 4, 5, 6 };
//
//             // Return an instance of RsaKeyParameters with the necessary parameters
//             return new RsaKeyParameters(true, new Org.BouncyCastle.Math.BigInteger(modulus), new Org.BouncyCastle.Math.BigInteger(exponent));
//         }
//     }
//     
//     // Wrapper class for ICipherParameters
//     public class CipherParametersWrapper
//     {
//         private readonly ICipherParameters _cipherParameters;
//
//         public CipherParametersWrapper(ICipherParameters cipherParameters)
//         {
//             _cipherParameters = cipherParameters;
//         }
//
//         // Method to cast ICipherParameters to RsaKeyParameters
//         public RsaKeyParameters AsRsaKeyParameters()
//         {
//             return _cipherParameters as RsaKeyParameters;
//         }
//     }
// }
