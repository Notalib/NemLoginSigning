namespace Nemlogin.QualifiedSigning.SDK.Core.Model;

/// <summary>
/// Contains the result from the ValidationService
/// </summary>
public class ValidationResult
{
    public ValidationResult() { }

    public string DocumentName { get; set; }

    public int SignaturesCount { get; private set; }

    public int ValidSignaturesCount { get; set; }

    public IList<ValidationSignature> Signatures { get; set; }
}