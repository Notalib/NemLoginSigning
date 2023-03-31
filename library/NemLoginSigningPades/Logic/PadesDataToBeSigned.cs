using NemLoginSigningCore.Core;
using NemLoginSigningCore.Format;

namespace NemLoginSigningPades.Logic
{
    /// <summary>
    /// Encapsulates PAdES DTBS
    /// </summary>
    public class PadesDataToBeSigned : DataToBeSigned
    {
        public PadesDataToBeSigned(byte[] data, string name) : base(SignatureFormat.PAdES, data, name) { }
    }
}
