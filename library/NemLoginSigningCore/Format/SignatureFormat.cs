using System.ComponentModel;

namespace NemLoginSigningCore.Format
{
    /// <summary>
    /// Defines the valid DTBS (Data To Be Signed) formats.
    /// </summary>
    public enum SignatureFormat
    {
        /// <summary>
        /// XML containing XMLDsig
        /// </summary>
        [Description("XAdES-B-LTA")]
        XAdES,

        /// <summary>
        /// PDF containing signature dictionary
        /// </summary>
        [Description("PAdES-B-LTA")]
        PAdES,
    }
}
