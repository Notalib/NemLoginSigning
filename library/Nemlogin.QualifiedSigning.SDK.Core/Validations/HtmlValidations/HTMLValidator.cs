using Nemlogin.QualifiedSigning.SDK.Core.Enums;
using Nemlogin.QualifiedSigning.SDK.Core.Exceptions;
using Nemlogin.QualifiedSigning.SDK.Core.Fundamental;

namespace Nemlogin.QualifiedSigning.SDK.Core.Validations;

/// <summary>
/// IValidator implementation that validates HTML input from signersdocument.
/// </summary>
public class HTMLValidator : IValidator
{
    public void Validate(TransformationContext ctx)
    {
        string html = ctx.SignersDocument.DataAsText();

        HtmlSignTextValidator validator = new HtmlSignTextValidator();
        bool result = validator.Validate(html);

        if (result == false)
        {
            string validationErrors = string.Join($"; {Environment.NewLine}", validator.ErrorMessages.ToArray());
            throw new ValidationException(validationErrors, ErrorCode.SDK010);
        }
    }
        
    public bool CanValidate(DocumentFormat format)
    {
        return format == DocumentFormat.HTML;
    }
}