using Nemlogin.QualifiedSigning.SDK.Core.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Nemlogin.QualifiedSigning.SDK.Core.Model;

/// <summary>
/// Encapsulates the validation result for a Signature
/// </summary>
public class ValidationSignature
{
    [JsonConverter(typeof(StringEnumConverter))]
    public SignatureFormat SignatureFormat { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public SignatureLevel SignatureLevel { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public Indication Indication { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public SubIndication SubIndication { get; set; }

    public DateTime? SigningTime { get; set; }

    public string SignedBy { get; set; }

    public string Email { get; set; }

    public List<ValidationCertificate> CertificateChain { get; set; }

    public IList<string> Errors { get; set; }

    public IList<string> Warnings { get; set; }

    public IList<string> Infos{ get; set; }
}