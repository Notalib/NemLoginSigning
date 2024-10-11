using PdfSharp.Pdf;

namespace Nemlogin.QualifiedSigning.SDK.Pades.PdfValidation;

/// <summary>
/// This is holding the failing validation results when validating PDF documents
/// </summary>
public class PdfValidationResultV2
{
    public KeyValuePair<string, PdfItem> PdfObject { get; private set; }

    public string PdfName { get; private set; }

    public int ObjectNumber { get; private set; }

    public PdfValidationResultV2(KeyValuePair<string, PdfItem> pdfObject, string pdfName, int objectNumber)
    {
        PdfObject = pdfObject;
        PdfName = pdfName;
        ObjectNumber = objectNumber;
    }
}