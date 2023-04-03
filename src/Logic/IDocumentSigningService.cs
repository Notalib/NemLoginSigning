using NemLoginSigningCore.Core;
using NemLoginSigningCore.DTO;
using NemLoginSigningCore.Format;

namespace NemLoginSigningWebApp.Logic
{
    /// <summary>
    /// Interface defining the SigningService for the Web Application
    /// </summary>
    public interface IDocumentSigningService
    {
        public SigningPayloadDTO GenerateSigningPayload(SignersDocument signersDocument, SignatureParameters signatureParameters, SignatureFormat signatureFormat, SignatureKeys signatureKeys);
    }
}