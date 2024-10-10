using System.Diagnostics;
using Nemlogin.QualifiedSigning.SDK.Core.DTO;
using Nemlogin.QualifiedSigning.SDK.Core.Enums;
using Nemlogin.QualifiedSigning.SDK.Core.Exceptions;
using Nemlogin.QualifiedSigning.SDK.Core.Fundamental;
using Nemlogin.QualifiedSigning.SDK.Core.Logic;
using Nemlogin.QualifiedSigning.SDK.Core.Utilities;
using Nemlogin.QualifiedSigning.SDK.Core.Validations;

namespace Nemlogin.QualifiedSigning.SDK.Core.Services;

    /// <summary>
    /// Entry class for using the SignSdk.
    /// Service Providers should call 'ProduceSigningPayloadDTO(TransformationContext)' to produce a SigningPayloadDTO
    /// suitable for passing on to the Signing Client as a JSON object.
    /// Brokers should call the 'ProduceSigningPayload(TransformationContext)' method 
    /// to produce a signing payload for further processing.
    /// </summary>
    public class SigningPayloadService : ISigningPayloadService
    {
        private readonly IValidationFactory _validationFactory;

        public SigningPayloadService(IValidationFactory validationFactory)
        {
            _validationFactory = validationFactory;
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

            // Validate SD
            _validationFactory.Create(ctx.SignersDocument.DocumentFormat).Validate(ctx);

            // Transform SD to DTBS
            TransformatorFactory.Create(ctx.GetTransformation()).Transform(ctx);

            // Attach source documents to DTBS
            AttacherFactory.Create(ctx.GetTransformation()).Attach(ctx);

            // Pre-sign DTBS
            SignatureStamperFactory.Create(ctx.DataToBeSigned.Format).PreSignDocument(ctx);

            // Compute Digest for DTBS - only used for SP flow
            UpdateDtbsDigest(ctx);

            // Validate the signature parameters
            ctx.SignatureParameters.Validate();

            // Sign the signature parameters
            string signedSignatureParameters = SignatureParameterSignerFactory.Create().Sign(ctx.SignatureParameters, ctx.SignatureKeys);

            // Return as SigningPayload
            SigningPayload signingPayload = new SigningPayload(signedSignatureParameters, ctx.DataToBeSigned);

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

            return new SigningPayloadDTO(signingPayload);
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