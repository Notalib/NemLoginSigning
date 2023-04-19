using NemLoginSignatureValidationService.Model;

namespace NemLoginSigningWebApp.Models
{
    /// <summary>
    /// SigningResultModel is used for handling the signed document
    /// and eventually the validation report.
    /// </summary>
    public class SigningResultModel : ViewModelBase
    {
        public SigningResultModel()
        {
        }

        public SigningResultModel(ValidationReport validationReport)
        {
            ValidationReport = validationReport;
        }

        public string SignedDocument { get; set; }

        public string Type { get; set; }

        public string Name { get; set; }

        public string Format { get; set; }

        public string MediaType { get; set; }

        public string SignedDocumentFileName { get; set; }

        public ValidationReport ValidationReport { get; }

        public string EtsiReport => ValidationReport.EtsiReport;
    }
}