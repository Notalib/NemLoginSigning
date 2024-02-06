using System.ComponentModel;

namespace NemLoginSigningCore.Core
{
    /// <summary>
    /// Defining the language to use in the signing client.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:Element should begin with upper-case letter", Justification = "ISO language names are lowercase")]
    public enum Language
    {
        /// <summary>
        /// Danish
        /// </summary>
        [Description("Danish")]
        da,

        /// <summary>
        /// English
        /// </summary>
        [Description("English")]
        en,
    }
}
