using iTextSharp.text.pdf;

namespace Nemlogin.QualifiedSigning.SDK.Pades.PdfValidation;

/// <summary>
/// This is holding the failing validation results when validating PDF documents
/// </summary>
public class PdfValidationResult
{
    public PdfObject PdfObject { get; private set; }

    public PdfName PdfName { get; private set; }

    public int ObjectNumber { get; private set; }

    public PdfValidationResult(PdfObject pdfObject, PdfName pdfName, int objectNumber)
    {
        PdfObject = pdfObject;
        PdfName = pdfName;
        ObjectNumber = objectNumber;
    }
}