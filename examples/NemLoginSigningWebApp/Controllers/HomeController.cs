using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NemLoginSigningCore.Configuration;
using NemLoginSigningCore.Core;
using NemLoginSigningCore.Format;
using NemLoginSigningCore.Utilities;
using NemLoginSigningWebApp.Logic;
using NemLoginSigningWebApp.Models;

namespace NemLoginSigningWebApp.Controllers
{
    /// <summary>
    /// Controller for the NemLoginSigningWebApp containing all the Signing and functionality for the WebApp
    /// </summary>
    public class HomeController : Controller
    {
        private readonly string RESULT_TYPE_DOCUMENT_SIGNED = "signedDocument";
        private readonly string RESULT_TYPE_ERROR = "errorResponse";
        private readonly string RESULT_TYPE_CANCEL = "cancelSign";

        private readonly ILogger<HomeController> _logger;
        private readonly ISignersDocumentLoader _signersDocumentLoader;
        private readonly NemloginConfiguration _nemloginConfiguration;
        private readonly IDocumentSigningService _documentSigningService;
        private readonly ISigningResultService _signingResultService;

        private readonly SignatureKeysConfiguration _signatureKeysConfiguration;


        public HomeController(ILogger<HomeController> logger, 
            ISignersDocumentLoader signersDocumentLoader, 
            IDocumentSigningService documentSigningService,
            IOptions<NemloginConfiguration> nemloginConfiguration, 
            ISigningResultService signingResultService)
        {
            if (nemloginConfiguration == null)
            {
                throw new ArgumentNullException(nameof(nemloginConfiguration));
            }

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _signersDocumentLoader = signersDocumentLoader ?? throw new ArgumentNullException(nameof(signersDocumentLoader));
            _documentSigningService = documentSigningService ?? throw new ArgumentNullException(nameof(documentSigningService));
            _nemloginConfiguration = nemloginConfiguration.Value;
            _signingResultService = signingResultService ?? throw new ArgumentNullException(nameof(signingResultService));

            _signatureKeysConfiguration = _nemloginConfiguration.SignatureKeysConfiguration.First();
        }

        /// <summary>
        /// Index - Front page for the WebApp presenting the documents to sign
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            _logger.LogInformation("NemloginSigningWebApp - Index");

            IndexModel indexModel = new IndexModel();

            indexModel.Language = HttpContext.Request.Query["culture"].ToString();

            if (string.IsNullOrEmpty(indexModel.Language))
            {
                indexModel.Language = System.Globalization.CultureInfo.CurrentCulture.ToString();
            }

            indexModel.Files = _signersDocumentLoader.GetFiles();
            
            return View(indexModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Errror page for the WebApp - returning the ErrorViewModel
        /// </summary>
        /// <returns>ErrorViewModel</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /// <summary>
        /// Signing of a document - redirecting to the Sign.cshtml page
        /// </summary>
        /// <param name="format">Format to sign the document, either xml = XAdES or pdf = PAdES</param>
        /// <param name="filePath">Path to the document to sign</param>
        /// <param name="language">Language to pass to the signature parameters. Here taken from the chosen language from the Web Application</param>
        /// <returns>SigningModel</returns>
        public IActionResult Sign(string format, string filePath, string language)
        {
            _logger.LogInformation("NemloginSigningWebApp - Sign");

            SignatureFormat signatureFormat = SignatureFormat.XAdES;

            switch (format)
            {
                case "xml":
                    signatureFormat = SignatureFormat.XAdES;
                    break;
                case "pdf":
                    signatureFormat = SignatureFormat.PAdES;
                    break;
                default:
                    throw new ArgumentException("Signatureformat not set");
            }

            language = string.IsNullOrEmpty(language) ? "da" : language.Substring(0, 2);

            SignatureKeys signatureKeys = new SignatureKeysLoader()
                .WithKeyStorePath(_signatureKeysConfiguration.KeystorePath)
                .WithKeyStorePassword(_signatureKeysConfiguration.KeyStorePassword)
                .WithPrivateKeyPassword(_signatureKeysConfiguration.PrivateKeyPassword)
                .LoadSignatureKeys();

            var signingPayloadDTO = _documentSigningService.GenerateSigningPayload(signatureFormat, signatureKeys, language, Path.GetFileName(filePath), filePath);

            SigningModel signingModel = new SigningModel(signingPayloadDTO, _nemloginConfiguration.SigningClientURL, _signersDocumentLoader.CreateSignersDocumentFromFile(filePath), format);

            return View("Sign", signingModel);
        }

        /// <summary>
        /// When signing is done this is invoked where check for signingresult is done 
        /// and a redirect to either "Signcomplete" page or "SignError" based on the signing result.
        /// </summary>
        /// <param name="type">Result of the signing</param>
        /// <param name="name"></param>
        /// <param name="format"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public IActionResult SigningResult(string type, string name, string format, string result)
        {
            _logger.LogInformation("NemloginSigningWebApp - SigningResult");

            SigningResultModel signingResultModel = new SigningResultModel();

            signingResultModel.Name = name;
            signingResultModel.SignedDocumentFileName = _signingResultService.SignedDocumentFileName(name, format);

            if (type == RESULT_TYPE_DOCUMENT_SIGNED)
            {
                signingResultModel.Name = signingResultModel.SignedDocumentFileName;
                signingResultModel.MediaType = format;
                signingResultModel.SignedDocument = result;

                return View("SignComplete", signingResultModel);
            }

            if (type == RESULT_TYPE_CANCEL)
            {
                return View("SignCancel");
            }

            if (type == RESULT_TYPE_ERROR)
            {
                SignErrorModel signErrorModel = _signingResultService.ParseError(result);
                
                return View("SignError", signErrorModel);
            }

            throw new ArgumentException($"Invalid signing result type: {type}");
        }

        public async Task<IActionResult> ValidateSigningResult(string filename, string document)
        {
            _logger.LogInformation("NemloginSigningWebApp - ValidateSigningResult");

            try
            {
                var validationReport = await _signingResultService.ValidateSignedDocumentAsync(filename, document);

                if (validationReport == null)
                {
                    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
                }

                SigningResultModel signingResultModel = new SigningResultModel(validationReport);

                return View("SignComplete", signingResultModel);
            }
            catch (Exception)
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }
        public FileResult Download(string fileName, string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes($@"./wwwroot/Content/Files/{fileName}");
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }

            throw new FileNotFoundException("File not found");
        }

        public IActionResult Delete(string fileName, string filePath)
        {
            _logger.LogInformation("NemloginSigningWebApp - Delete");

            if (System.IO.File.Exists(filePath))
            {
                try
                {
                    System.IO.File.Delete(filePath);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            return View("Index", new IndexModel { Files = _signersDocumentLoader.GetFiles() });
        }

        [HttpPost("FileUpload")]
        public async Task<IActionResult> Upload(List<IFormFile> files)
        {
            _logger.LogInformation("NemloginSigningWebApp - Upload");

            if (files != null && files.Any())
            {
                string directory = ".\\wwwroot\\content\\UploadedFiles";

                if (files.Any())
                {
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                }

                long size = files.Sum(f => f.Length);

                foreach (var formFile in files)
                {
                    if (formFile.Length > 0)
                    {
                        var path = Path.Combine(directory, formFile.FileName);
                        var filePath = Path.GetTempFileName();

                        using (var stream = new FileStream(path, FileMode.Create))
                        {
                            await formFile.CopyToAsync(stream);
                        }
                    }
                }
            }

            return View("Index", new IndexModel { Files = _signersDocumentLoader.GetFiles() });
        }

        public async Task<string> IsAlive()
        {
            _logger.LogInformation("NemloginSigningWebApp - IsAlive");

            return await Task.FromResult("NemloginSigningWebApp is up and running.");
        }
    }
}