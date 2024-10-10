namespace Nemlogin.QualifiedSigning.SDK.Pades.Logic;

/// <summary>
/// Data transfer class to use internally in PdfSignatureStamper for returning multiple values
/// </summary>
public class PreSignResult
{
    public PreSignResult(byte[] signedResult, byte[] signature)
    {
        SignedResult = signedResult;   
        Signature = signature;
    }

    public byte[] SignedResult { get; private set; }

    public byte[] Signature { get; set; }
}