using System;
using System.Collections.Generic;
using System.Linq;

namespace NemLoginSigningCore.Utilities
{
    /// <summary>
    /// Utility class for reflection. Used for reflecting over types and returning matching implementations
    /// </summary>
    public static class ReflectorLogic
    {
        public static IEnumerable<Type> GetClassesWithInterfaceType(Type type)
        {
            List<Type> result = new List<Type>();

            // Get assemblies
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName.StartsWith("NemLogin", StringComparison.Ordinal)).ToList();

            // Get all classes that are assignable to the 'type' parameter (the interface)
            result = assemblies.SelectMany(a => a.GetTypes().Where(t => t.IsClass && !t.IsAbstract && type.IsAssignableFrom(t))).ToList();

            // Get all abstract classes that implements the 'type' parameter
            var assignableAbstracts = assemblies.SelectMany(a => a.GetTypes().Where(t => t.IsAbstract && !t.IsInterface && type.IsAssignableFrom(t)));

            foreach (var item in assignableAbstracts)
            {
                var outResult = assemblies.SelectMany(a => a.GetTypes().Where(t => t.IsSubclassOf(item)));
                result.AddRange(outResult);
            }

            return result.Distinct();
        }
    }
}
