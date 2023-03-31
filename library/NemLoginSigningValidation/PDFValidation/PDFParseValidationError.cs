namespace NemLoginSigningValidation.PDFValidation
{
    public class PDFParseValidationError
    {
        public bool Parsed { get; private set; }
        public string Error { get; private set; }

        // Explicitly settings parsed to through for clarity!
        public PDFParseValidationError(bool parsed)
        {
            Parsed = parsed;
        }

        public PDFParseValidationError(string error)
        {
            // If we parse in an error, parsed is false
            Parsed = false;
            this.Error = error;
        }
    }
}
