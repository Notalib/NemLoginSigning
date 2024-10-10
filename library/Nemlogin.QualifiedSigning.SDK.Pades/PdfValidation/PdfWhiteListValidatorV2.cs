using System.Text.RegularExpressions;
using PdfSharp.Pdf;
using PdfSharp.Pdf.Advanced;
using PdfArray = PdfSharp.Pdf.PdfArray;
using PdfDictionary = PdfSharp.Pdf.PdfDictionary;
using PdfName = PdfSharp.Pdf.PdfName;
using Font = PdfSharp.Pdf.Advanced.PdfFont;

namespace Nemlogin.QualifiedSigning.SDK.Pades.PdfValidation;

    /// <summary>
    /// Whitelist validator used to validate PDF files against static resource of whitelist elements.
    /// </summary>
    public class PdfWhiteListValidatorV2
    {
        public IEnumerable<PdfValidationResultV2> WhitelistValidation(KeyValuePair<string,PdfItem> pdfItem, int objectNumber)
        {
            List<PdfValidationResultV2> validationResult = new();

            
            ParsePdfItemKey(pdfItem, objectNumber, ref validationResult);

            switch (pdfItem.Value)
            {
                case PdfName pdfName:
                    ParsePdfName(pdfItem, pdfName, 1, ref validationResult);
                    break;

                case PdfReference pdfReference:
                    ParsePdfReference(pdfItem, pdfReference, 1, ref validationResult);
                    break;
                
                case PdfDictionary pdfDictionary:
                    ParsePdfDictionary(pdfItem, pdfDictionary, 1, ref validationResult);
                    break;
                
                case PdfArray pdfArray:
                    ParsePdfArray(pdfItem, pdfArray, 1, ref validationResult);
                    break;
            }

            return validationResult;
        }

        private void ParsePdfItemKey(KeyValuePair<string,PdfItem> pdfItem, int objectNumber, ref List<PdfValidationResultV2> validationResult)
        {
            if (pdfItem.Key != null)
            {
                if (NotWhitelisted(pdfItem.Key))
                {
                    validationResult.Add(new PdfValidationResultV2(pdfItem, string.Empty, objectNumber));
                }
            }

        }
        
        private void ParsePdfName(KeyValuePair<string,PdfItem> pdfItem, PdfName pdfName, int objectNumber, ref List<PdfValidationResultV2> validationResult)
        {
            if (NotWhitelisted(pdfName.Value))
            {
                validationResult.Add(new PdfValidationResultV2(pdfItem, pdfName.Value, objectNumber));
            }
        }
        
        private void ParsePdfReference(KeyValuePair<string, PdfItem> pdfItem, PdfReference pdfReference,
            int objectNumber, ref List<PdfValidationResultV2> validationResult)
        {
            if (pdfReference.Value is PdfDictionary dictionary)
            {
                ParsePdfDictionary(pdfItem, dictionary, objectNumber, ref validationResult);
            }
            else if(pdfReference.Value is PdfArray array)
            {
                ParsePdfArray(pdfItem, array, objectNumber, ref validationResult);
            }
        }


        private void ParsePdfDictionary(KeyValuePair<string,PdfItem> pdfItem, PdfDictionary pdfDictionary, int objectNumber, ref List<PdfValidationResultV2> validationResult)
        {
            foreach (var item in pdfDictionary)
            {
                if (item.Key is "/Font" or "/FontDescriptor")
                {
                    if (item.Value is PdfDictionary dic)
                    {
                        ParsePdfFont(pdfItem, dic, objectNumber, ref validationResult);
                    }
                    else if (item.Value is PdfReference reference)
                    {
                        ParsePdfFont(pdfItem, (PdfDictionary)reference.Value, objectNumber, ref validationResult);
                    }
                    
                }
                
                if (!PdfWhiteLists.Exclusions.Contains(item.Key))
                {
                    ParsePdfItemKey(item, objectNumber, ref validationResult);
                
                    if (item.Value is PdfName name)
                    {
                        ParsePdfName(pdfItem, name, objectNumber, ref validationResult);
                    }
                    else if (item.Value is PdfArray array)
                    {
                        ParsePdfArray(pdfItem, array, objectNumber, ref validationResult);
                    }
                    else if (item.Value is PdfDictionary dictionary)
                    {
                        ParsePdfDictionary(pdfItem, dictionary, objectNumber, ref validationResult);
                    }
                    else if (item.Value is PdfReference reference)
                    {
                        ParsePdfReference(pdfItem, reference, objectNumber, ref validationResult);
                    }

                }
            }
        }

        private void ParsePdfFont(KeyValuePair<string, PdfItem> pdfItem, PdfDictionary pdfFonts,
            int objectNumber, ref List<PdfValidationResultV2> validationResult)
        {
            List<PdfFontDescriptorV2> fontDescriptors = new();

            foreach (KeyValuePair<string,PdfItem?> font in pdfFonts)
            {
                var fontItems = (PdfReference)font.Value;

                if (fontItems.Value is PdfDictionary dictionary)
                {
                    foreach (KeyValuePair<string,PdfItem?> item in dictionary)
                    {
                        if (item.Key == "/FontDescriptor")
                        {
                            var fontDescriptor = (PdfReference)item.Value;
                            if (fontDescriptor.Value is PdfDictionary fontDescriptorDic)
                            {
                                fontDescriptors.Add(PdfFontDescriptorV2.FromFontDescriptor(pdfItem.Value, dictionary, objectNumber));                             
                            }
                        }
                        else if (item.Value.ToString() == "/Font")
                        {
                            fontDescriptors.Add(PdfFontDescriptorV2.FromFont(pdfItem.Value, dictionary, objectNumber));                             
                        }
                    }
                } 
            }
        }

        private void ParsePdfArray(KeyValuePair<string,PdfItem> pdfItem, PdfArray pdfArray, int objectNumber, ref List<PdfValidationResultV2> validationResult)
        {
            foreach (var item in pdfArray.Elements)
            {
                if (item is PdfName name)
                {
                    ParsePdfName(pdfItem, name, objectNumber, ref validationResult);
                }
            }

        }

        private bool NotWhitelisted(string pdfName)
        {
            List<string> patterns = PdfWhiteLists.NamesRegex;
            
            var isValid = true;
            foreach (string pattern in patterns)
            {
                isValid = isValid && Regex.Match(pdfName, pattern, RegexOptions.IgnoreCase).Success;
            }

            return !PdfWhiteLists.Names.Contains(pdfName) && isValid;
        }
    }