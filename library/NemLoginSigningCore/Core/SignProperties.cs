using System.Collections.Generic;

namespace NemLoginSigningCore.Core
{
    /// <summary>
    /// For XAdES, the SignProperties are included in the XAdES DTBS.
    /// The purpose is to provide similar functionality to the "SIGN_PROPERTIES"
    /// parameter of the NemID signing client.
    /// </summary>
    public class SignProperties : Dictionary<string, SignPropertyValue>
    {
        public SignProperties()
        {
        }

        public SignProperties(Dictionary<string, SignPropertyValue> dictionary)
            : base(dictionary)
        {
        }
    }
}
