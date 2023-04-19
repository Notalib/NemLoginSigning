using System;
using Microsoft.Extensions.Logging;
using NemLoginSigningCore.Core;

namespace NemLoginSigningValidation.PlainTextValidation
{
    /// <summary>
    /// Validation of plain text. No validation is needed.
    /// </summary>
    public class PlainTextValidator : IValidator
    {
        private readonly ILogger _logger;

        public PlainTextValidator(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Validate(TransformationContext ctx)
        {
            _logger.LogInformation("Validating PlainText - Everything is good..");
        }
    }
}