namespace Nemlogin.QualifiedSigning.SDK.Core.Utilities;

/// <summary>
/// Utility class for reflection. Used for reflecting over types and returning matching implementations
/// </summary>
public static class ReflectorLogic
{
    public static IEnumerable<Type> GetClassesWithInterfaceType(Type type)
    {
        List<Type> result = new List<Type>();

        // Get assemblies
        List<System.Reflection.Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName.StartsWith("Nemlogin", StringComparison.Ordinal)).ToList();

        // Get all classes that are assignable to the 'type' parameter (the interface)
        result = assemblies.SelectMany(a => a.GetTypes().Where(t => t.IsClass && !t.IsAbstract && type.IsAssignableFrom(t))).ToList();

        // Get all abstract classes that implements the 'type' parameter
        IEnumerable<Type> assignableAbstracts = assemblies.SelectMany(a => a.GetTypes().Where(t => t.IsAbstract && !t.IsInterface && type.IsAssignableFrom(t)));

        foreach (Type item in assignableAbstracts)
        {
            IEnumerable<Type> outResult = assemblies.SelectMany(a => a.GetTypes().Where(t => t.IsSubclassOf(item)));
            result.AddRange(outResult);
        }

        return result.Distinct();
    }
}