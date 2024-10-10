using System;
using System.Collections.Generic;

namespace NemLoginSignatureValidationService.Model
{
    /// <summary>
    /// Encapsulates a signature certificate
    /// </summary>
    public class ValidationCertificate
    {
        public string SubjectDN { get; set; }

        public string SerialNumber { get; set; }

        public DateTime? NotBefore { get; set; }

        public DateTime? NotAfter { get; set; }

        public List<string> Policies { get; set; }
    }
}
