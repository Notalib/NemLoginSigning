using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Nemlogin.QualifiedSigning.Common.Services;
using Nemlogin.QualifiedSigning.Example.WebApp.Controllers;
using Nemlogin.QualifiedSigning.Example.WebApp.Logic;
using Nemlogin.QualifiedSigning.Example.WebApp.Models;
using Nemlogin.QualifiedSigning.SDK.Core.Configuration;
using Nemlogin.QualifiedSigning.SDK.Core.DTO;
using Nemlogin.QualifiedSigning.SDK.Core.Enums;
using Nemlogin.QualifiedSigning.SDK.Core.Fundamental;
using Nemlogin.QualifiedSigning.SDK.Core.Model;
using Xunit;

namespace Nemlogin.QualifiedSigning.SDK.Tests.WebApp
{
    public class HomeControllerTests
    {
        private readonly Mock<ILogger<HomeController>> _loggerMock;
        private readonly Mock<ISignersDocumentLoader> _signersDocumentLoaderMock;
        private readonly Mock<IDocumentSigningService> _documentSigningServiceMock;
        private readonly Mock<IOptions<NemloginConfiguration>> _nemloginConfigurationMock;
        private readonly Mock<ISigningResultService> _signingResultServiceMock;
        private readonly HomeController _homeController;

        public HomeControllerTests()
        {
            _loggerMock = new Mock<ILogger<HomeController>>();
            _signersDocumentLoaderMock = new Mock<ISignersDocumentLoader>();
            _documentSigningServiceMock = new Mock<IDocumentSigningService>();
            _nemloginConfigurationMock = new Mock<IOptions<NemloginConfiguration>>();
            _signingResultServiceMock = new Mock<ISigningResultService>();
            var config = TestHelper.GetConfiguration();
            
            _nemloginConfigurationMock.Setup(x => x.Value)
                .Returns(config);
            
            _homeController = new HomeController(
                _loggerMock.Object,
                _signersDocumentLoaderMock.Object,
                _documentSigningServiceMock.Object,
                _nemloginConfigurationMock.Object,
                _signingResultServiceMock.Object
            );
        }

        [Fact]
        public void Index_WithValidQueryString_ReturnsViewResult()
        {
            // Arrange
            _signersDocumentLoaderMock
                .Setup(loader => loader.GetFiles())
                .Returns(new List<SignersDocument>());
            _homeController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            _homeController.HttpContext.Request.QueryString = new QueryString("?culture=da");

            // Act
            var result = _homeController.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<IndexModel>(viewResult.Model);
        }
        
        [Fact]
        public void Index_WithEmptyQueryString_ReturnsViewResult()
        {
            // Arrange
            _signersDocumentLoaderMock
                .Setup(loader => loader.GetFiles())
                .Returns(new List<SignersDocument>());
            _homeController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            _homeController.HttpContext.Request.QueryString = new QueryString("?culture=");

            // Act
            var result = _homeController.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<IndexModel>(viewResult.Model);
        }

        [Fact]
        public void Sign_WithValidPdfFile_ReturnsViewResult()
        {
            // Arrange
            var filePath = "test.pdf";
            SignersDocument signersDocument = null;
            SignersDocumentFile signersDocumentFile = new SignersDocumentFile.SignersDocumentFileBuilder()
                .WithPath("./Resources/SignersDocuments/heiberg2002011324.pdf")
                .Build();
            signersDocument = new PdfSignersDocument(signersDocumentFile);

            _signersDocumentLoaderMock
                .Setup(loader => loader.CreateSignersDocumentFromFile(signersDocument.SignersDocumentFile.Path))
                .Returns(signersDocument);

            _documentSigningServiceMock
                .Setup(service => service.GenerateSigningPayload(
                    SignatureFormat.PAdES,
                    It.IsAny<SignatureKeys>(),
                    "da",
                    filePath,
                    filePath))
                .Returns(new SigningPayloadDTO());

            // Act
            var result = _homeController.Sign("pdf", filePath, "da");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<SigningModel>(viewResult.Model);
        }
        
        [Fact]
        public void Sign_WithValidXmlFile_ReturnsViewResult()
        {
            // Arrange
            var filePath = "test.xml";
            SignersDocument signersDocument = null;
            SignersDocumentFile signersDocumentFile = new SignersDocumentFile.SignersDocumentFileBuilder()
                .WithPath("./Resources/SignersDocuments/heiberg2002011324.xml")
                .Build();
            signersDocument = new PdfSignersDocument(signersDocumentFile);

            _signersDocumentLoaderMock
                .Setup(loader => loader.CreateSignersDocumentFromFile(signersDocument.SignersDocumentFile.Path))
                .Returns(signersDocument);

            _documentSigningServiceMock
                .Setup(service => service.GenerateSigningPayload(
                    SignatureFormat.XAdES,
                    It.IsAny<SignatureKeys>(),
                    "da",
                    filePath,
                    filePath))
                .Returns(new SigningPayloadDTO());

            // Act
            var result = _homeController.Sign("xml", filePath, "da");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<SigningModel>(viewResult.Model);
        }

        [Fact]
        public void Sign_WithInvalidFile_ReturnsArgumentException()
        {
            // Arrange
            var filePath = "test.txt";
            SignersDocument signersDocument = null;
            SignersDocumentFile signersDocumentFile = new SignersDocumentFile.SignersDocumentFileBuilder()
                .WithPath("./Resources/SignersDocuments/heiberg2002011324.txt")
                .Build();
            signersDocument = new PdfSignersDocument(signersDocumentFile);

            _signersDocumentLoaderMock
                .Setup(loader => loader.CreateSignersDocumentFromFile(signersDocument.SignersDocumentFile.Path))
                .Returns(signersDocument);

            _documentSigningServiceMock
                .Setup(service => service.GenerateSigningPayload(
                    SignatureFormat.XAdES,
                    It.IsAny<SignatureKeys>(),
                    "da",
                    filePath,
                    filePath))
                .Returns(new SigningPayloadDTO());

            // Assert
            Assert.Throws<ArgumentException>(() => _homeController.Sign("txt", filePath, "da"));
        }
        
        [Fact]
        public void Privacy_WithoutParameters_ReturnsViewResult()
        {
            // Act
            var result = _homeController.Privacy();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.Model);
        }
        
        [Fact]
        public void Error_WithoutParameters_ReturnsErrorViewModel()
        {
            // Arrange
            _homeController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            _homeController.HttpContext.TraceIdentifier = Guid.NewGuid().ToString();

            // Act
            var result = _homeController.Error();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<ErrorViewModel>(viewResult.Model);
        }

        [Fact]
        public void SigningResult_Returns_SignComplete_View()
        {
            // Arrange
            var expectedResult = "expected result";
            var name = "name";
            var format = "format";
            var result = "result";
            var type = "signedDocument";
            _signingResultServiceMock.Setup(s => s.SignedDocumentFileName(name, format)).Returns(expectedResult);

            // Act
            var resultView = _homeController.SigningResult(type, name, format, result) as ViewResult;

            // Assert
            Assert.NotNull(resultView);
            Assert.Equal("SignComplete", resultView.ViewName);
            Assert.Equal(expectedResult, ((SigningResultModel)resultView.Model).Name);
            Assert.Equal(format, ((SigningResultModel)resultView.Model).MediaType);
            Assert.Equal(result, ((SigningResultModel)resultView.Model).SignedDocument);
        }

        [Fact]
        public void SigningResult_Returns_SignCancel_View()
        {
            // Arrange
            var type = "cancelSign";

            // Act
            var result = _homeController.SigningResult(type, null, null, null) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("SignCancel", result.ViewName);
        }

        [Fact]
        public void SigningResult_Returns_SignError_View()
        {
            // Arrange
            var type = "errorResponse";
            var expectedResult = new SignErrorModel();
            _signingResultServiceMock.Setup(s => s.ParseError(It.IsAny<string>())).Returns(expectedResult);

            // Act
            var result = _homeController.SigningResult(type, null, null, "some error") as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("SignError", result.ViewName);
            Assert.Equal(expectedResult, result.Model);
        }

        [Fact]
        public void SigningResult_Throws_Exception_For_Invalid_Result_Type()
        {
            // Arrange
            var type = "invalid_type";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _homeController.SigningResult(type, null, null, null));
        }

        [Fact]
        public async Task ValidateSigningResult_View_ReturnsSignComplete()
        {
            // Arrange
            var filename = "filename";
            var document = "document";
            var validationReport = new ValidationReport(); // Assuming this is the type returned by ValidateSignedDocumentAsync
            _signingResultServiceMock.Setup(s => s.ValidateSignedDocumentAsync(filename, document)).ReturnsAsync(validationReport);

            // Act
            var result = await _homeController.ValidateSigningResult(filename, document) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("SignComplete", result.ViewName);
            Assert.Equal(filename, ((SigningResultModel)result.Model).Name);
            Assert.Equal(filename, ((SigningResultModel)result.Model).SignedDocumentFileName);
            Assert.Equal(document, ((SigningResultModel)result.Model).SignedDocument);
        }

        [Fact]
        public async Task ValidateSigningResult_ViewOnException_ReturnsError()
        {
            // Arrange
            var filename = "filename";
            var document = "document";
            _homeController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            _homeController.HttpContext.TraceIdentifier = Guid.NewGuid().ToString();
            _signingResultServiceMock.Setup(s => s.ValidateSignedDocumentAsync(filename, document)).ThrowsAsync(new Exception());

            // Act
            var result = await _homeController.ValidateSigningResult(filename, document) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Error", result.ViewName);
        }

        [Fact]
        public void Download_WhenFileExists_ReturnsFileResult()
        {
            // Arrange
            var fileName = "8.pdf";
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"wwwroot\content\Files", fileName); // Assuming the file exists in a "Files" folder

            // Act
            var result = _homeController.Download(fileName, filePath);

            // Assert
            var fileResult = Assert.IsType<FileContentResult>(result);
            Assert.Equal("application/octet-stream", fileResult.ContentType);
            Assert.Equal(fileName, fileResult.FileDownloadName);
        }

        [Fact]
        public void Download_WhenFileNotFound_ThrowsFileNotFoundException()
        {
            // Arrange
            var fileName = "nonexistent.txt";
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files", fileName); // Assuming the file does not exist

            // Act & Assert
            Assert.Throws<FileNotFoundException>(() => _homeController.Download(fileName, filePath));
        }
        
        [Fact]
        public void Delete_WhenFileExists_DeletesFileAndReturnsIndexView()
        {
            // Arrange
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"wwwroot\content\Files", "file.txt"); // Assuming the file exists
            File.Create(filePath).Dispose(); // Creating a dummy file for testing

            // Act
            var result = _homeController.Delete(filePath) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ViewName);
            Assert.NotNull(result.Model);
            _signersDocumentLoaderMock.Verify(m => m.GetFiles(), Times.Once);

            // Check if the file is deleted
            Assert.False(File.Exists(filePath));
        }

        [Fact]
        public void Delete_FileOutsideTargetDirectory_DoesNotDeleteAndReturnsIndexView()
        {
            // Arrange
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\SignersDocuments", "heiberg2002011324.pdf"); // Assuming the file exists
            var initialFilesCount = _signersDocumentLoaderMock.Object.GetFiles(); // Get initial files count

            // Act
            var result = _homeController.Delete(filePath) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ViewName);
            Assert.NotNull(result.Model);
            _signersDocumentLoaderMock.Verify(m => m.GetFiles(), Times.AtLeastOnce);

            // Check if no file was deleted
            Assert.Equal(initialFilesCount, _signersDocumentLoaderMock.Object.GetFiles());
        }

        [Fact]
        public void Delete_WhenFileNotFound_DoesNotThrowExceptionAndReturnsIndexView()
        {
            // Arrange
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"wwwroot\content\Files", "nonexistent.txt"); // Assuming the file does not exist

            // Act
            var result = _homeController.Delete(filePath) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ViewName);
            Assert.NotNull(result.Model);
        }
        
        [Fact]
        public async Task IsAlive_IfAlive_ReturnsExpectedResponse()
        {
            // Act
            var result = await _homeController.IsAlive();

            // Assert
            Assert.Equal("Nemlogin.QualifiedSigning.Example.WebApp is up and running.", result);
        }
        
        [Fact]
        public async Task Upload_WithValidFiles_UploadsFilesAndReturnsIndexView()
        {
            // Arrange
            var files = new List<IFormFile>
            {
                new FormFile(Stream.Null, 0, 10, "file1.txt", "file1.txt"),
                new FormFile(Stream.Null, 0, 20, "file2.txt", "file2.txt")
            };

            // Act
            var result = await _homeController.Upload(files) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ViewName);
            Assert.NotNull(result.Model);
            _signersDocumentLoaderMock.Verify(m => m.GetFiles(), Times.Once);

            // Check if files were uploaded
            var uploadedFiles = Directory.GetFiles(".\\wwwroot\\content\\UploadedFiles");
            Assert.Equal(2, uploadedFiles.Length); // Assuming 2 files were uploaded
            Assert.Contains(uploadedFiles, f => Path.GetFileName(f) == "file1.txt");
            Assert.Contains(uploadedFiles, f => Path.GetFileName(f) == "file2.txt");
            
            File.Delete(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"wwwroot\content\UploadedFiles", "file1.txt"));
            File.Delete(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"wwwroot\content\UploadedFiles", "file2.txt"));
        }

        [Fact]
        public async Task Upload_WhenFilesNull_DoesNotThrowExceptionAndReturnsIndexView()
        {
            // Act
            var result = await _homeController.Upload(null) as ViewResult;
            
            // Act & Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ViewName);
            Assert.NotNull(result.Model);
        }

        [Fact]
        public async Task Upload_WhenFilesEmpty_DoesNotThrowExceptionAndReturnsIndexView()
        {
            // Act
            var result = await _homeController.Upload(new List<IFormFile>()) as ViewResult;
            
            // Act & Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.ViewName);
            Assert.NotNull(result.Model);

        }
    }
}
