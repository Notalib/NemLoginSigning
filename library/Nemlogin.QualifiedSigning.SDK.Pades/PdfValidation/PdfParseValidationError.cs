namespace Nemlogin.QualifiedSigning.SDK.Pades.PdfValidation;

public class PdfParseValidationError
{
    public bool Parsed { get; private set; }
    public string Error { get; private set; }

    // Explicitly settings parsed to through for clarity!
    public PdfParseValidationError(bool parsed)
    {
        Parsed = parsed;
    }

    public PdfParseValidationError(string error)
    {
        // If we parse in an error, parsed is false
        Parsed = false;
        this.Error = error;
    }
}