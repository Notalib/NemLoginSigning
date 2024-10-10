using Nemlogin.QualifiedSigning.SDK.Core.Enums;
using Nemlogin.QualifiedSigning.SDK.Core.Fundamental;

namespace Nemlogin.QualifiedSigning.Common.Services;

public interface ITransformationPropertiesService
{
    TransformationProperties GetTransformationProperties(SignersDocument signersDocument, SignatureFormat signatureFormat);
}