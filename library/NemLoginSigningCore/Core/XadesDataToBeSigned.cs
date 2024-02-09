using NemLoginSigningCore.Format;

namespace NemLoginSigningCore.Core
{
    /// <summary>
    /// Encapsulates XAdES DTBS
    /// </summary>
    public class XadesDataToBeSigned : DataToBeSigned
    {
        public XadesDataToBeSigned(byte[] data, string name)
            : base(SignatureFormat.XAdES, data, name)
        {
        }
    }
}
