using Nemlogin.QualifiedSigning.SDK.Core.Enums;

namespace Nemlogin.QualifiedSigning.SDK.Core.Fundamental;

/// <summary>
/// Encapsulates XAdES DTBS
/// </summary>
public class XadesDataToBeSigned : DataToBeSigned
{
    public XadesDataToBeSigned(byte[] data, string name) : base(SignatureFormat.XAdES, data, name) { }
}