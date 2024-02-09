using NemLoginSigningCore.Exceptions;

namespace NemLoginSigningCore.Core
{
    /// <summary>
    /// Encapsulates the data involved in transforming
    /// an SD (Signer's Document) to a DTBS (Data To Be Signed) document.
    /// </summary>
    public class TransformationContext
    {
        /// <summary>
        /// Output Dtbs
        /// </summary>
        public DataToBeSigned DataToBeSigned { get; set; }

        /// <summary>
        /// Input SignersDocument
        /// </summary>
        public SignersDocument SignersDocument { get; private set; }

        /// <summary>
        /// Signature keys used to JWS-sign the SigningPayload
        /// </summary>
        public SignatureKeys SignatureKeys { get; private set; }

        /// <summary>
        /// Signature parameters - the dtbsDigest and dtbsSignedInfo field will be updated during the transformation
        /// </summary>
        public SignatureParameters SignatureParameters { get; private set; }

        public TransformationProperties TransformationProperties { get; private set; }

        public TransformationContext(SignersDocument signersDocument, SignatureKeys signatureKeys, SignatureParameters signatureParameters)
        {
            SignersDocument = signersDocument;
            SignatureKeys = signatureKeys;
            SignatureParameters = signatureParameters;

            if (TransformationProperties == null)
            {
                TransformationProperties = new TransformationProperties();
            }
        }

        public TransformationContext(SignersDocument signersDocument, SignatureKeys signatureKeys,
                SignatureParameters signatureParameters, TransformationProperties transformationProperties)
            : this(signersDocument, signatureKeys, signatureParameters)
        {
            TransformationProperties = transformationProperties;
        }

        public Transformation GetTransformation()
        {
            if (!SignatureParameters.DocumentFormat.HasValue || !SignatureParameters.SignatureFormat.HasValue)
            {
                string sdFormatValue = SignatureParameters.DocumentFormat.HasValue ? SignatureParameters.DocumentFormat.Value.ToString() : "null";
                string signatureFormatValue = SignatureParameters.SignatureFormat.HasValue ? SignatureParameters.SignatureFormat.Value.ToString() : "null";

                throw new InvalidSignatureParametersException($"No valid format for sdFormat={sdFormatValue}, signatureFormat={signatureFormatValue}");
            }

            return ValidTransformation.GetTransformation(SignatureParameters.DocumentFormat.Value, SignatureParameters.SignatureFormat.Value);
        }

        public void UpdateDtbsSignedInfo(string signatureEncoded)
        {
            SignatureParameters.DtbsSignedInfo = signatureEncoded;
        }
    }
}
