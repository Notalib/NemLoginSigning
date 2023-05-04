namespace NemLoginSigning.DTO
{
    /// <summary>
    /// Constitutes a DTO form of the SigningPayload class
    /// suitable for passing on to the signing client.
    /// </summary>
    public class SigningPayloadDTO
    {
        /// <summary>
        /// SigninParameters (Encoded in signed JWT token
        /// </summary>
        public string SignatureParameters { get; set; }

        /// <summary>
        /// XML Document containing the Documents To Be Signed, in base64.
        /// </summary>
        public string Dtbs { get; set; }
    }
}
