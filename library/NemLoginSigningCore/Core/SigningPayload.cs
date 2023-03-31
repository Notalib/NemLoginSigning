namespace NemLoginSigningCore.Core
{
    /// <summary>
    /// Encapsulated the JWT SignatureParameters and DataToBeSigned as a signing payload.
    /// </summary>
    public class SigningPayload
    {
        public string SignatureParameters { get; private set; }

        public DataToBeSigned DataToBeSigned { get; private set; }

        public SigningPayload(string signedParameters, DataToBeSigned dtbs)
        {
            SignatureParameters = signedParameters;
            DataToBeSigned = dtbs;
        }
    }
}