using System.ComponentModel;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NemLoginSigningCore.Core
{
    /// <summary>
    /// Persistence level for UUID in the "subjectSerialNumber" of the short term certificate.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SsnPersistenceLevel
    {
        /// <summary>
        /// Session
        /// </summary>
        [Description("Session")]
        Session,

        /// <summary>
        /// Global
        /// </summary>
        [Description("Global")]
        Global,
    }
}
