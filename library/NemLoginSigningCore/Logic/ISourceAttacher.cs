using NemLoginSigningCore.Core;

namespace NemLoginSigningCore.Logic
{
    /// <summary>
    /// Interface defining implementations for how files are attached to source documents.
    /// </summary>
    public interface ISourceAttacher
    {
        bool CanAttach(Transformation transformation);

        void Attach(TransformationContext ctx);
    }
}
