using iTextSharp.text.pdf;

namespace NemLoginSigningValidation
{
    /// <summary>
    /// Helper method for iTextSharp PdfName
    /// </summary>
    public static class PdfNameExtensions
    {
        public static string DecodeName(this PdfName pdfName)
        {
            return PdfName.DecodeName(pdfName.ToString());
        }
    }
}
