﻿using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Collections;
using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.Crypto;
using ISigner = PdfSharp.Pdf.Signatures.ISigner;

namespace Nemlogin.QualifiedSigning.SDK.Pades.Logic;

public class SigningSigner : ISigner
{
    private X509Certificate2 Certificate { get; set; }
    private X509Certificate2Collection CertificateChain { get; }
    private AsymmetricKeyParameter Key { get; }

    public string GetName()
    {
        return Certificate.GetNameInfo(X509NameType.SimpleName, false);
    }

    public SigningSigner(Tuple<X509Certificate2, X509Certificate2Collection> certificateData, AsymmetricKeyParameter key)
    {
        Certificate = certificateData.Item1;
        CertificateChain = certificateData.Item2;
        Key = key;
    }

    public byte[] GetSignedCms(Stream rangedStream, int pdfVersion)
    {
        rangedStream.Position = 0;

        CmsSignedDataGenerator signedDataGenerator = new CmsSignedDataGenerator();

        var cert = DotNetUtilities.FromX509Certificate(Certificate);
        var allCerts = CertificateChain.OfType<X509Certificate2>().Select(item => DotNetUtilities.FromX509Certificate(item));

        var store = CollectionUtilities.CreateStore(allCerts);

        signedDataGenerator.AddSigner(Key, cert, GetProperDigestAlgorithm(pdfVersion));
        signedDataGenerator.AddCertificates(store);

        CmsProcessableInputStream msg = new CmsProcessableInputStream(rangedStream);

        CmsSignedData signedData = signedDataGenerator.Generate(msg, false);

        return signedData.GetEncoded();
    }

    /// <summary>
    /// adbe.pkcs7.detached supported algorithms: SHA1 (PDF 1.3), SHA256 (PDF 1.6), SHA384/SHA512/RIPEMD160 (PDF 1.7)
    /// </summary>
    /// <param name="pdfVersion">PDF version as int</param>
    /// <returns></returns>
    private string GetProperDigestAlgorithm(int pdfVersion)
    {
        switch (pdfVersion)
        {
            case int when pdfVersion >= 17:
                return CmsSignedDataGenerator.DigestSha512;
            case int when pdfVersion == 16:
                return CmsSignedDataGenerator.DigestSha256;
            case int when pdfVersion >= 13:
            default:
                return CmsSignedDataGenerator.DigestSha256; // SHA1 is obsolete, use at least SHA256
        }
    }
}