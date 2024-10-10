using PdfSharp.Pdf;

namespace Nemlogin.QualifiedSigning.SDK.Pades.PdfValidation;

    public class PdfFontValidatorV2
    {
        /// <summary>
        /// Validation class for validating fonts.
        /// Only embedded fonts or standard fonts are allowed
        /// </summary>
        private static readonly List<string> StandardFonts =
            new List<string>()
            {
                "Helvetica",
                "Helvetica-Oblique",
                "Helvetica-Bold",
                "Helvetica-BoldOblique",
                "Times-Roman",
                "Times-Italic",
                "Times-Bold",
                "Times-BoldItalic",
                "Courier",
                "Courier-Oblique",
                "Courier-Bold",
                "Courier-BoldOblique",
                "Symbol",
                "ZapfDingbats"
            };

        public IEnumerable<PdfValidationResultV2> ValidateFonts(PdfPages pdfPages)
        {
            List<PdfValidationResultV2> pdfValidationResults = new();

            var scanResult = ScanForFonts(pdfPages);
            var validationResult = scanResult.Where(f => !f.Embedded && !IsStandardFont(f.FontName.ToString()));
            
            foreach (var item in validationResult)
            {
                // pdfValidationResults.Add(new PdfValidationResultV2(item.PdfObject, item.FontName, item.ObjectNumber));
            }

            return pdfValidationResults;
        }

        /// <summary>
        /// Method that will scan the PDF dictionaries for types Font or FontDescriptor
        /// </summary>
        /// <param name="pdfPages"></param>
        /// <returns></returns>
        public IEnumerable<PdfFontDescriptorV2> ScanForFonts(PdfPages pdfPages)
        {
            if (pdfPages == null)
                throw new ArgumentNullException(nameof(pdfPages));

            List<PdfFontDescriptorV2> fontDescriptors = new();

            foreach (var page in pdfPages)
            {
                
            }

            return fontDescriptors;
        }

        /// <summary>
        /// Returns if the given font is one of the standard 14 PDF fonts
        /// </summary>
        /// <param name="fontName"></param>
        /// <returns></returns>
        public static bool IsStandardFont(string fontName)
        {
            return !string.IsNullOrEmpty(fontName) && StandardFonts.Contains(fontName);
        }
    }