using System;

using NemLoginSigningCore.Core;
using NemLoginSigningCore.Exceptions;
using NemLoginSigningCore.Utilities;

namespace NemLoginSigningCore.Logic
{
    /// <summary>
    /// Factory class for creating the Attacher logic class deciding if a document should
    /// be attached to the
    /// </summary>
    public static class AttacherFactory
    {
        public static ISourceAttacher Create(Transformation transformation)
        {
            System.Collections.Generic.IEnumerable<Type> candidates = ReflectorLogic.GetClassesWithInterfaceType(typeof(ISourceAttacher));

            foreach (Type item in candidates)
            {
                ISourceAttacher candidate = Activator.CreateInstance(item) as ISourceAttacher;
                if (candidate.CanAttach(transformation))
                {
                    return candidate;
                }
            }

            throw new TransformationException("Could not create attacher");
        }
    }
}
