using System;
using System.IO;
using Microsoft.Extensions.Options;
using NemLoginSigningService.Services;
using NemLoginSigningCore.Core;
using NemLoginSigningCore.DTO;
using NemLoginSigningCore.Format;
using static NemLoginSigningCore.Core.SignatureParameters;
using Microsoft.Extensions.Logging;
using NemLoginSigningCore.Utilities;
using NemLoginSigningCore.Configuration;
using System.Linq;

namespace NemLoginSigningWebApp.Logic
{
    /// <summary>
    /// Class responsible for handling the WebApp's Signing requests from the controller, 
    /// generating the Signature Parameters, the TransformationContext and passing 
    /// it to the SigningPayloadService.ProduceSigningPayload method.
    /// </summary>
    public class DocumentSigningService : IDocumentSigningService
    {
        private readonly ISigningPayloadService _signingPayloadService;
        private readonly ISignersDocumentLoader _signersDocumentLoader;
        private readonly ITransformationPropertiesService _transformationPropertiesService;
        private readonly NemloginConfiguration _nemloginConfiguration;
        private readonly ILogger _logger;

        public DocumentSigningService(ISigningPayloadService signingPayloadService,
            ISignersDocumentLoader signersDocumentLoader, 
            ITransformationPropertiesService transformationPropertiesService,
            IOptions<NemloginConfiguration> nemloginConfiguration,
            ILogger<DocumentSigningService> logger)
        {
            if (nemloginConfiguration == null)
            {
                throw new ArgumentNullException(nameof(nemloginConfiguration));
            }

            _signingPayloadService = signingPayloadService ?? throw new ArgumentNullException(nameof(signingPayloadService));
            _signersDocumentLoader = signersDocumentLoader ?? throw new ArgumentNullException(nameof(signersDocumentLoader));
            _transformationPropertiesService = transformationPropertiesService ?? throw new ArgumentNullException(nameof(transformationPropertiesService));
            _nemloginConfiguration = nemloginConfiguration.Value;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
    
        public SigningPayloadDTO GenerateSigningPayload(SignatureFormat signatureFormat, SignatureKeys signatureKeys, string language, string referenceText, string filePath)
        {
            var signersDocument = _signersDocumentLoader.CreateSignersDocumentFromFile(filePath);

            SignatureParameters signatureParameters = new SignatureParametersBuilder()
            .WithFlowType(FlowType.ServiceProvider)
            .WithPreferredLanguage(Enum.Parse<Language>(language))
            .WithReferenceText(referenceText)
            .WithSignersDocumentFormat(signersDocument.DocumentFormat)
            .WithSignatureFormat(signatureFormat)
            .WithEntityID(_nemloginConfiguration.SignatureKeysConfiguration.Single().EntityID)
            .Build();

            TransformationProperties transformationProperties = _transformationPropertiesService.GetTransformationProperties(signersDocument, signatureFormat);

            TransformationContext ctx = new TransformationContext(signersDocument, signatureKeys, signatureParameters, transformationProperties);

            var signingPayload = _signingPayloadService.ProduceSigningPayload(ctx);

            SigningPayloadDTO signingPayloadDTO = new SigningPayloadDTO(signingPayload);

            // If "SaveDtbsToFolder" property is set in configuration we will save the signed file to 
            // the specified folder.
            if (!string.IsNullOrEmpty(_nemloginConfiguration.SaveDtbsToFolder))
            {
                SaveFileToDtbsFolder(signingPayload);
            }

            return signingPayloadDTO;
        }

        /// <summary>
        /// Saves the signed file to the specified folder. If the path is empty or does not exist 
        /// the method does nothing.
        /// </summary>
        /// <param name="signingPayload"></param>
        private void SaveFileToDtbsFolder(SigningPayload signingPayload)
        {
            string relativePath = _nemloginConfiguration.SaveDtbsToFolder;
            
            if (string.IsNullOrEmpty(relativePath))
            {
                return;
            }

            var path = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), relativePath);

            if (!Directory.Exists(path))
            {
                _logger.LogInformation($"Directory for saving dtbs does not exist: {_nemloginConfiguration.SaveDtbsToFolder}");
            }
            else
            {
                try
                {
                    string extension = signingPayload.DataToBeSigned.Format == SignatureFormat.PAdES ? "pdf" : "xml";
                    FileWriter.WriteFileToPath(relativePath, "ProduceSigningPayload_End", extension, signingPayload.DataToBeSigned.GetData());
                }
                catch (Exception e)
                {
                    _logger.LogInformation($"Could not write dtbs to folder: {_nemloginConfiguration.SaveDtbsToFolder} {e.Message}");
                }
            }                
        }
    }
}