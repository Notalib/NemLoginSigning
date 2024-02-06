using System;

using iTextSharp.text.pdf;

namespace NemLoginSigningValidation.PDFValidation
{
    /// <summary>
    /// Class for holding information about fonts to be validated
    /// </summary>
    public class PdfFontDescriptor
    {
        public PdfObject PdfObject { get; set; }

        public PdfDictionary FontObject { get; set; }

        public PdfName FontName { get; set; }

        public bool Embedded { get; set; }

        public int ObjectNumber { get; set; }

        public PdfFontDescriptor(PdfObject pdfObject, PdfDictionary fontObject, PdfName fontName, bool embedded, int objectNumber)
        {
            PdfObject = pdfObject;
            FontObject = fontObject;
            FontName = fontName;
            Embedded = embedded;
            ObjectNumber = objectNumber;
        }

        public static PdfFontDescriptor FromFontDescriptor(PdfObject pdfObject, PdfDictionary dict, int objectNumber)
        {
            ArgumentNullException.ThrowIfNull(dict);

            PdfFontDescriptor result = null;

            PdfObject pdfObjectFontName = dict.Get(PdfName.FONTNAME);

            if (pdfObjectFontName.IsName())
            {
                PdfName pdfName = (PdfName)pdfObjectFontName;
                bool embedded = dict.Get(PdfName.FONTFILE) != null || dict.Get(PdfName.FONTFILE2) != null || dict.Get(PdfName.FONTFILE3) != null;

                result = new PdfFontDescriptor(pdfObject, dict, pdfName, embedded, objectNumber);
            }

            return result;
        }

        public static PdfFontDescriptor FromFont(PdfObject pdfObject, PdfDictionary dict, int objectNumber)
        {
            var baseFontPdfObject = dict.Get(PdfName.BASEFONT);
            PdfName baseFontPdfName = null;

            if (baseFontPdfObject != null && baseFontPdfObject.IsName())
            {
                baseFontPdfName = (PdfName)baseFontPdfObject;
            }

            string subType = null;

            PdfObject subTypePdfObject = dict.Get(PdfName.SUBTYPE);
            if (subTypePdfObject.IsName())
            {
                subType = PdfName.DecodeName(subTypePdfObject.ToString());
            }

            if (subType == PdfName.TYPE0.DecodeName())
            {
                return new PdfFontDescriptor(pdfObject, dict, baseFontPdfName, true, objectNumber);
            }

            if (subType == PdfName.TYPE1.DecodeName() || subType == PdfName.MMTYPE1.DecodeName() || subType == PdfName.TRUETYPE.DecodeName())
            {
                bool embedded = dict.Get(PdfName.FONTDESCRIPTOR) != null;
                return new PdfFontDescriptor(pdfObject, dict, baseFontPdfName, embedded, objectNumber);
            }

            if (subType == PdfName.TYPE3.DecodeName() || subType == PdfName.CIDFONTTYPE0.DecodeName() || subType == PdfName.CIDFONTTYPE2.DecodeName())
            {
                return new PdfFontDescriptor(pdfObject, dict, baseFontPdfName, true, objectNumber);
            }

            return new PdfFontDescriptor(pdfObject, dict, baseFontPdfName, false, objectNumber);
        }
    }
}