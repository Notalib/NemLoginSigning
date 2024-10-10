using Nemlogin.QualifiedSigning.SDK.Core.Enums;
using Nemlogin.QualifiedSigning.SDK.Core.Fundamental;

namespace Nemlogin.QualifiedSigning.SDK.Pades.Logic;

/// <summary>
/// Encapsulates PAdES DTBS
/// </summary>
public class PadesDataToBeSigned : DataToBeSigned
{
    public PadesDataToBeSigned(byte[] data, string name) : base(SignatureFormat.PAdES, data, name) { }
}