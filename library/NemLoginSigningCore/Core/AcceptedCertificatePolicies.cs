using System;
using System.ComponentModel;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NemLoginSigningCore.Core
{
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
}
