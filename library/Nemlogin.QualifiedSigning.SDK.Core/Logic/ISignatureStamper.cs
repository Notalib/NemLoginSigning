using Nemlogin.QualifiedSigning.SDK.Core.Enums;
using Nemlogin.QualifiedSigning.SDK.Core.Fundamental;

namespace Nemlogin.QualifiedSigning.SDK.Core.Logic;

/// <summary>
/// Interface defining how Signature are stamped to either PAdES or XAdES documents.
/// </summary>
public interface ISignatureStamper
{
    bool CanSign(SignatureFormat signatureFormat);
    void PreSignDocument(TransformationContext ctx);
}