using System;
using System.Collections.Generic;
using System.Linq;

using iTextSharp.text.pdf;
using Microsoft.Extensions.Logging;
using NemLoginSigningCore.Core;
using NemLoginSigningCore.Exceptions;

namespace NemLoginSigningValidation.PDFValidation
{
    /// <summary>
    /// Validates PDF files as part of the validation process as the first step in ProduceSigningPayload.
    /// Consists of three steps:
    /// 1. Validation of PDF parsing
    /// 2. Validation against whitelist elements
    /// 3. Validation of fonts embedded in the PDF file against whitelist.
    /// </summary>
    public class PDFValidator : IValidator
    {
        private readonly ILogger _logger;

        public PDFValidator(ILogger logger)
        {
            ArgumentNullException.ThrowIfNull(logger);
            _logger = logger;
        }

        public void Validate(TransformationContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            List<PdfValidationResult> pdfValidationResults = null;

            PdfSignersDocument pdfSignersDocument = context.SignersDocument as PdfSignersDocument;

            byte[] data = pdfSignersDocument.SignersDocumentFile.GetData();

            IEnumerable<PdfObject> pdfObjects;

            using (PdfReader reader = new PdfReader(data))
            {
                pdfObjects = GetPdfObjects(reader);
            }

            // Validate that we can parse the PDF data before we continue
            PDFParseValidationError pdfParseValidationError = CanParsePDFFile(data);
            if (!pdfParseValidationError.Parsed)
            {
                throw new ValidationException(pdfParseValidationError.Error, ErrorCode.SDK010);
            }

            // Validate Whitelist
            pdfValidationResults = ValidateAgainstWhiteList(pdfObjects).ToList();

            // Validate Fonts
            pdfValidationResults.AddRange(ValidateFonts(pdfObjects));

            // Print validation errors and throw exception
            if (pdfValidationResults != null && pdfValidationResults.Any())
            {
                var resultString = pdfValidationResults.OrderByDescending(p => p.PdfName).Select(t => new
                {
                    value = $"{t.PdfName.ToString()} [{t.ObjectNumber}]"
                });

                string errors = String.Join(", ", resultString.Select(a => a.value));

                _logger.LogInformation("PDF Validation errors: {Errors}", errors);

                throw new ValidationException($"PDF Validation errors: {errors}", ErrorCode.SDK010);
            }
        }

        public IEnumerable<PdfValidationResult> ValidateAgainstWhiteList(IEnumerable<PdfObject> pdfObjects)
        {
            ArgumentNullException.ThrowIfNull(pdfObjects);

            List<PdfValidationResult> pdfValidationResults = new List<PdfValidationResult>();

            int i = 0;

            foreach (PdfObject pdfObject in pdfObjects)
            {
                PdfWhiteListValidator pdfWhiteListValidator = new PdfWhiteListValidator();
                pdfValidationResults.AddRange(pdfWhiteListValidator.WhitelistValidation(pdfObject, i));
                i++;
            }

            return pdfValidationResults;
        }

        public IEnumerable<PdfValidationResult> ValidateFonts(IEnumerable<PdfObject> pdfObjects)
        {
            ArgumentNullException.ThrowIfNull(pdfObjects);

            PdfFontValidator pdfFontValidator = new PdfFontValidator();
            return pdfFontValidator.ValidateFonts(pdfObjects);
        }

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
        public PDFParseValidationError CanParsePDFFile(byte[] data)
        {
            List<PdfValidationResult> validationErrors = new List<PdfValidationResult>();

            try
            {
                PdfReader reader = new PdfReader(data);
                return new PDFParseValidationError(true);
            }
            catch (Exception e)
            {
                return new PDFParseValidationError($"Can not parse PDF document. {e.Message}");
            }
        }
    }
}
