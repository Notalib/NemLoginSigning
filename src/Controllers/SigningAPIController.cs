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

using SignatureFormat = NemLoginSigningCore.Format.SignatureFormat;

namespace NemLoginSigningWebApp.Controllers
{
    [Route("")]
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
            _nemloginConfiguration = nemloginConfiguration.Value;
            _signatureKeysConfiguration = nemloginConfiguration.Value.SignatureKeysConfiguration;
            _signingValidationService = signingValidationService;
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

            SignatureKeys keys = new SignatureKeysLoader()
                .WithKeyStorePath(_signatureKeysConfiguration.KeystorePath)
                .WithKeyStorePassword(_signatureKeysConfiguration.KeyStorePassword)
                .WithPrivateKeyPassword(_signatureKeysConfiguration.PrivateKeyPassword)
                .LoadSignatureKeys();

            SignersDocument signersDocument = _signersDocumentLoader.CreateSignersDocumentFromSigningDocumentDTO(document);

            Language language = Enum.TryParse(request.Language, out Language lang) ? lang : Language.da;
            SignatureFormat format = Enum.TryParse(request.SignatureFormat, out SignatureFormat fmt) ? fmt : SignatureFormat.XAdES;

            var paramBuilder = new SignatureParameters.SignatureParametersBuilder()
                .WithFlowType(FlowType.ServiceProvider)
                .WithPreferredLanguage(language)
                .WithReferenceText(request.ReferenceText)
                .WithSignersDocumentFormat(signersDocument.DocumentFormat)
                .WithSignatureFormat(format)
                .WithEntityID(_nemloginConfiguration.EntityID)
                .WithMinAge(18); // Must be of legal age, for signature to be valid.

            if (!String.IsNullOrWhiteSpace(request.RequiredSigner))
            {
                paramBuilder.WithSignerSubjectNameID(request.RequiredSigner);
            }

            SignatureParameters parameters = paramBuilder.Build();

            SigningPayloadDTO payload = _documentSigningService.GenerateSigningPayload(signersDocument, parameters, format, keys);

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
        public async Task<IActionResult> Validate(SigningDocumentDTO signedDocument)
        {
            SignatureValidationContext ctx = new SignatureValidationContext.SignatureValidationContextBuilder()
                .WithDocumentName(signedDocument.FileName)
                .WithDocumentData(Convert.FromBase64String(signedDocument.EncodedContent))
                .WithValidationServiceUrl(_nemloginConfiguration.ValidationServiceURL)
                .Build();

            ValidationReport validationReport = await _signingValidationService.Validate(ctx);

            return Ok(validationReport);
        }

        [HttpGet]
        [Route("Ping")]
        public IActionResult Ping()
        {
            return Ok(new { pong = DateTime.UtcNow });
        }
    }
}
