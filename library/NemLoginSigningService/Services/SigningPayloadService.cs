using System;
using System.Diagnostics;

using Microsoft.Extensions.Logging;
using NemLoginSigningCore.Core;
using NemLoginSigningCore.Exceptions;
using NemLoginSigningCore.Logic;
using NemLoginSigningCore.Utilities;
using NemLoginSigningDTO.Signing;
using NemLoginSigningValidation;

namespace NemLoginSigningService.Services
{
    /// <summary>
    /// Entry class for using the SignSdk.
    /// Service Providers should call 'ProduceSigningPayloadDTO(TransformationContext)' to produce a SigningPayloadDTO
    /// suitable for passing on to the Signing Client as a JSON object.
    /// Brokers should call the 'ProduceSigningPayload(TransformationContext)' method
    /// to produce a signing payload for further processing.
    /// </summary>
    public class SigningPayloadService : ISigningPayloadService
    {
        private readonly ILogger<SigningPayloadService> _logger;

        public SigningPayloadService(ILogger<SigningPayloadService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Creates SigningPayload based on TransformationContext which must contain the 'Signers Document', 'Signature Parameters' and
        /// 'Signature Keys' to use for signing the signature parameters.
        /// Brokers should call this method to produce a signing payload for further processing.
        /// The SigningPayload.SignatureParameters property is suitable for passing on to the
        /// 'begin-sign-flow' Signing API endpoint.
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns>SigningPayload</returns>
        public SigningPayload ProduceSigningPayload(TransformationContext ctx)
        {
            if (ctx == null)
            {
                throw new ArgumentNullException(nameof(ctx));
            }

            _logger.LogInformation("SignSdk - SigningPayloadService - ProduceSigningPayload");

            // Validate SD
            _logger.LogInformation("SignSdk - SigningPayloadService - Validate SD");
            ValidatorFactory.Create(ctx.SignersDocument.DocumentFormat).Validate(ctx);

            // Transform SD to DTBS
            _logger.LogInformation("SignSdk - SigningPayloadService - Transform");
            TransformatorFactory.Create(ctx.GetTransformation()).Transform(ctx, _logger);

            // Attach source documents to DTBS
            _logger.LogInformation("SignSdk - SigningPayloadService - Attach");
            AttacherFactory.Create(ctx.GetTransformation()).Attach(ctx);

            // Pre-sign DTBS
            _logger.LogInformation("SignSdk - SigningPayloadService - PreSign");
            SignatureStamperFactory.Create(ctx.DataToBeSigned.Format).PresignDocument(ctx);

            // Compute Digest for DTBS - only used for SP flow
            _logger.LogInformation("SignSdk - SigningPayloadService - Update DTBS");
            UpdateDtbsDigest(ctx);

            // Validate the signature parameters
            _logger.LogInformation("SignSdk - SigningPayloadService - Validate Signature Parameters");
            ctx.SignatureParameters.Validate();

            // Sign the signature parameters
            _logger.LogInformation("SignSdk - SigningPayloadService - Sign");
            string signedSignatureParameters = SignatureParameterSignerFactory.Create().Sign(ctx.SignatureParameters, ctx.SignatureKeys);

            // Return as SigningPayload
            _logger.LogInformation("SignSdk - SigningPayloadService - Create New Signing Payload");
            SigningPayload signingPayload = new SigningPayload(signedSignatureParameters, ctx.DataToBeSigned);

            _logger.LogInformation("SignSdk - SigningPayloadService - ProduceSigningPayload Done");

            return signingPayload;
        }

        /// <summary>
        /// Variant of the 'ProduceSigningPayload(TransformationContext)' that wraps the resulting
        /// signing payload as 'SigningPayloadDTO'.
        /// Service Providers should call this method to produce a signing payload suitable for passing on
        /// to the Signing Client as a JSON object.
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public SigningPayloadDTO ProduceSigningPayloadDTO(TransformationContext ctx)
        {
            SigningPayload signingPayload = ProduceSigningPayload(ctx);

            return new SigningPayloadDTO
            {
                SignatureParameters = signingPayload.SignatureParameters,
                Dtbs = Convert.ToBase64String(signingPayload.DataToBeSigned.GetData()),
            };
        }

        /// <summary>
        /// For SP flow only.
        /// Computes a digest for the entire DTBS document using the same algorithm as used for signing signature parameters
        /// and updates the dtbsDigest field of the signature parameters.
        /// </summary>
        /// <param name="ctx"></param>
        private void UpdateDtbsDigest(TransformationContext ctx)
        {
            if (ctx.SignatureParameters.FlowType == FlowType.ServiceProvider)
            {
                try
                {
                    byte[] digest = CryptographyLogic.ComputeSha256Hash(ctx.DataToBeSigned.GetData());
                    ctx.SignatureParameters.DtbsDigest = Convert.ToBase64String(digest);

                    Debug.WriteLine($"Dtbs Digest: {ctx.SignatureParameters.DtbsDigest}");
                }
                catch (Exception e)
                {
                    throw new TransformationException("Error computing digest for DTBS", ErrorCode.SDK006, e);
                }
            }
        }
    }
}