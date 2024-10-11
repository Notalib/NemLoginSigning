using Nemlogin.QualifiedSigning.SDK.Core.Exceptions;
using Nemlogin.QualifiedSigning.SDK.Core.Fundamental;
using Nemlogin.QualifiedSigning.SDK.Core.Utilities;

namespace Nemlogin.QualifiedSigning.SDK.Core.Logic;

/// <summary>
/// Factory class for creating the Attacher logic class deciding if a document should 
/// be attached to the 
/// </summary>
public static class AttacherFactory
{
    public static ISourceAttacher Create(Transformation transformation)
    {
        IEnumerable<Type> candidates = ReflectorLogic.GetClassesWithInterfaceType(typeof(ISourceAttacher));

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