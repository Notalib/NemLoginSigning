namespace Nemlogin.QualifiedSigning.SDK.Core.Model;

/// <summary>
/// Contains the validation result and the xml-based ETSI report returned 
/// from the ValidationService
/// </summary>
public class ValidationReport
{
    public ValidationResult Result { get; set; }

    public string EtsiReport { get; set; }
}