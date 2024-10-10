using NemLoginSigningCore.Core;
using NemLoginSigningCore.Format;

namespace NemLoginSigningWebApp.Logic;

public interface ITransformationPropertiesService
{
    TransformationProperties GetTransformationProperties(SignersDocument signersDocument, SignatureFormat signatureFormat);
}
