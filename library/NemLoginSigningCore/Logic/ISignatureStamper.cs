using NemLoginSigningCore.Core;
using NemLoginSigningCore.Format;

namespace NemLoginSigningCore.Logic
{
    /// <summary>
    /// Interface defining how Signature are stamped to either PAdES or XAdES documents.
    /// </summary>
    public interface ISignatureStamper
    {
        bool CanSign(SignatureFormat signatureFormat);

        void PresignDocument(TransformationContext context);
    }
}
