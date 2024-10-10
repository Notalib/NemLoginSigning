using Nemlogin.QualifiedSigning.SDK.Core.Enums;
using Nemlogin.QualifiedSigning.SDK.Core.Fundamental;

namespace NemLoginSigningWebApp.Logic;

public interface ITransformationPropertiesService
{
    TransformationProperties GetTransformationProperties(SignersDocument signersDocument, SignatureFormat signatureFormat);
}
