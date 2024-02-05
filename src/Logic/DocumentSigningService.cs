using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NemLoginSigningCore.Configuration;
using NemLoginSigningCore.Core;
using NemLoginSigningCore.Format;
using NemLoginSigningDTO.Signing;
using NemLoginSigningService.Services;

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
        private readonly ITransformationPropertiesService _transformationPropertiesService;
        private readonly NemloginConfiguration _nemloginConfiguration;
        private readonly ILogger _logger;

        public DocumentSigningService(ISigningPayloadService signingPayloadService,
            ITransformationPropertiesService transformationPropertiesService,
            IOptions<NemloginConfiguration> nemloginConfiguration,
            ILogger<DocumentSigningService> logger)
        {
            if (nemloginConfiguration == null)
            {
                throw new ArgumentNullException(nameof(nemloginConfiguration));
            }

            _signingPayloadService = signingPayloadService ?? throw new ArgumentNullException(nameof(signingPayloadService));
            _transformationPropertiesService = transformationPropertiesService ?? throw new ArgumentNullException(nameof(transformationPropertiesService));
            _nemloginConfiguration = nemloginConfiguration.Value;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public SigningPayloadDTO GenerateSigningPayload(SignersDocument signersDocument, SignatureParameters signatureParameters, SignatureFormat signatureFormat, SignatureKeys signatureKeys)
        {
            TransformationProperties transformationProperties = _transformationPropertiesService.GetTransformationProperties(signersDocument, signatureFormat);

            TransformationContext ctx = new TransformationContext(signersDocument, signatureKeys, signatureParameters, transformationProperties);

            SigningPayload signingPayload = _signingPayloadService.ProduceSigningPayload(ctx);

            return new SigningPayloadDTO
            {
                SignatureParameters = signingPayload.SignatureParameters,
                Dtbs = Convert.ToBase64String(signingPayload.DataToBeSigned.GetData()),
            };
        }
    }
}