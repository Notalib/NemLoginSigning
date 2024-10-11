using Nemlogin.QualifiedSigning.SDK.Pades.PdfValidation.WhiteListResources;

namespace Nemlogin.QualifiedSigning.SDK.Pades.PdfValidation;

/// <summary>
/// Static resources for whitelist validation of PDF documents
/// </summary>
public static class PdfWhiteLists
{
    public static List<string> Exclusions => ResourceReader.GetResource("Nemlogin.QualifiedSigning.SDK.Pades.PdfValidation.WhiteListResources.whitelistexclusions.txt");
    public static List<string> Names => ResourceReader.GetResource("Nemlogin.QualifiedSigning.SDK.Pades.PdfValidation.WhiteListResources.whitelist.txt");
    public static List<string> Keys => ResourceReader.GetResource("Nemlogin.QualifiedSigning.SDK.Pades.PdfValidation.WhiteListResources.whitelistkeys.txt");
    public static List<string> NamesRegex => ResourceReader.GetResource("Nemlogin.QualifiedSigning.SDK.Pades.PdfValidation.WhiteListResources.whitelist-regex.txt");
}