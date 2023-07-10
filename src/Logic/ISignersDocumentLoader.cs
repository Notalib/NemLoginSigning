using NemLoginSigningCore.Core;
using NemLoginSigningDTO.Signing;

namespace NemLoginSigningWebApp.Logic
{
    /// <summary>
    /// Interface defining the SignersDocumentLoader
    /// </summary>
    public interface ISignersDocumentLoader
    {
        SignersDocument CreateSignersDocumentFromFile(string filePath);

        SignersDocument CreateSignersDocumentFromContent(string fileName, byte[] content, SignProperties signProperties);

        SignersDocument CreateSignersDocumentFromSigningDocumentDTO(SigningDocumentDTO dto);
    }
}