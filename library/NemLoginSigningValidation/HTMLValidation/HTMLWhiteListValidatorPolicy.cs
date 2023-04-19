using System.Collections.Generic;
using System.Linq;

namespace NemLoginSigningValidation
{
    /// <summary>
    /// .Net implementation of HTML validation originally from the NemID validation project.
    /// Validates HTML elements/attributes definition against a whitelist
    /// </summary>
    public class HTMLWhiteListValidatorPolicy
    {
        public HTMLWhiteListValidatorPolicy(string[] attributes, string[] onElements)
        {
            AllowedAttributes = new HashSet<string>(attributes.ToList());
            OnElements = new HashSet<string>(onElements.ToList());
        }

        public HTMLWhiteListValidatorPolicy(string[] attributes, string[] onElements, bool documentLinksOnly)
        {
            AllowedAttributes = new HashSet<string>(attributes.ToList());
            OnElements = new HashSet<string>(onElements.ToList());
            DocumentLinksOnly = documentLinksOnly;
        }

        public HTMLWhiteListValidatorPolicy(string[] allowTextIn)
        {
            AllowTextIn = new HashSet<string>(allowTextIn.ToList());
        }

        public HashSet<string> AllowedAttributes { get; }

        public HashSet<string> OnElements { get; }

        public HashSet<string> AllowTextIn { get; }

        public HashSet<string> Matching { get; }

        public bool DocumentLinksOnly { get; }
    }
}