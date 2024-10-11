using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using System.ComponentModel;

namespace Nemlogin.QualifiedSigning.SDK.Core.Fundamental;

/// <summary>
/// Certificate policy 
/// Values defined verbatim according to the Signing flow specification.
/// </summary>
[Flags]
[JsonConverter(typeof(StringEnumConverter))]
public enum AcceptedCertificatePolicies : int
{
    [Description("Person")]
    Person = 1,

    [Description("Employee")]
    Employee = 2,

    [Description("Organization")]
    Organization = 4,
}