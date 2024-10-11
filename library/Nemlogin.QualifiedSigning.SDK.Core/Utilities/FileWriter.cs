using System.Globalization;

namespace Nemlogin.QualifiedSigning.SDK.Core.Utilities;

/// <summary>
/// Class for writing a file to the specified path.
/// Used to storing testfiles and for debugging purposes with the class library
/// setting the "SaveDtbsToFolder" property in the Demo Web application.
/// </summary>
public class FileWriter
{
    public static void WriteFileToPath(string path, string name, string extension, byte[] fileData)
    {
        string rootDir = Environment.CurrentDirectory;
        string targetPath = Path.Combine(rootDir, path);

        if (!Directory.Exists(targetPath))
            return;

        int number = new Random().Next(10000);
        string datetime = DateTime.UtcNow.ToString("yyyy-dd-M--HH-mm-ss.ffff", CultureInfo.InvariantCulture);
        string fileName = $"{number}_{datetime}_{name}_{Guid.NewGuid()}.{extension}";

        string fileNameAndPath = Path.Combine(targetPath, fileName);
        File.WriteAllBytes(fileNameAndPath, fileData);
    }
}