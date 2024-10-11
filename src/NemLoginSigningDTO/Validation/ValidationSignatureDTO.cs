using System;
using System.Collections.Generic;

namespace NemLoginSigningDTO.Validation;

public class ValidationSignatureDTO
{
    public string SignatureFormat { get; set; }

    public string SignatureLevel { get; set; }

    public string Indication { get; set; }

    public string SubIndication { get; set; }

    public DateTime? SigningTime { get; set; }

    public string SignedBy { get; set; }

    public string Email { get; set; }

    public ICollection<ValidationCertificateDTO> CertificateChain { get; set; }

    public ICollection<string> Errors { get; set; }

    public ICollection<string> Warnings { get; set; }

    public ICollection<string> Infos { get; set; }
}
