using System.Collections.Generic;

namespace NemLoginSigningDTO.Validation
{
    public class ValidationResultDTO
    {
        public string DocumentName { get; set; }

        public int SignaturesCount { get; set; }

        public int ValidSignaturesCount { get; set; }

        public ICollection<ValidationSignatureDTO> Signatures { get; set; }
    }
}
