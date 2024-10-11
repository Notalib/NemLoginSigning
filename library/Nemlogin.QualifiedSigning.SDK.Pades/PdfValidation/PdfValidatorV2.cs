#nullable enable
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
public class PdfValidatorV2 : IValidator
{
    public void Validate(TransformationContext ctx)
    {
        ArgumentNullException.ThrowIfNull(ctx);

        try
        {
            byte[] data = ((PdfSignersDocument)ctx.SignersDocument).SignersDocumentFile.GetData();
            using MemoryStream stream = new(data);
            PdfDocument document = PdfReader.Open(stream);

            // Validate Whitelist
            List<PdfValidationResultV2>? pdfValidationResults = (ValidateAgainstWhiteList(document.Pages) ?? Array.Empty<PdfValidationResultV2>()).ToList();
            // Validate Fonts
            // pdfValidationResults.AddRange(ValidateFonts(document.Pages));

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
        catch (Exception e)
        {
            throw new IOException(
                $"File {ctx.SignersDocument.SignersDocumentFile.Name} @ {ctx.SignersDocument.SignersDocumentFile.Path} is not validated!",
                e
            );
        }
    }

    public bool CanValidate(DocumentFormat format)
    {
        return format == DocumentFormat.PDF;
    }

    private IEnumerable<PdfValidationResultV2>? ValidateAgainstWhiteList(PdfPages pdfPages)
    {
        if (pdfPages == null)
        {
            throw new ArgumentNullException(nameof(pdfPages));
        }

        List<PdfValidationResultV2>? pdfValidationResults = new();

        int i = 0;

        foreach (PdfPage page in pdfPages)
        {
            foreach (KeyValuePair<string, PdfItem?> element in page.Elements)
            {

                // Validate keys in the resources dictionary
                PdfWhiteListValidatorV2 pdfWhiteListValidatorV2 = new();
                pdfValidationResults?.AddRange(pdfWhiteListValidatorV2.WhitelistValidation(element, i));
                i++;
            }
        }

        return pdfValidationResults;
    }
}