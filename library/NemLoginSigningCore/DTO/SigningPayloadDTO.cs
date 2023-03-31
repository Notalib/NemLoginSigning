using System;
using System.Collections.Generic;
using System.Text;
using NemLoginSigningCore.Core;

namespace NemLoginSigningCore.DTO
{
    /// <summary>
    /// Constitutes a DTO form of the SigningPayload class
    /// suitable for passing on to the signing client.
    /// </summary>
    public class SigningPayloadDTO
    {
        public SigningPayloadDTO()
        {
        }

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
}