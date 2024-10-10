using System;
using System.IO;

using Microsoft.Extensions.Logging;
using NemLoginSigningCore.Logging;

namespace NemLoginSigningCore.Utilities
{
    /// <summary>
    /// Class for writing a file to the specified path.
    /// Used to storing testfiles and for debugging purposes with the class library
    /// setting the "SaveDtbsToFolder" property in the Demo Web application.
    /// </summary>
    public static class FileWriter
    {
        public static void WriteFileToPath(string path, string name, string extension, byte[] fileData)
        {
            ILogger logger = LoggerCreator.CreateLogger(nameof(FileWriter));

            string fileNameAndPath = string.Empty;

            try
            {
                string rootDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                if (path == null)
                {
                    logger.LogError($"Cannot write file. Path is null");
                    return;
                }

                path = Path.Combine(rootDir, path);

                if (!Directory.Exists(path))
                {
                    logger.LogError("Cannot write file. Directory does not exist {Path}", path);
                }

                int number = new Random().Next(10000);

                string datetime = DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString().Replace(":", string.Empty);
                string fileName = $"{number}_{datetime}_{name}_{Guid.NewGuid()}.{extension}";

                fileNameAndPath = Path.Combine(path, fileName);

                File.WriteAllBytes(fileNameAndPath, fileData);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error writing file {Path}. {Message}", fileNameAndPath, e.Message);
            }
        }
    }
}
