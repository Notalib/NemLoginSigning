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

using static NemLoginSigningCore.Core.SignatureParameters;
using SignatureFormat = NemLoginSigningCore.Format.SignatureFormat;

namespace NemLoginSigningWebApp.Controllers
{
    [Route("Sign")]
    [ApiController]
    public class SignAPIController : ControllerBase
    {
        private readonly IDocumentSigningService _documentSigningService;
        private readonly ISignersDocumentLoader _signersDocumentLoader;
        private readonly NemloginConfiguration _nemloginConfiguration;
        private readonly SignatureKeysConfiguration _signatureKeysConfiguration;
        private readonly ISigningValidationService _signingValidationService;

        public SignAPIController(IDocumentSigningService documentSigningService, ISignersDocumentLoader signersDocumentLoader,
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
        public IActionResult GetSigningPayload(SignFileDTO fileToBeSigned, SignatureFormat format = SignatureFormat.XAdES)
        {
            if (fileToBeSigned?.ContentsBase64 is null || !fileToBeSigned.Validate())
            {
                return BadRequest();
            }

            // 3.1.1: Signer’s Document Size Restriction
            // The SD must have a size of at most 20 MB.
            if (fileToBeSigned.IsContentTooLarge)
            {
                return StatusCode(StatusCodes.Status413RequestEntityTooLarge);
            }

            string language = "da";

            SignatureKeys keys = new SignatureKeysLoader()
                .WithKeyStorePath(_signatureKeysConfiguration.KeystorePath)
                .WithKeyStorePassword(_signatureKeysConfiguration.KeyStorePassword)
                .WithPrivateKeyPassword(_signatureKeysConfiguration.PrivateKeyPassword)
                .LoadSignatureKeys();

            SignersDocument document = _signersDocumentLoader.CreateSignersDocumentFromSignFileDTO(fileToBeSigned);

            var paramBuilder = new SignatureParametersBuilder()
                .WithFlowType(FlowType.ServiceProvider)
                .WithPreferredLanguage(Enum.Parse<Language>(language))
                .WithReferenceText(fileToBeSigned.FileName)
                .WithSignersDocumentFormat(document.DocumentFormat)
                .WithSignatureFormat(format)
                .WithEntityID(_nemloginConfiguration.EntityID)
                .WithMinAge(18);

            if (!String.IsNullOrWhiteSpace(fileToBeSigned.RequiredSigner))
            {
                paramBuilder.WithSignerSubjectNameID(fileToBeSigned.RequiredSigner);
            }

            SignatureParameters parameters = paramBuilder.Build();

            SigningPayloadDTO payload = _documentSigningService.GenerateSigningPayload(document, parameters, format, keys);

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
