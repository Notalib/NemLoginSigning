using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace NemLoginSigningValidation
{
    /// <summary>
    /// Helper class for reading resourcestreams
    /// </summary>
    public static class ResourceReader
    {
        public static List<string> GetRessource(string ressource)
        {
            List<string> ressourceList = new List<string>();

            using (var streamReader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream(ressource)))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    ressourceList.Add(line);
                }
            }

            return ressourceList;
        }
    }
}