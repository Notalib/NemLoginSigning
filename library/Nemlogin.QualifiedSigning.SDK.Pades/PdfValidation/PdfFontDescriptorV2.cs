
using PdfSharp.Pdf;
using Font = PdfSharp.Pdf.Advanced.PdfFont;
using FontDescriptor = PdfSharp.Pdf.Advanced.PdfFontDescriptor;

namespace Nemlogin.QualifiedSigning.SDK.Pades.PdfValidation;

    /// <summary>
    /// Class for holding information about fonts to be validated
    /// </summary>
    public class PdfFontDescriptorV2
    {
        public PdfItem PdfItem { get; set; }

        public PdfDictionary FontObject { get; set; }

        public PdfName FontName { get; set; }

        public bool Embedded { get; set; }

        public int ObjectNumber { get; set; }

        private PdfFontDescriptorV2(PdfItem pdfItem, PdfDictionary fontObject, PdfName fontName, bool embedded, int objectNumber)
        {
            PdfItem = pdfItem;
            FontObject = fontObject;
            FontName = fontName;
            Embedded = embedded;
            ObjectNumber = objectNumber;
        }

        public static PdfFontDescriptorV2 FromFontDescriptor(PdfItem pdfItem, PdfDictionary dict, int objectNumber)
        {
            ArgumentNullException.ThrowIfNull(dict);

            PdfFontDescriptorV2 result = null;
        
            var pdfObjectFontName = dict.FirstOrDefault(prop => prop.Key == FontDescriptor.Keys.FontName).Value;
            
            if (pdfObjectFontName != null)
            {
                PdfName pdfName = (PdfName)pdfObjectFontName;
                bool embedded = dict.Any(prop => prop.Key is FontDescriptor.Keys.FontFile or FontDescriptor.Keys.FontFile2 or FontDescriptor.Keys.FontFile3);
            
                result = new PdfFontDescriptorV2(pdfItem, dict, pdfName, embedded, objectNumber);
            }
        
            return result;
        }
        
        public static PdfFontDescriptorV2 FromFont(PdfItem pdfItem, PdfDictionary dict, int objectNumber)
        {
            var baseFontPdfObject = dict.FirstOrDefault(prop => prop.Key == Font.Keys.BaseFont).Value;
            PdfName baseFontPdfName = null;
        
            if (baseFontPdfObject != null)
            {
                baseFontPdfName = (PdfName)baseFontPdfObject;
            }
        
            string subType = null;
        
            var subTypePdfObject = dict.FirstOrDefault(prop => prop.Key == Font.Keys.Subtype).Value;
            if (subTypePdfObject is PdfName name)
            {
                subType = name.Value.ToUpper();
            }
        
            if (subType == "/TYPE0")
            {
                return new PdfFontDescriptorV2(pdfItem, dict, baseFontPdfName, true, objectNumber);
            }
        
            if (subType is "/TYPE1" or "/MMTYPE1" or "/TRUETYPE")
            {
                bool embedded = dict.FirstOrDefault(prop => prop.Key == Font.Keys.FontDescriptor).Value != null;
                return new PdfFontDescriptorV2(pdfItem, dict, baseFontPdfName, embedded, objectNumber);
        
            }
        
            if (subType is "/TYPE3" or "/CIDFONTTYPE0" or "/CIDFONTTYPE2")
            {
                return new PdfFontDescriptorV2(pdfItem, dict, baseFontPdfName, true, objectNumber);
            }
        
            return new PdfFontDescriptorV2(pdfItem, dict, baseFontPdfName, false, objectNumber);
        }
    }