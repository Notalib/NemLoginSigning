using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using NemLoginSignatureValidationService.Model;
using NemLoginSignatureValidationService.Service;
using NemLoginSigningCore.Configuration;
using NemLoginSigningCore.Core;
using NemLoginSigningCore.DTO;
using NemLoginSigningCore.Utilities;
using NemLoginSigningWebApp.DTOs;
using NemLoginSigningWebApp.Logic;

namespace NemLoginSigningWebApp.Controllers
{
    [Route("Sign")]
    [ApiController]
    public class SigningAPIController : ControllerBase
    {
        private readonly IDocumentSigningService _documentSigningService;
        private readonly ISignersDocumentLoader _signersDocumentLoader;
        private readonly NemloginConfiguration _nemloginConfiguration;
        private readonly SignatureKeysConfiguration _signatureKeysConfiguration;
        private readonly ISigningValidationService _signingValidationService;

        public SigningAPIController(IDocumentSigningService documentSigningService, ISignersDocumentLoader signersDocumentLoader,
            ISigningValidationService signingValidationService, IOptions<NemloginConfiguration> nemloginConfiguration)
        {
            if (nemloginConfiguration == null)
            {
                throw new ArgumentNullException(nameof(nemloginConfiguration));
            }

            _documentSigningService = documentSigningService;
            _signersDocumentLoader = signersDocumentLoader;
            _signatureKeysConfiguration = _nemloginConfiguration.SignatureKeysConfiguration;
            _signingValidationService = signingValidationService;
            _nemloginConfiguration = nemloginConfiguration.Value;
        }

        [HttpPost]
        [Route("Payload")]
        public IActionResult GetSigningPayload(SigningRequestDTO request)
        {
            SigningDocumentDTO document = request?.Document;

            if (request is null || document is null || !document.Validate())
            {
                return BadRequest();
            }

            // 3.1.1: Signer’s Document Size Restriction
            // The SD must have a size of at most 20 MB.
            if (document.IsContentTooLarge)
            {
                return StatusCode(StatusCodes.Status413RequestEntityTooLarge);
            }

            string language = "da";

            SignatureKeys keys = new SignatureKeysLoader()
                .WithKeyStorePath(_signatureKeysConfiguration.KeystorePath)
                .WithKeyStorePassword(_signatureKeysConfiguration.KeyStorePassword)
                .WithPrivateKeyPassword(_signatureKeysConfiguration.PrivateKeyPassword)
                .LoadSignatureKeys();

            SignersDocument signersDocument = _signersDocumentLoader.CreateSignersDocumentFromSigningDocumentDTO(document);

            var paramBuilder = new SignatureParameters.SignatureParametersBuilder()
                .WithFlowType(FlowType.ServiceProvider)
                .WithPreferredLanguage(Enum.Parse<Language>(language))
                .WithReferenceText(document.FileName)
                .WithSignersDocumentFormat(signersDocument.DocumentFormat)
                .WithSignatureFormat(request.SignatureFormat)
                .WithEntityID(_nemloginConfiguration.EntityID)
                .WithMinAge(18); // Must be of legal age, for signature to be valid.

            if (!String.IsNullOrWhiteSpace(request.RequiredSigner))
            {
                paramBuilder.WithSignerSubjectNameID(request.RequiredSigner);
            }

            SignatureParameters parameters = paramBuilder.Build();

            SigningPayloadDTO payload = _documentSigningService.GenerateSigningPayload(signersDocument, parameters, request.SignatureFormat, keys);

            return Ok(payload);
        }

        [HttpGet]
        [Route("SigningClient")]
        public IActionResult GetSigningClient()
        {
            return Ok(_nemloginConfiguration.SigningClientURL);
        }

        [HttpPost]
        [Route("Validate")]
        public async Task<IActionResult> Validate(string signedDocumentFilename, string documentData)
        {
            SignatureValidationContext ctx = new SignatureValidationContext.SignatureValidationContextBuilder()
                .WithDocumentName(signedDocumentFilename)
                .WithDocumentData(Convert.FromBase64String(documentData))
                .WithValidationServiceUrl(_nemloginConfiguration.ValidationServiceURL)
                .Build();

            ValidationReport validationReport = await _signingValidationService.Validate(ctx);

            return Ok(validationReport);
        }
    }
}
