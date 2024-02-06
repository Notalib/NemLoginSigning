namespace NemLoginSignatureValidationService.Model
{
    /// <summary>
    ///  An sub-indication of the validation result.
    ///  Based on the DSS implementation of { eu.europa.esig.dss.enumerations.SubIndication }.
    ///  Source ETSI EN 319 102-1
    /// </summary>
    public enum SubIndication
    {
        FORMAT_FAILURE,
        HASH_FAILURE,
        SIG_CRYPTO_FAILURE,
        REVOKED,
        SIG_CONSTRAINTS_FAILURE,
        CHAIN_CONSTRAINTS_FAILURE,
        CERTIFICATE_CHAIN_GENERAL_FAILURE,
        CRYPTO_CONSTRAINTS_FAILURE,
        EXPIRED,
        NOT_YET_VALID,
        POLICY_PROCESSING_ERROR,
        SIGNATURE_POLICY_NOT_AVAILABLE,
        TIMESTAMP_ORDER_FAILURE,
        NO_SIGNING_CERTIFICATE_FOUND,
        NO_CERTIFICATE_CHAIN_FOUND,
        REVOKED_NO_POE,
        REVOKED_CA_NO_POE,
        OUT_OF_BOUNDS_NO_POE,
        OUT_OF_BOUNDS_NOT_REVOKED,
        CRYPTO_CONSTRAINTS_FAILURE_NO_POE,
        NO_POE,
        TRY_LATER,
        SIGNED_DATA_NOT_FOUND
    }
}