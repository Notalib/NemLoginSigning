using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;

namespace NemLoginSigningValidation.HTMLValidation
{
    /// <summary>
    /// .Net implementation of whitelist validation similar to the NemID validation project.
    /// Validates HTML elements/attributes definition against a whitelist
    /// </summary>
    public class HtmlSignTextValidator
    {
        private readonly HTMLWhiteListValidatorPolicies _htmlWhiteListValidatorPolicies;

        public List<string> ErrorMessages { get; private set; }

        public string CleanHTML { get; private set; }

        public string ErrorString => $"{string.Join(",", ErrorMessages.ToArray())}";

        public HtmlSignTextValidator()
        {
            ErrorMessages = new List<string>();
            _htmlWhiteListValidatorPolicies = new HTMLWhiteListValidatorPolicies();
        }

        public bool Validate(string html)
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(html);

            var errors = ValidateHtmlDocument(document);

            if (errors.Any())
            {
                ErrorMessages = errors.ToList();
                return false;
            }

            return true;
        }

        private IEnumerable<string> ValidateHtmlDocument(HtmlDocument document)
        {
            List<string> errors = new List<string>();

            if (document.ParseErrors.Any())
            {
                var parseErrorsReason = document.ParseErrors.Select(s => s.Reason);

                errors = parseErrorsReason.ToList();
                return errors;
            }

            errors = ValidateHtmlNode(document.DocumentNode).ToList();

            return errors;
        }

        private IEnumerable<string> ValidateHtmlNode(HtmlNode node)
        {
            List<string> errors = new List<string>();

            if (node.NodeType == HtmlNodeType.Element)
            {
                var result = _htmlWhiteListValidatorPolicies.Validate(node);

                if (result.Any())
                {
                    errors.AddRange(result);
                }
            }

            if (node.HasChildNodes)
            {
                for (int i = 0; i < node.ChildNodes.Count; i++)
                {
                    errors.AddRange(ValidateHtmlNode(node.ChildNodes[i]));
                }
            }

            return errors;
        }
    }
}