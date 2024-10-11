using Nemlogin.QualifiedSigning.SDK.Core.Fundamental;

namespace Nemlogin.QualifiedSigning.SDK.Core.Logic;

/// <summary>
/// Interface defining implementations for how files are attached to source documents.
/// </summary>
public interface ISourceAttacher
{
    bool CanAttach(Transformation transformation);
    void Attach(TransformationContext ctx);
}