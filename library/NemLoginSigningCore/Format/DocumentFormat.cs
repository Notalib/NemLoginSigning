using System.ComponentModel;

namespace NemLoginSigningCore.Format
{
    /// <summary>
    ///  Defines the valid SD (signer's document) formats.
    /// </summary>
    public enum DocumentFormat
    {
        /// <summary>
        /// Plain UTF-8 text
        /// </summary>
        TEXT,

        /// <summary>
        /// The HTML subset supported by the NemLog-In signing component
        /// </summary>
        [Description("HTML")]
        HTML,

        /// <summary>
        /// XML
        /// </summary>
        [Description("XML")]
        XML,

        /// <summary>
        /// The PDF subset supported by the NemLog-In signing component
        /// </summary>
        [Description("PDF")]
        PDF,
    }
}
