using System;
using System.Collections.Generic;
using System.Linq;

using iTextSharp.text.pdf;

namespace NemLoginSigningValidation.PDFValidation
{
    public class PdfFontValidator
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

        public IEnumerable<PdfValidationResult> ValidateFonts(IEnumerable<PdfObject> pdfObjects)
        {
            List<PdfValidationResult> pdfValidationResults = new List<PdfValidationResult>();

            var scanResult = ScanForFonts(pdfObjects);
            var validationResult = scanResult.Where(f => !f.Embedded && !IsStandardFont(f.FontName.DecodeName()));

            foreach (var item in validationResult)
            {
                pdfValidationResults.Add(new PdfValidationResult(item.PdfObject, item.FontName, item.ObjectNumber));
            }

            return pdfValidationResults;
        }

        /// <summary>
        /// Method that will scan the PDF dictionaries for types Font or FontDescriptor
        /// </summary>
        /// <param name="pdfObjects"></param>
        /// <returns></returns>
        public IEnumerable<PdfFontDescriptor> ScanForFonts(IEnumerable<PdfObject> pdfObjects)
        {
            if (pdfObjects == null)
            {
                throw new ArgumentNullException(nameof(pdfObjects));
            }

            List<PdfFontDescriptor> fontDescriptors = new List<PdfFontDescriptor>();

            int i = 0;

            foreach (var pdfObject in pdfObjects)
            {
                if (pdfObject != null)
                {
                    if (pdfObject.IsDictionary())
                    {
                        PdfDictionary pdfDictionary = (PdfDictionary)pdfObject;
                        var type = pdfDictionary.Get(PdfName.TYPE);

                        if (type != null && type.IsName())
                        {
                            PdfName pdfName = (PdfName)type;

                            if (pdfName.DecodeName() == PdfName.FONT.DecodeName())
                            {
                                if (pdfDictionary.Get(PdfName.FONTDESCRIPTOR) == null)
                                {
                                    fontDescriptors.Add(PdfFontDescriptor.FromFont(pdfObject, pdfDictionary, i));
                                }
                            }
                            else if (pdfName.DecodeName() == PdfName.FONTDESCRIPTOR.DecodeName())
                            {
                                fontDescriptors.Add(PdfFontDescriptor.FromFontDescriptor(pdfObject, pdfDictionary, i));
                            }
                        }
                    }
                }

                i++;
            }

            return fontDescriptors;
        }

        /// <summary>
        /// Returns if the given font is one of the standard 14 PDF fonts
        /// </summary>
        /// <param name="fontName"></param>
        /// <returns></returns>
        public bool IsStandardFont(string fontName)
        {
            return !string.IsNullOrEmpty(fontName) && StandardFonts.Contains(fontName);
        }
    }
}