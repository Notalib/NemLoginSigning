using System;
using System.Collections.Generic;

namespace NemLoginSigningDTO
{
    public class ValidationCertificateDTO
    {
        public string SubjectDN { get; set; }

        public string SerialNumber { get; set; }

        public DateTime? NotBefore { get; set; }

        public DateTime? NotAfter { get; set; }

        public ICollection<string> Policies { get; set; }
    }
}
