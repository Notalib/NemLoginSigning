using System.ComponentModel;
using System.Runtime.Serialization;

namespace Nemlogin.QualifiedSigning.SDK.Core.Enums;

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
    
    [EnumMember(Value = "XML-NOT-ETSI")]
    XML_NOT_ETSI,

    [EnumMember(Value = "XAdES-C")]
    XAdES_C,

    [EnumMember(Value = "XAdES-X")]
    XAdES_X,

    [EnumMember(Value = "XAdES-XL")]
    XAdES_XL,

    [EnumMember(Value = "XAdES-A")]
    XAdES_A,

    [EnumMember(Value = "XAdES-BASELINE-LTA")]
    XAdES_BASELINE_LTA,

    [EnumMember(Value = "XAdES-BASELINE-LT")]
    XAdES_BASELINE_LT,

    [EnumMember(Value = "XAdES-BASELINE-T")]
    XAdES_BASELINE_T,

    [EnumMember(Value = "XAdES-BASELINE-B")]
    XAdES_BASELINE_B,

    [EnumMember(Value = "CMS-NOT-ETSI")]
    CMS_NOT_ETSI,

    [EnumMember(Value = "CAdES-BASELINE-LTA")]
    CAdES_BASELINE_LTA,

    [EnumMember(Value = "CAdES-BASELINE-LT")]
    CAdES_BASELINE_LT,

    [EnumMember(Value = "CAdES-BASELINE-T")]
    CAdES_BASELINE_T,

    [EnumMember(Value = "CAdES-BASELINE-B")]
    CAdES_BASELINE_B,

    [EnumMember(Value = "CAdES-101733-C")]
    CAdES_101733_C,

    [EnumMember(Value = "CAdES-101733-X")]
    CAdES_101733_X,

    [EnumMember(Value = "CAdES-101733-A")]
    CAdES_101733_A,

    [EnumMember(Value = "PDF-NOT-ETSI")]
    PDF_NOT_ETSI,

    [EnumMember(Value = "PAdES-BASELINE-LTA")]
    PAdES_BASELINE_LTA,

    [EnumMember(Value = "PAdES-BASELINE-LT")]
    PAdES_BASELINE_LT,

    [EnumMember(Value = "PAdES-BASELINE-T")]
    PAdES_BASELINE_T,

    [EnumMember(Value = "PAdES-BASELINE-B")]
    PAdES_BASELINE_B,

    [EnumMember(Value = "PKCS7-B")]
    PKCS7_B,

    [EnumMember(Value = "PKCS7-T")]
    PKCS7_T,

    [EnumMember(Value = "PKCS7-LT")]
    PKCS7_LT,

    [EnumMember(Value = "PKCS7-LTA")]
    PKCS7_LTA,

    [EnumMember(Value = "UNKNOWN")]
    UNKNOWN    
}