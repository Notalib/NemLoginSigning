using System;
using NemLoginSigningWebApp.Models;
using System.Threading.Tasks;
using NemLoginSignatureValidationService.Model;

namespace NemLoginSigningWebApp.Logic
{
    /// <summary>
    /// Interface defining methods to be implemented for the SigningResult
    /// </summary>
    public interface ISigningResultService
    {
        string SignedDocumentFileName(string name, string format);

        SignErrorModel ParseError(string error);

        Task<ValidationReport> ValidateSignedDocumentAsync(string filname, string document);
    }
}