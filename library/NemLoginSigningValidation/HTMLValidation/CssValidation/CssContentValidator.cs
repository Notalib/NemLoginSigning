using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using HtmlAgilityPack;

namespace NemLoginSigningValidation.HTMLValidation
{
    /// <summary>
    /// .NET implementation similar to the the NemID validation project
    /// Validates CSS definition against a whitelist
    /// </summary>
    public class CssContentValidator
    {
        private HashSet<string> _allowedStyles = new HashSet<string>() 
        {
            "COLOR", "BACKGROUND", "BACKGROUND-COLOR", "FLOAT", "OVERFLOW", "LINE-HEIGHT", "POSITION", "TOP", "BOTTOM", "LEFT", "RIGHT",
            "MARGIN", "MARGIN-RIGHT", "MARGIN-TOP", "MARGIN-LEFT", "MARGIN-BOTTOM", "WIDTH", "HEIGHT", "FLOAT", "CLEAR", "DISPLAY", "WHITE-SPACE" 
        };

        private HashSet<string> _allowedFamilies = new HashSet<string>() { "BORDER", "FONT", "TEXT", "LIST", "PADDING" };

        private HashSet<string> _disAllowedFunctions = new HashSet<string>() { "URL", "ATTR", "EXPRESSION", "ELEMENT" };

        /// <summary>
        /// Entry for CSS validation.
        /// </summary>
        /// <param name="attribute">The style attribute to be validated against the whitelist</param>
        /// <returns>String with errordescription. If empty, no errors detected</returns>
        public string Validate(HtmlAttribute attribute)
        {
            var deEntitized = HtmlEntity.DeEntitize(attribute.Value);

            string result = string.Empty;

            if (!string.IsNullOrEmpty(deEntitized))
            {
                result = Parse(deEntitized, attribute);
            }

            return result;
        }

        private string Parse(string content, HtmlAttribute attribute)
        {
            string errorText = string.Empty;

            if (content.Contains("@"))
            {
                return "@XXX' CSS instructions are not allowed";
            }

            if (content.Contains("{"))
            {
                content = content.Substring(content.IndexOf("{", StringComparison.OrdinalIgnoreCase));
            }

            string[] tokens = content.Split("{".ToCharArray());

            for (int i = 0; i < tokens.Length; i++)
            {
                var token = tokens[i];

                if (token.Contains("}"))
                {
                    string end = token.Substring(token.IndexOf("}", StringComparison.OrdinalIgnoreCase));

                    token = token.Substring(0, token.IndexOf("}", StringComparison.OrdinalIgnoreCase));

                    if (end.Contains("/") || end.Contains("&#47;"))
                    {
                        errorText = ParseStyle(end, attribute);
                    }
                }

                if (string.IsNullOrEmpty(errorText))
                {
                    errorText = ParseStyle(token, attribute);
                }
            }

            return errorText;
        }

        private string ParseStyle(string end, HtmlAttribute attribute)
        {
            string errorText = string.Empty;
            string[] outer = end.Split(";".ToCharArray());

            foreach (var token in outer)
            {
                if (!string.IsNullOrEmpty(errorText))
                    return errorText;

                if (!string.IsNullOrEmpty(errorText))
                {
                    break;
                }

                errorText = ValidateStyleDefinition(token, attribute);
            }

            return errorText;
        }

        private string GetFamily(string name)
        {
            int index = name.IndexOf("-", StringComparison.OrdinalIgnoreCase);

            if (index == -1)
            {
                return name;
            }

            return name.Substring(0, index);
        }

        private string ValidateStyleDefinition(string token, HtmlAttribute attribute)
        {
            token = token?.Trim();

            if (string.IsNullOrEmpty(token))
            {
                return string.Empty;
            }

            if (token.IndexOf(":", StringComparison.OrdinalIgnoreCase) <= 0)
            {
                return $"{token.ToLowerInvariant()} is not a valid style definition";
            }

            string name = token.Substring(0, token.IndexOf(":", StringComparison.OrdinalIgnoreCase));

            name = name.Trim().ToUpperInvariant();

            string value = token.Substring(name.Length);

            value = value.Trim().ToUpperInvariant();


            bool allowed = _allowedStyles.Contains(name);

            if (!allowed)
            {
                allowed = _allowedFamilies.Contains(GetFamily(name));
            }

            if (!allowed)
            {
                return $"{name.ToLowerInvariant()} is not an allowed style property";
            }

            if (value.ToUpperInvariant().Contains("URL"))
            {
                return $"Url's are not allowed in styles - {name.ToLowerInvariant()} is defined using an url";
            }

            if (_disAllowedFunctions.Any(s => attribute.Value.ToUpperInvariant().Contains(s.ToUpperInvariant())))
            {
                return $"Function type are not allowed in styles functions - '{attribute.Value}'";
            }

            // No errors
            return string.Empty;
        }
    }
}
