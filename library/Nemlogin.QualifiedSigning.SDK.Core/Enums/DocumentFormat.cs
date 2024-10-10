using System.ComponentModel;
namespace Nemlogin.QualifiedSigning.SDK.Core.Enums;

/// <summary>
///  Defines the valid SD (signer's document) formats.
/// </summary>
public enum DocumentFormat
{
    /// <summary>
    /// Plain UTF-8 text 
    /// </summary>
    [Description("TEXT")]
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