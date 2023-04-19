using System.Collections.Generic;

namespace NemLoginSigningValidation.PDFValidation
{
    /// <summary>
    /// Static resources for whitelist validation of PDF documents
    /// </summary>
    public static class PDFWhiteLists
    {
        public static List<string> Exclusions => ResourceReader.GetRessource("NemLoginSigningValidation.PDFValidation.WhiteListRessources.whitelistexclusions.txt");

        public static List<string> Names => ResourceReader.GetRessource("NemLoginSigningValidation.PDFValidation.WhiteListRessources.whitelist.txt");

        public static List<string> Keys => ResourceReader.GetRessource("NemLoginSigningValidation.PDFValidation.WhiteListRessources.whitelistkeys.txt");

        public static List<string> NamesRegex => ResourceReader.GetRessource("NemLoginSigningValidation.PDFValidation.WhiteListRessources.whitelist-regex.txt");
    }
}
