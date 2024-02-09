using System;
using System.Collections.Generic;
using System.Linq;

using HtmlAgilityPack;

namespace NemLoginSigningValidation.HTMLValidation
{
    /// <summary>
    /// .Net implementation of HTML validation originally from the NemID validation project.
    /// Validates HTML elements/attributes definition against a whitelist
    /// </summary>
    public class HTMLWhiteListValidatorPolicies : List<HTMLWhiteListValidatorPolicy>
    {
        private readonly List<HTMLWhiteListValidatorPolicy> _whiteListAttributesOnTypes;

        private HashSet<string> _whiteListElements = new HashSet<string>()
        {
             "html", "body", "head", "meta", "style", "title", "p", "div", "span", "ul", "ol", "li",
             "h1", "h2", "h3", "h4", "h5", "h6", "table", "tbody", "thead", "tfoot", "tr", "td", "th",
             "i", "b", "u", "center", "a", "br"
        };

        public HTMLWhiteListValidatorPolicies()
        {
            _whiteListAttributesOnTypes = new List<HTMLWhiteListValidatorPolicy>
            {
                new HTMLWhiteListValidatorPolicy(attributes: new[] { "xmlns" },
                                                                            onElements: new[] { "html" }),
                new HTMLWhiteListValidatorPolicy(attributes: new[] { "charset", "http-equiv", "name", "content" },
                                                                            onElements: new[] { "meta" }),
                new HTMLWhiteListValidatorPolicy(attributes: new[] { "text", "bgcolor", "class", "style" },
                                                                            onElements: new[] { "body" }),
                new HTMLWhiteListValidatorPolicy(attributes: new[] { "type" },
                                                                            onElements: new[] { "style" }),
                new HTMLWhiteListValidatorPolicy(allowTextIn: new[] { "style" }),
                new HTMLWhiteListValidatorPolicy(attributes: new[] { "align", "bgcolor", "style", "class" },
                                                                            onElements: new[] { "p", "div", "span" }),
                new HTMLWhiteListValidatorPolicy(attributes: new[] { "style", "class" },
                                                                            onElements: new[] { "ul", "li", "h1", "h2", "h3", "h4", "h5", "h6" }),
                new HTMLWhiteListValidatorPolicy(attributes: new[] { "start", "type", "style", "class" },
                                                                            onElements: new[] { "ol" }),
                new HTMLWhiteListValidatorPolicy(attributes: new[] { "border", "cellspacing", "cellpadding", "width", "align", "style" },
                                                                            onElements: new[] { "table" }),
                new HTMLWhiteListValidatorPolicy(attributes: new[] { "bgcolor", "class", "style" },
                                                                            onElements: new[] { "tr" }),
                new HTMLWhiteListValidatorPolicy(attributes: new[] { "bgcolor", "rowspan", "colspan", "align", "valign", "width", "class", "style" },
                                                                            onElements: new[] { "th" }),
                new HTMLWhiteListValidatorPolicy(attributes: new[] { "bgcolor", "rowspan", "colspan", "align", "valign", "width", "class", "style" },
                                                                            onElements: new[] { "td" }),
                new HTMLWhiteListValidatorPolicy(attributes: new[] { "bgcolor", "rowspan", "colspan", "align", "valign", "width", "class", "style" },
                                                                onElements: new[] { "td" }),
                new HTMLWhiteListValidatorPolicy(attributes: new[] { "name" },
                                                                onElements: new[] { "a" }),
                new HTMLWhiteListValidatorPolicy(attributes: new[] { "href" },
                                                                onElements: new[] { "a" }, documentLinksOnly: true)
            };
        }

        public IEnumerable<string> Validate(HtmlNode node)
        {
            List<string> errors = new List<string>();

            ArgumentNullException.ThrowIfNull(node);

            // Check against whitelist
            if (!_whiteListElements.Contains(node.Name))
            {
                errors.Add($"HTML not valid: {node.Name.ToLowerInvariant()}");
                return errors;
            }

            // Validate all attributes
            foreach (HtmlAttribute attribute in node.Attributes)
            {
                if (attribute.Name == "style")
                {
                    CssContentValidator cssContentValidator = new CssContentValidator();
                    string result = cssContentValidator.Validate(attribute);

                    if (!string.IsNullOrEmpty(result))
                    {
                        errors.Add(result);
                    }
                }
                else
                {
                    if (!IsElementWhiteListed(node.Name))
                    {
                        errors.Add($"HTML not valid: {node.Name}");
                    }

                    if (!AttributeAllowedOnElement(node.Name, attribute.Name, attribute.Value))
                    {
                        errors.Add($"HTML not valid. Element: {node.Name}. Attribute: {attribute.Value}");
                    }
                }
            }

            if (AttributeExistOnlyOnceValidation(node.Attributes, "href"))
            {
                errors.Add("Invalid html: href");
            }

            return errors;
        }

        private bool AttributeExistOnlyOnceValidation(HtmlAttributeCollection attributes, string name)
        {
            IEnumerable<IGrouping<string, HtmlAttribute>> attributesGrouped = attributes.GroupBy(w => w.Name).Where(c => c.Count() > 1);
            return attributesGrouped.Where(w => w.Key.Equals(name, StringComparison.InvariantCultureIgnoreCase)).Any();
        }

        private bool AttributeAllowedOnElement(string tagName, string attributeName, string value)
        {
            List<HTMLWhiteListValidatorPolicy> withElementsAndAttributes = _whiteListAttributesOnTypes.Where(w => w.OnElements != null && w.OnElements.Any() && w.AllowedAttributes != null && w.AllowedAttributes.Any()).ToList();
            IEnumerable<HTMLWhiteListValidatorPolicy> matchingWhiteList = withElementsAndAttributes.Where(w => w.OnElements.Contains(tagName) && w.AllowedAttributes.Contains(attributeName));

            if (!matchingWhiteList.Any())
            {
                return false;
            }

            HTMLWhiteListValidatorPolicy anyMatch = matchingWhiteList.FirstOrDefault();

            if (anyMatch != null)
            {
                if (anyMatch.DocumentLinksOnly)
                {
                    if (!value.StartsWith("#", StringComparison.OrdinalIgnoreCase))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool IsElementWhiteListed(string tagName)
        {
            return _whiteListElements.Any(c => c.Contains(tagName, StringComparison.InvariantCultureIgnoreCase));
        }

        private IEnumerable<HTMLWhiteListValidatorPolicy> ElementsToCheck(HtmlNode node)
        {
            return _whiteListAttributesOnTypes.Where(c => c.OnElements.Contains(node.Name));
        }
    }
}
