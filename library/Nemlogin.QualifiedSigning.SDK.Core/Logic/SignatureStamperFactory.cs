using Nemlogin.QualifiedSigning.SDK.Core.Enums;
using Nemlogin.QualifiedSigning.SDK.Core.Utilities;

namespace Nemlogin.QualifiedSigning.SDK.Core.Logic;

public static class SignatureStamperFactory
{
    /// <summary>
    /// Factory for creating concrete implementations of the ISignatureStamper interface.
    /// Creation is based on if SignatureFormat is either XAdES or PAdES.
    /// </summary>
    /// <param name="dataToBeSignedFormat"></param>
    /// <returns></returns>
    public static ISignatureStamper Create(SignatureFormat dataToBeSignedFormat)
    {
        var signatureStampers = ReflectorLogic.GetClassesWithInterfaceType(typeof(ISignatureStamper));

        foreach (var item in signatureStampers)
        {
            var signatureStamper = Activator.CreateInstance(item) as ISignatureStamper;
            if (signatureStamper.CanSign(dataToBeSignedFormat))
            {
                return signatureStamper;
            }
        }

        throw new InvalidOperationException($"Could not create instance of type {nameof(ISignatureStamper)})");
    }
}