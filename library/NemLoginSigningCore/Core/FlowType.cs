using System.ComponentModel;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NemLoginSigningCore.Core
{
    /// <summary>
    /// Defines the valid signing component flows
    /// Values defined verbatim according to the Signing flow specification.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum FlowType
    {
        /// <summary>
        /// Service Provider flow
        /// </summary>
        [Description("ServiceProvider")]
        ServiceProvider,

        /// <summary>
        /// Broker flow
        /// </summary>
        [Description("Broker")]
        Broker,
    }
}