using NemLoginSigningCore.Core;
using System.Collections.Generic;

namespace NemLoginSigningWebApp.Logic
{
    /// <summary>
    /// Interface defining the SignersDocumentLoader
    /// </summary>
    public interface ISignersDocumentLoader
    {
        IEnumerable<SignersDocument> GetFiles();
        
        SignersDocument CreateSignersDocumentFromFile(string filePath);
    }
}