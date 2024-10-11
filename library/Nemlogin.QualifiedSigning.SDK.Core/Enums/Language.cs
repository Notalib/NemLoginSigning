using System.ComponentModel;

namespace Nemlogin.QualifiedSigning.SDK.Core.Enums;

/// <summary>
/// Defining the language to use in the signing client.
/// </summary>
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

    /// <summary>
    /// Greenlandic
    /// </summary>
    [Description("Greenlandic")]
    kl,
}