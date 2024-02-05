using NemLoginSigningCore.Format;

namespace NemLoginSigningCore.Core
{
    /// <summary>
    /// Encapsulates the DTBS (Data To Be Signed) document to be signed by the signing component.
    /// The 'data' is holding the document loaded into memory.
    /// 'FileName' is the name of the associated file.
    /// </summary>
    public class DataToBeSigned
    {
        private readonly byte[] _data;

        public SignatureFormat Format { get; set; }

        public string FileName { get; private set; }

        public DataToBeSigned(SignatureFormat format, byte[] data, string name)
        {
            Format = format;
            _data = data;
            FileName = name;
        }

        public byte[] GetData()
        {
            return (byte[])_data.Clone();
        }
    }
}
