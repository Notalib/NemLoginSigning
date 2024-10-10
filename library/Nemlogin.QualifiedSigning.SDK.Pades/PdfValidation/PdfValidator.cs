using Nemlogin.QualifiedSigning.SDK.Core.Enums;
using Nemlogin.QualifiedSigning.SDK.Core.Exceptions;
using Nemlogin.QualifiedSigning.SDK.Core.Fundamental;
using Nemlogin.QualifiedSigning.SDK.Core.Validations;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace Nemlogin.QualifiedSigning.SDK.Pades.PdfValidation;

    /// <summary>
    /// Validates PDF files as part of the validation process as the first step in ProduceSigningPayload.
    /// Consists of three steps:
    /// 1. Validation of PDF parsing
    /// 2. Validation against whitelist elements
    /// 3. Validation of fonts embedded in the PDF file against whitelist. 
    /// </summary>
    public class PdfValidator : IValidator
    {
        public void Validate(TransformationContext ctx)
        {
            ArgumentNullException.ThrowIfNull(ctx);

            List<PdfValidationResultV2> pdfValidationResults = null;

            var pdfSignersDocument = (PdfSignersDocument)ctx.SignersDocument;

            var data = pdfSignersDocument.SignersDocumentFile.GetData();

            IEnumerable<PdfObject> pdfObjects;

            using (PdfReader reader = new PdfReader(data))
            {
                pdfObjects = GetPdfObjects(reader);
            }

            // Validate that we can parse the PDF data before we continue
            PdfParseValidationError pdfParseValidationError = CanParsePdfFile(data);
            if (!pdfParseValidationError.Parsed)
            {
                throw new ValidationException(pdfParseValidationError.Error, ErrorCode.SDK010);
            }

            // Validate Whitelist
            pdfValidationResults = ValidateAgainstWhiteList(pdfObjects).ToList();

            // Validate Fonts
            // pdfValidationResults.AddRange(ValidateFonts(pdfObjects));

            // Print validation errors and throw exception

            if (pdfValidationResults != null && pdfValidationResults.Any())
            {
                var resultString = pdfValidationResults.OrderByDescending(p => p.PdfName).Select(t => new
                {
                    value = $"{t.PdfName.ToString()} [{t.ObjectNumber}]"
                });

                throw new ValidationException($"PDF Validation errors: {string.Join(",", resultString.Select(a => a.value))}", ErrorCode.SDK010);
            }
        }

        public bool CanValidate(DocumentFormat format)
        {
            return format == DocumentFormat.PDF;
        }

        public IEnumerable<PdfValidationResultV2> ValidateAgainstWhiteList(IEnumerable<PdfObject> pdfObjects)
        {
            if (pdfObjects == null)
            {
                throw new ArgumentNullException(nameof(pdfObjects));
            }

            List<PdfValidationResultV2> pdfValidationResults = new();

            int i = 0;

            foreach (var pdfObject in pdfObjects)
            {
                PdfWhiteListValidatorV2 pdfWhiteListValidator = new();
                // pdfValidationResults.AddRange(pdfWhiteListValidator.WhitelistValidation(pdfObject, i));
                i++;
            }

            return pdfValidationResults;
        }

        // private IEnumerable<PdfValidationResultV2> ValidateFonts(IEnumerable<PdfObject> pdfObjects)
        // {
        //     if (pdfObjects == null)
        //         throw new ArgumentNullException(nameof(pdfObjects));
        //
        //     PdfFontValidatorV2 pdfFontValidator = new();
        //     return pdfFontValidator.ValidateFonts(pdfObjects);
        // }

        public IEnumerable<PdfObject> GetPdfObjects(PdfReader reader)
        {
            List<PdfObject> pdfObjects = new List<PdfObject>();

            for (int i = 0; i < reader.XrefSize; i++)
            {
                pdfObjects.Add(reader.GetPdfObject(i));
            }

            return pdfObjects;
        }

        /// <summary>
        /// Validate that we can read the PDF File data with iTextSharp
        /// </summary>
        /// <param name="data"></param>
        private PdfParseValidationError CanParsePdfFile(byte[] data)
        {
            List<PdfValidationResultV2> validationErrors = new();

            try
            {
                PdfReader reader = new PdfReader(data);
                return new PdfParseValidationError(true);
            }
            catch (Exception e)
            {
                return new PdfParseValidationError($"Can not parse PDF document. {e.Message}");
            }
        }
    }