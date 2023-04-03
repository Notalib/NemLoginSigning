using System;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;

using NemLoginSigningWebApp.Models;
using NemLoginSignatureValidationService.Service;
using NemLoginSignatureValidationService.Model;
using NemLoginSigningCore.Utilities;
using NemLoginSigningCore.Configuration;

using static NemLoginSignatureValidationService.Model.SignatureValidationContext;

namespace NemLoginSigningWebApp.Logic
{
    /// <summary>
    /// Handling call to the SigningValidationService which can be called after signing of a document.
    /// </summary>
    public class SigningResultService : ISigningResultService
    {
        private readonly NemloginConfiguration _configuration;
        private readonly ISigningValidationService _signingValidationService;

        public SigningResultService(IOptions<NemloginConfiguration> configuration, ISigningValidationService signingValidationService)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            _configuration = configuration.Value;
            _signingValidationService = signingValidationService;
        }

        public string SignedDocumentFileName(string name, string format)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            var index = name.LastIndexOf(".", StringComparison.Ordinal);

            return index != -1 ? $"{name.Substring(0, index)}-signed.{format}" : $"{name}-signed.{format}";
        }

        public async Task<ValidationReport> ValidateSignedDocumentAsync(string filename, string document)
        {
            SignatureValidationContext ctx = new SignatureValidationContextBuilder()
                .WithDocumentName(filename)
                .WithDocumentData(Convert.FromBase64String(document))
                .WithValidationServiceUrl(_configuration.ValidationServiceURL)
                .Build();

            var validationReport = await _signingValidationService.Validate(ctx);

            return validationReport;
        }

        public SignErrorModel ParseError(string error)
        {
            var data = Convert.FromBase64String(error);

            var errorstring = Encoding.UTF8.GetString(data);

            SignErrorModel signErrorModel = ObjectSerializer.DeSerializeObject<SignErrorModel>(errorstring);

            return signErrorModel;
        }
    }
}