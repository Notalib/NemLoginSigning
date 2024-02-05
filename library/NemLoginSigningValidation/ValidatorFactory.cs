using System;
using NemLoginSigningCore.Format;
using NemLoginSigningCore.Logging;
using NemLoginSigningValidation.PDFValidation;
using NemLoginSigningValidation.PlainTextValidation;
using NemLoginSigningValidation.XMLValidation;

namespace NemLoginSigningValidation
{
    /// <summary>
    /// Factory method for creating the concrete validation class
    /// </summary>
    public static class ValidatorFactory
    {
        public static IValidator Create(DocumentFormat format)
        {
            var logger = LoggerCreator.CreateLogger(nameof(ValidatorFactory));

            switch (format)
            {
                case DocumentFormat.TEXT:
                    return new PlainTextValidator(logger);
                case DocumentFormat.HTML:
                    return new HTMLValidator();
                case DocumentFormat.XML:
                    return new XMLValidator();
                case DocumentFormat.PDF:
                    return new PDFValidator(logger);
                default:
                    throw new ArgumentException($"Could not find validator for documentformat: {format}");
            }
        }
    }
}
