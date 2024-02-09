using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;

namespace NemLoginSigningCore.Utilities
{
    /// <summary>
    /// Utility class for hash computation and canonicalization of xml the signature
    /// </summary>
    public static class CryptographyLogic
    {
        public static byte[] ComputeSha256Hash(byte[] data)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                return sha256Hash.ComputeHash(data);
            }
        }

        public static byte[] Canonicalize(byte[] dtbs)
        {
            var xmlDsigC14NTransform = new XmlDsigC14NTransform()
            {
                Algorithm = SignedXml.XmlDsigExcC14NTransformUrl
            };

            using (MemoryStream memoryStream = new MemoryStream(dtbs))
            {
                xmlDsigC14NTransform.LoadInput(memoryStream);
                var xml = xmlDsigC14NTransform.GetXml();
            }

            Type streamType = typeof(System.IO.Stream);
            MemoryStream result = (MemoryStream)xmlDsigC14NTransform.GetOutput(streamType);
            return result.ToArray();
        }
    }
}
