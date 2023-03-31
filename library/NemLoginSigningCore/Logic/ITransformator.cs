using NemLoginSigningCore.Core;
using Microsoft.Extensions.Logging;

namespace NemLoginSigningCore.Logic
{
    /// <summary>
    /// Interface for handling transformations between document formats.
    /// </summary>
    public interface ITransformator
    {
        bool CanTransform(Transformation transformation);
        void Transform(TransformationContext transformationContext, ILogger logger);
    }
}
