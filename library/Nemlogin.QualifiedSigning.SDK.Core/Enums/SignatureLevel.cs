using System.Runtime.Serialization;

namespace Nemlogin.QualifiedSigning.SDK.Core.Enums;

/// <summary>
/// ETSI signature qualification levels.
/// Inspired by the DSS implementation of { eu.europa.esig.dss.enumerations.SignatureQualification }.
/// </summary>
public enum SignatureLevel
{
    [EnumMember(Value = "QESig")]
    QESIG,

    [EnumMember(Value = "QESeal")]
    QESEAL,

    [EnumMember(Value = "QES")]
    QES,

    [EnumMember(Value = "AdESigQC")]
    ADESIG_QC,

    [EnumMember(Value = "AdESealQC")]
    ADESEAL_QC,

    [EnumMember(Value = "AdESQC")]
    ADES_QC,

    [EnumMember(Value = "AdESig")]
    ADESIG,

    [EnumMember(Value = "AdESeal")]
    ADESEAL,

    [EnumMember(Value = "AdES")]
    ADES,

    [EnumMember(Value = "indeterminateQESig")]
    INDETERMINATE_QESIG,

    [EnumMember(Value = "indeterminateQESeal")]
    INDETERMINATE_QESEAL,

    [EnumMember(Value = "indeterminateQES")]
    INDETERMINATE_QES,

    [EnumMember(Value = "indeterminateAdESigQC")]
    INDETERMINATE_ADESIG_QC,

    [EnumMember(Value = "indeterminateAdESealQC")]
    INDETERMINATE_ADESEAL_QC,

    [EnumMember(Value = "indeterminateAdESQC")]
    INDETERMINATE_ADES_QC,

    [EnumMember(Value = "indeterminateAdESig")]
    INDETERMINATE_ADESIG,

    [EnumMember(Value = "indeterminateAdESeal")]
    INDETERMINATE_ADESEAL,

    [EnumMember(Value = "indeterminateAdES")]
    INDETERMINATE_ADES,

    [EnumMember(Value = "notAdESbutQCwithQSCD")]
    NOT_ADES_QC_QSCD,

    [EnumMember(Value = "notAdESbutQC")]
    NOT_ADES_QC,

    [EnumMember(Value = "notAdES")]
    NOT_ADES,

    [EnumMember(Value = "notApplicable")]
    NA
}