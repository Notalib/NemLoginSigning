using Nemlogin.QualifiedSigning.SDK.Core.Exceptions;
using Nemlogin.QualifiedSigning.SDK.Core.Fundamental;
using Nemlogin.QualifiedSigning.SDK.Core.Utilities;

namespace Nemlogin.QualifiedSigning.SDK.Core.Logic;

/// <summary>
/// Factory class for creating ITransformator implementations based on the SignatureFormat  
/// and SDFormat.
/// </summary>
public static class TransformatorFactory
{
    public static ITransformer Create(Transformation transformation)
    {
        var transformators = ReflectorLogic.GetClassesWithInterfaceType(typeof(ITransformer)).Where(c => !c.IsAbstract).ToList();

        foreach (var item in transformators)
        {
            var transformator = Activator.CreateInstance(item) as ITransformer;
            if (transformator.CanTransform(transformation))
            {
                return transformator;
            }
        }

        throw new TransformationException("Could not create transformator");
    }
}