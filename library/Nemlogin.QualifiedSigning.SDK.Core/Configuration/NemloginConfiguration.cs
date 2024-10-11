namespace Nemlogin.QualifiedSigning.SDK.Core.Configuration;

public class NemloginConfiguration
{
    private List<SignatureKeysConfiguration> _signatureKeysConfiguration;

    public NemloginConfiguration()
    {
        _signatureKeysConfiguration = new List<SignatureKeysConfiguration>();
    }

    /// <summary>
    /// The URL to the running signingclient.
    /// </summary>
    public string SigningClientUrl { get; set; }

    /// <summary>
    /// URL for the validation service to validate the signed document.
    /// </summary>
    public string ValidationServiceUrl { get; set; }

    /// <summary>
    /// By setting this property the SignSdk will save the signed document 
    /// to the specified folder.
    /// </summary>
    public string SaveDtbsToFolder { get; set; }

    /// <summary>
    /// SignatureKeys configuration for the signing
    /// </summary>
    public List<SignatureKeysConfiguration> SignatureKeysConfiguration => _signatureKeysConfiguration;
}