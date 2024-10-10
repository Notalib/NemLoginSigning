namespace Nemlogin.QualifiedSigning.SDK.Pades.PdfValidation.WhiteListResources;

/// <summary>
/// Helper class for reading resource-streams
/// </summary>
public static class ResourceReader
{
    public static List<string> GetResource(string resource)
    {
        List<string> resourceList = new();

        using StreamReader streamReader = new StreamReader(typeof(ResourceReader).Assembly.GetManifestResourceStream(resource)!);
        while (streamReader.ReadLine() is { } line)
        {
            resourceList.Add(line);
        }

        return resourceList;
    }
}