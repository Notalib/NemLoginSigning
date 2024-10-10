using Nemlogin.QualifiedSigning.SDK.Core.Fundamental;

namespace Nemlogin.QualifiedSigning.SDK.Core.Logic;

/// <summary>
/// Interface for handling transformations between document formats.
/// </summary>
public interface ITransformer
{
    bool CanTransform(Transformation transformation);
    void Transform(TransformationContext transformationContext);
}