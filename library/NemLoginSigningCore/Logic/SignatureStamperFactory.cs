using System;

using NemLoginSigningCore.Format;
using NemLoginSigningCore.Utilities;

namespace NemLoginSigningCore.Logic
{
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
            System.Collections.Generic.IEnumerable<Type> signatureStampers = ReflectorLogic.GetClassesWithInterfaceType(typeof(ISignatureStamper));

            foreach (Type item in signatureStampers)
            {
                ISignatureStamper signatureStamper = Activator.CreateInstance(item) as ISignatureStamper;
                if (signatureStamper.CanSign(dataToBeSignedFormat))
                {
                    return signatureStamper;
                }
            }

            throw new InvalidOperationException($"Could not create instance of type {nameof(ISignatureStamper)})");
        }
    }
}
