using System.Collections.Generic;
using System.Text.RegularExpressions;
using iTextSharp.text.pdf;

namespace NemLoginSigningValidation.PDFValidation
{
    /// <summary>
    /// Whitelist validator used to validate PDF files against static resource of whitelist elements.
    /// </summary>
    public class PdfWhiteListValidator
    {
        public IEnumerable<PdfValidationResult> WhitelistValidation(PdfObject pdfObject, int objectNumber)
        {
            List<PdfValidationResult> validationResult = new List<PdfValidationResult>();

            if (pdfObject == null)
            {
                return validationResult;
            }

            switch (pdfObject.Type)
            {
                case PdfObject.NAME:
                    PdfName pdfName = (PdfName)pdfObject;

                    if (NotWhitelisted(pdfName))
                    {
                        validationResult.Add(new PdfValidationResult(pdfObject, pdfName, objectNumber));
                    }
                    
                    break;
                case PdfObject.DICTIONARY:

                    PdfDictionary pdfDictionary = (PdfDictionary)pdfObject;

                    foreach (var item in pdfDictionary)
                    {
                        string key = item.Key.ToString();

                        if (!PDFWhiteLists.Exclusions.Contains(key))
                        {
                            if (NotWhitelisted(item.Key))
                            {
                                validationResult.Add(new PdfValidationResult(pdfObject, item.Key, objectNumber));
                            }

                            if ((item.Value is PdfObject) && !PDFWhiteLists.Keys.Contains(key))
                            {
                                validationResult.AddRange(WhitelistValidation(item.Value, objectNumber));
                            }
                        }
                    }

                    break;
                case PdfObject.ARRAY:
                    PdfArray pdfArray = (PdfArray)pdfObject;

                    foreach (var item in pdfArray.ArrayList)
                    {
                        validationResult.AddRange(WhitelistValidation(item, objectNumber));
                    }

                    break;
            }

            return validationResult;
        }

        private bool NotWhitelisted(PdfName pdfName)
        {
            //Regex name-match for improved font support - matches /F1, /F2,...,/Fx
            string pattern = @"/F\d+";
            return !PDFWhiteLists.Names.Contains(pdfName.ToString()) && !Regex.Match(pdfName.ToString(), pattern).Success;
        }
    }
}