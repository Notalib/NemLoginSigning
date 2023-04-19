using System;
using System.Runtime.Serialization;

namespace NemLoginSignatureValidationService.Model
{
    /// <summary>
    /// An indication of the validation result.
    /// Based on the DSS implementation of { eu.europa.esig.dss.enumerations.Indication }.
    /// Source ETSI EN 319 102-1
    /// </summary>
    public enum Indication
    {
        [EnumMember(Value = "TOTAL-PASSED")]
        TOTAL_PASSED,

        [EnumMember(Value = "TOTAL-FAILED")]
        TOTAL_FAILED,

        [EnumMember(Value = "INDETERMINATE")]
        INDETERMINATE
    }
}
