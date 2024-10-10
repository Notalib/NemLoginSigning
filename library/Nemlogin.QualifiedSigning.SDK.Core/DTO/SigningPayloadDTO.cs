namespace Nemlogin.QualifiedSigning.SDK.Core.DTO;

public class SigningPayloadDTO
{
    public SigningPayloadDTO() { }

    public SigningPayloadDTO(SigningPayload signingPayload)
    {
        if (signingPayload == null)
        {
            throw new ArgumentNullException(nameof(signingPayload));
        }

        SignatureParameters = signingPayload.SignatureParameters;
        Dtbs = Convert.ToBase64String(signingPayload.DataToBeSigned.GetData());
    }
       
    public string SignatureParameters { get; set; }

    public string Dtbs { get; set; }
}