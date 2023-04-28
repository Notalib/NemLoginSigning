using System;
using System.Collections.Generic;

namespace NemLoginSignatureValidationService.Model
{
    /// <summary>
    /// Contains the result from the ValidationService
    /// </summary>
    public class ValidationResult
    {
        public ValidationResult()
        {
        }

        public string DocumentName { get; set; }

        public int SignaturesCount { get; set; }

        public int ValidSignaturesCount { get; set; }

        public IList<ValidationSignature> Signatures { get; set; }
    }
}