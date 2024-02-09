using System;
using System.Linq;

using NemLoginSigningCore.Core;
using NemLoginSigningCore.Exceptions;
using NemLoginSigningCore.Utilities;

namespace NemLoginSigningCore.Logic
{
    /// <summary>
    /// Factory class for creating ITransformator implementations based on the SignatureFormat
    /// and SDFormat.
    /// </summary>
    public static class TransformatorFactory
    {
        public static ITransformator Create(Transformation transformation)
        {
            System.Collections.Generic.List<Type> transformators = ReflectorLogic.GetClassesWithInterfaceType(typeof(ITransformator)).Where(c => !c.IsAbstract).ToList();

            foreach (Type item in transformators)
            {
                ITransformator transformator = Activator.CreateInstance(item) as ITransformator;
                if (transformator.CanTransform(transformation))
                {
                    return transformator;
                }
            }

            throw new TransformationException("Could not create transformator");
        }
    }
}
