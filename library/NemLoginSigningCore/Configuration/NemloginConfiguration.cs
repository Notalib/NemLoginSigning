using System;
using System.Collections.Generic;
using System.Text;

namespace NemLoginSigningCore.Configuration
{
    public class NemloginConfiguration
    {
        /// <summary>
        /// The URL to the running signingclient.
        /// </summary>
        public string SigningClientURL { get; set; }

        /// <summary>
        /// URL for the validation service to validate the signed document.
        /// </summary>
        public string ValidationServiceURL { get; set; }

        /// <summary>
        /// URL of the UUID match service.
        /// </summary>
        public string UUIDMatchServiceURL { get; set; }

        /// <summary>
        /// A Broker-specific entity ID provisioned using the NemLog-In Administration Component.
        /// </summary>
        public string EntityID { get; set; }

        /// <summary>
        /// SignatureKeys configuration for the signing.
        /// </summary>
        public SignatureKeysConfiguration SignatureKeysConfiguration { get; set; }
    }
}
