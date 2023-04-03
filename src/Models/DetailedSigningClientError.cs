namespace NemLoginSigningWebApp.Models
{
    /// <summary>
    /// Detailed error code and message which may be returned from the Signing Client.
    /// The error will either have occurred directly in the Signing Client, or in one of
    /// the backend Signing API REST calls performed by the client.
    /// The list of valid ErrorCode values, and proposed error texts that could be displayed
    /// to end users, is included in the SignSDK.
    /// </summary>
    public class DetailedSigningClientError
    {
        public string ErrorCode { get; set; }

        public string ErrorMessage { get; set; }
    }
}