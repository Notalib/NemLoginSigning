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
            var candidates = ReflectorLogic.GetClassesWithInterfaceType(typeof(ISourceAttacher));

            foreach (var item in candidates)
            {
                var candidate = Activator.CreateInstance(item) as ISourceAttacher;
                if (candidate.CanAttach(transformation))
                {
                    return candidate;
                }
            }

            throw new TransformationException("Could not create attacher");
        }
    }
}