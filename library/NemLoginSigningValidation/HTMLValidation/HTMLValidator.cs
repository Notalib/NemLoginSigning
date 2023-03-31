using System;
using Microsoft.Extensions.Logging;
using NemLoginSigningCore.Core;
using NemLoginSigningCore.Exceptions;
using NemLoginSigningCore.Logging;
using NemLoginSigningValidation.HTMLValidation;

namespace NemLoginSigningValidation
{
    /// <summary>
    /// IValidator implementation that validates HTML input from signersdocument.
    /// </summary>
    public class HTMLValidator : IValidator
    {
        public void Validate(TransformationContext ctx)
        {
            var logger = LoggerCreator.CreateLogger<HTMLValidator>();

            string html = ctx.SignersDocument.DataAsText();

            HtmlSignTextValidator validator = new HtmlSignTextValidator();
            bool result = validator.Validate(html);

            if (result == false)
            {
                string validationErrors = string.Join($"; {Environment.NewLine}", validator.ErrorMessages.ToArray());
                logger.LogDebug($"HTML Validation Errors: {validationErrors}");

                throw new ValidationException(validationErrors, ErrorCode.SDK010);
            }
        }
    }
}