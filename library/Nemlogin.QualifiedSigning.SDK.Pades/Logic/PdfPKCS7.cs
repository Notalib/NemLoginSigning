using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Esf;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace Nemlogin.QualifiedSigning.SDK.Pades.Logic;

public sealed class PdfPkcs7
{
    private SignaturePolicyIdentifier signaturePolicyIdentifier = null;

    private int version = 1;
    private int signerversion = 1;
    private string digestAlgorithmOid;
    private IDigest messageDigest;
    private Dictionary<string, object> digestalgos;
    private string digestEncryptionAlgorithmOid;
    private byte[] externalDigest;
    private byte[] externalRSAdata;
    private ISigner sig;
    private byte[] digest;
    private byte[] RSAdata;
    private List<X509Certificate> certs;
    private X509Certificate signCert;
    private ICollection<X509Crl> crls;


    internal IDigest GetHashClass() => DigestUtilities.GetDigest(this.GetHashAlgorithm());

    public string GetHashAlgorithm() => DigestAlgorithms.GetDigest(this.digestAlgorithmOid);

    public PdfPkcs7(
        ICipherParameters privateKey,
        ICollection<X509Certificate> certChain,
        string hashAlgorithm,
        bool hasRsaData)
    {
        digestAlgorithmOid = DigestAlgorithms.GetAllowedDigests(hashAlgorithm);
        if (digestAlgorithmOid == null)
            throw new ArgumentException($"unknown.hash.algorithm.1 {(object)hashAlgorithm}");
        version = this.signerversion = 1;
        certs = new List<X509Certificate>(certChain);
        crls = new List<X509Crl>();
        digestalgos = new Dictionary<string, object>
        {
            [digestAlgorithmOid] = null
        };
        signCert = certs[0];
        switch (privateKey)
        {
            case null:
                if (hasRsaData)
                {
                    RSAdata = Array.Empty<byte>();
                    messageDigest = this.GetHashClass();
                }
                if (privateKey == null)
                    break;
                sig = this.InitSignature(privateKey);
                break;
            case RsaKeyParameters _:
                digestEncryptionAlgorithmOid = "1.2.840.113549.1.1.1";
                goto case null;
            case DsaKeyParameters _:
                digestEncryptionAlgorithmOid = "1.2.840.10040.4.1";
                goto case null;
            default:
                throw new ArgumentException($"unknown.key.algorithm.1 {(object)privateKey.ToString()}");
        }
    }

    public string GetDigestAlgorithm()
    {
        return this.GetHashAlgorithm() + "with" + this.GetEncryptionAlgorithm();
    }

    public string GetEncryptionAlgorithm()
    {
        return EncryptionAlgorithms.GetAlgorithm(this.digestEncryptionAlgorithmOid) ?? this.digestEncryptionAlgorithmOid;
    }

    private ISigner InitSignature(ICipherParameters key)
    {
        ISigner signer = SignerUtilities.GetSigner(this.GetDigestAlgorithm());
        signer.Init(true, key);
        return signer;
    }

    public byte[] GetAuthenticatedAttributeBytes(
        byte[] secondDigest,
        byte[] ocsp,
        ICollection<byte[]> crlBytes,
        CryptoStandard sigType)
    {
        return this.GetAuthenticatedAttributeSet(secondDigest, ocsp, crlBytes, sigType).GetEncoded("DER");
    }

    private DerSet GetAuthenticatedAttributeSet(
      byte[] secondDigest,
      byte[] ocsp,
      ICollection<byte[]> crlBytes,
      CryptoStandard sigType)
    {
        Asn1EncodableVector v1 = new(Array.Empty<Asn1Encodable>());
        v1.Add(new DerSequence(new Asn1EncodableVector(Array.Empty<Asn1Encodable>())
      {
          new Asn1Encodable[1]
          {
              new DerObjectIdentifier("1.2.840.113549.1.9.3")
          },
          new Asn1Encodable[1]
          {
              new DerSet(new DerObjectIdentifier("1.2.840.113549.1.7.1"))
          }
      }));
        v1.Add(new DerSequence(new Asn1EncodableVector(Array.Empty<Asn1Encodable>())
      {
          new Asn1Encodable[1]
          {
              new DerObjectIdentifier("1.2.840.113549.1.9.4")
          },
          new Asn1Encodable[1]
          {
              new DerSet(new DerOctetString(secondDigest))
          }
      }));
        bool flag = false;
        if (crlBytes != null)
        {
            foreach (byte[] crlByte in crlBytes)
            {
                if (crlByte != null)
                {
                    flag = true;
                    break;
                }
            }
        }
        if (ocsp != null | flag)
        {
            Asn1EncodableVector v2 = new(Array.Empty<Asn1Encodable>());
            v2.Add(new DerObjectIdentifier("1.2.840.113583.1.1.8"));
            Asn1EncodableVector v3 = new(Array.Empty<Asn1Encodable>());
            if (flag)
            {
                Asn1EncodableVector v4 = new(Array.Empty<Asn1Encodable>());
                foreach (byte[] crlByte in crlBytes)
                {
                    if (crlByte != null)
                    {
                        Asn1InputStream asn1InputStream = new(crlByte);
                        v4.Add(asn1InputStream.ReadObject());
                    }
                }
                v3.Add(new DerTaggedObject(true, 0, new DerSequence(v4)));
            }
            if (ocsp != null)
            {
                DerOctetString derOctetString = new(ocsp);
                Asn1EncodableVector v5 = new(Array.Empty<Asn1Encodable>());
                Asn1EncodableVector v6 = new(Array.Empty<Asn1Encodable>());
                v6.Add(OcspObjectIdentifiers.PkixOcspBasic);
                v6.Add(derOctetString);
                DerEnumerated derEnumerated = new(0);
                v5.Add(new DerSequence(new Asn1EncodableVector(Array.Empty<Asn1Encodable>())
          {
              new Asn1Encodable[1] { derEnumerated },
              new Asn1Encodable[1]
              {
                  new DerTaggedObject(true, 0, new DerSequence(v6))
              }
          }));
                v3.Add(new DerTaggedObject(true, 1, new DerSequence(v5)));
            }
            v2.Add(new DerSet(new DerSequence(v3)));
            v1.Add(new DerSequence(v2));
        }

        if (sigType == CryptoStandard.CADES)
        {
            Asn1EncodableVector v7 = new(Array.Empty<Asn1Encodable>());
            v7.Add(new DerObjectIdentifier("1.2.840.113549.1.9.16.2.47"));
            Asn1EncodableVector v8 = new(Array.Empty<Asn1Encodable>());
            if (!DigestAlgorithms.GetAllowedDigests("SHA-256").Equals(digestAlgorithmOid))
            {
                AlgorithmIdentifier algorithmIdentifier = new(new DerObjectIdentifier(digestAlgorithmOid));
                v8.Add(algorithmIdentifier);
            }
            byte[] str = DigestAlgorithms.Digest(this.GetHashAlgorithm(), signCert.GetEncoded());
            v8.Add(new DerOctetString(str));
            v7.Add(new DerSet(new DerSequence(new DerSequence(new DerSequence(v8)))));
            v1.Add(new DerSequence(v7));
        }

        if (signaturePolicyIdentifier != null)
        {
            v1.Add(new Org.BouncyCastle.Asn1.Cms.Attribute(PkcsObjectIdentifiers.IdAAEtsSigPolicyID, new DerSet(signaturePolicyIdentifier)));
        }

        return new DerSet(v1);
    }

    public void SetExternalDigest(
        byte[] digest,
        byte[] rsaData,
        string digestEncryptionAlgorithm)
    {
        externalDigest = digest;
        externalRSAdata = rsaData;
        switch (digestEncryptionAlgorithm)
        {
            case null:
                break;
            case "RSA":
                digestEncryptionAlgorithmOid = "1.2.840.113549.1.1.1";
                break;
            case "DSA":
                digestEncryptionAlgorithmOid = "1.2.840.10040.4.1";
                break;
            case "ECDSA":
                digestEncryptionAlgorithmOid = "1.2.840.10045.2.1";
                break;
            default:
                throw new ArgumentException($"unknown.key.algorithm.1 {(object)digestEncryptionAlgorithm}");
        }
    }

    public byte[] GetEncodedPKCS7(
      byte[] secondDigest,
      CryptoStandard sigtype)
    {
        if (externalDigest != null)
        {
            digest = externalDigest;
            if (RSAdata != null)
                RSAdata = externalRSAdata;
        }
        else if (externalRSAdata != null && RSAdata != null)
        {
            RSAdata = externalRSAdata;
            sig.BlockUpdate(RSAdata, 0, RSAdata.Length);
            digest = sig.GenerateSignature();
        }
        else
        {
            if (RSAdata != null)
            {
                RSAdata = new byte[messageDigest.GetDigestSize()];
                messageDigest.DoFinal(RSAdata, 0);
                sig.BlockUpdate(RSAdata, 0, RSAdata.Length);
            }
            digest = sig.GenerateSignature();
        }
        Asn1EncodableVector v1 = new(Array.Empty<Asn1Encodable>());
        foreach (string key in digestalgos.Keys)
            v1.Add(new DerSequence(new Asn1EncodableVector(Array.Empty<Asn1Encodable>())
        {
            new Asn1Encodable[1]
            {
                new DerObjectIdentifier(key)
            },
            new Asn1Encodable[1] { DerNull.Instance }
        }));
        Asn1EncodableVector v2 = new(Array.Empty<Asn1Encodable>()) { new DerObjectIdentifier("1.2.840.113549.1.7.1") };
        if (this.RSAdata != null)
            v2.Add(new DerTaggedObject(0, new DerOctetString(this.RSAdata)));
        DerSequence derSequence = new(v2);
        Asn1EncodableVector v3 = new(Array.Empty<Asn1Encodable>());
        foreach (X509Certificate cert in certs)
        {
            Asn1InputStream asn1InputStream = new(new MemoryStream(cert.GetEncoded()));
            v3.Add(asn1InputStream.ReadObject());
        }
        DerSet derSet = new(v3);
        Asn1EncodableVector v4 = new(Array.Empty<Asn1Encodable>())
      {
          new DerInteger(signerversion),
          new DerSequence(new Asn1EncodableVector(Array.Empty<Asn1Encodable>())
          {
              new Asn1Encodable[1]
              {
                  GetIssuer(signCert.GetTbsCertificate())
              },
              new Asn1Encodable[1]
              {
                  new DerInteger(signCert.SerialNumber)
              }
          }),
          new DerSequence(new Asn1EncodableVector(Array.Empty<Asn1Encodable>())
          {
              new Asn1Encodable[1]
              {
                  new DerObjectIdentifier(digestAlgorithmOid)
              },
              new Asn1Encodable[1] { DerNull.Instance }
          })
      };
        if (secondDigest != null)
            v4.Add(new DerTaggedObject(false, 0, GetAuthenticatedAttributeSet(secondDigest, null, null, sigtype)));
        v4.Add(new DerSequence(new Asn1EncodableVector(Array.Empty<Asn1Encodable>())
      {
          new Asn1Encodable[1]
          {
              new DerObjectIdentifier(digestEncryptionAlgorithmOid)
          },
          new Asn1Encodable[1] { DerNull.Instance }
      }));
        v4.Add(new DerOctetString(digest));

        Asn1EncodableVector v6 = new(Array.Empty<Asn1Encodable>())
      {
          new DerInteger(this.version),
          new DerSet(v1),
          derSequence,
          new DerTaggedObject(false, 0, derSet),
          new DerSet(new DerSequence(v4))
      };
        Asn1EncodableVector v7 = new(Array.Empty<Asn1Encodable>());
        v7.Add(new DerObjectIdentifier("1.2.840.113549.1.7.2"));
        v7.Add(new DerTaggedObject(0, new DerSequence(v6)));
        MemoryStream os = new();
        Asn1OutputStream asn1OutputStream = Asn1OutputStream.Create(os);
        asn1OutputStream.WriteObject(new DerSequence(v7));
        asn1OutputStream.Close();
        return os.ToArray();
    }

    public static Asn1Object GetIssuer(byte[] enc)
    {
        Asn1Sequence asn1Sequence = (Asn1Sequence)new Asn1InputStream((Stream)new MemoryStream(enc)).ReadObject();
        return (Asn1Object)asn1Sequence[asn1Sequence[0] is Asn1TaggedObject ? 3 : 2];
    }
}

public enum CryptoStandard
{
    CMS,
    CADES,
}
