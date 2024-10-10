using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;

namespace Nemlogin.QualifiedSigning.SDK.Pades.Logic;

  public static class DigestAlgorithms
  {
    public const string SHA1 = "SHA-1";
    public const string SHA256 = "SHA-256";
    public const string SHA384 = "SHA-384";
    public const string SHA512 = "SHA-512";
    public const string RIPEMD160 = "RIPEMD160";
    private static readonly Dictionary<string, string> digestNames = new Dictionary<string, string>();
    private static readonly Dictionary<string, string> allowedDigests = new Dictionary<string, string>();

    static DigestAlgorithms()
    {
      DigestAlgorithms.digestNames["1.2.840.113549.2.5"] = "MD5";
      DigestAlgorithms.digestNames["1.2.840.113549.2.2"] = "MD2";
      DigestAlgorithms.digestNames["1.3.14.3.2.26"] = nameof (SHA1);
      DigestAlgorithms.digestNames["2.16.840.1.101.3.4.2.4"] = "SHA224";
      DigestAlgorithms.digestNames["2.16.840.1.101.3.4.2.1"] = nameof (SHA256);
      DigestAlgorithms.digestNames["2.16.840.1.101.3.4.2.2"] = nameof (SHA384);
      DigestAlgorithms.digestNames["2.16.840.1.101.3.4.2.3"] = nameof (SHA512);
      DigestAlgorithms.digestNames["1.3.36.3.2.2"] = "RIPEMD128";
      DigestAlgorithms.digestNames["1.3.36.3.2.1"] = nameof (RIPEMD160);
      DigestAlgorithms.digestNames["1.3.36.3.2.3"] = "RIPEMD256";
      DigestAlgorithms.digestNames["1.2.840.113549.1.1.4"] = "MD5";
      DigestAlgorithms.digestNames["1.2.840.113549.1.1.2"] = "MD2";
      DigestAlgorithms.digestNames["1.2.840.113549.1.1.5"] = nameof (SHA1);
      DigestAlgorithms.digestNames["1.2.840.113549.1.1.14"] = "SHA224";
      DigestAlgorithms.digestNames["1.2.840.113549.1.1.11"] = nameof (SHA256);
      DigestAlgorithms.digestNames["1.2.840.113549.1.1.12"] = nameof (SHA384);
      DigestAlgorithms.digestNames["1.2.840.113549.1.1.13"] = nameof (SHA512);
      DigestAlgorithms.digestNames["1.2.840.113549.2.5"] = "MD5";
      DigestAlgorithms.digestNames["1.2.840.113549.2.2"] = "MD2";
      DigestAlgorithms.digestNames["1.2.840.10040.4.3"] = nameof (SHA1);
      DigestAlgorithms.digestNames["2.16.840.1.101.3.4.3.1"] = "SHA224";
      DigestAlgorithms.digestNames["2.16.840.1.101.3.4.3.2"] = nameof (SHA256);
      DigestAlgorithms.digestNames["2.16.840.1.101.3.4.3.3"] = nameof (SHA384);
      DigestAlgorithms.digestNames["2.16.840.1.101.3.4.3.4"] = nameof (SHA512);
      DigestAlgorithms.digestNames["1.3.36.3.3.1.3"] = "RIPEMD128";
      DigestAlgorithms.digestNames["1.3.36.3.3.1.2"] = nameof (RIPEMD160);
      DigestAlgorithms.digestNames["1.3.36.3.3.1.4"] = "RIPEMD256";
      DigestAlgorithms.digestNames["1.2.643.2.2.9"] = "GOST3411";
      DigestAlgorithms.allowedDigests["MD2"] = "1.2.840.113549.2.2";
      DigestAlgorithms.allowedDigests["MD-2"] = "1.2.840.113549.2.2";
      DigestAlgorithms.allowedDigests["MD5"] = "1.2.840.113549.2.5";
      DigestAlgorithms.allowedDigests["MD-5"] = "1.2.840.113549.2.5";
      DigestAlgorithms.allowedDigests[nameof (SHA1)] = "1.3.14.3.2.26";
      DigestAlgorithms.allowedDigests["SHA-1"] = "1.3.14.3.2.26";
      DigestAlgorithms.allowedDigests["SHA224"] = "2.16.840.1.101.3.4.2.4";
      DigestAlgorithms.allowedDigests["SHA-224"] = "2.16.840.1.101.3.4.2.4";
      DigestAlgorithms.allowedDigests[nameof (SHA256)] = "2.16.840.1.101.3.4.2.1";
      DigestAlgorithms.allowedDigests["SHA-256"] = "2.16.840.1.101.3.4.2.1";
      DigestAlgorithms.allowedDigests[nameof (SHA384)] = "2.16.840.1.101.3.4.2.2";
      DigestAlgorithms.allowedDigests["SHA-384"] = "2.16.840.1.101.3.4.2.2";
      DigestAlgorithms.allowedDigests[nameof (SHA512)] = "2.16.840.1.101.3.4.2.3";
      DigestAlgorithms.allowedDigests["SHA-512"] = "2.16.840.1.101.3.4.2.3";
      DigestAlgorithms.allowedDigests["RIPEMD128"] = "1.3.36.3.2.2";
      DigestAlgorithms.allowedDigests["RIPEMD-128"] = "1.3.36.3.2.2";
      DigestAlgorithms.allowedDigests[nameof (RIPEMD160)] = "1.3.36.3.2.1";
      DigestAlgorithms.allowedDigests["RIPEMD-160"] = "1.3.36.3.2.1";
      DigestAlgorithms.allowedDigests["RIPEMD256"] = "1.3.36.3.2.3";
      DigestAlgorithms.allowedDigests["RIPEMD-256"] = "1.3.36.3.2.3";
      DigestAlgorithms.allowedDigests["GOST3411"] = "1.2.643.2.2.9";
    }

    public static IDigest GetMessageDigestFromOid(string digestOid)
    {
      return DigestUtilities.GetDigest(digestOid);
    }

    public static IDigest GetMessageDigest(string hashAlgorithm)
    {
      return DigestUtilities.GetDigest(hashAlgorithm);
    }

    public static byte[] Digest(Stream data, string hashAlgorithm)
    {
      IDigest messageDigest = DigestAlgorithms.GetMessageDigest(hashAlgorithm);
      return DigestAlgorithms.Digest(data, messageDigest);
    }

    public static byte[] Digest(Stream data, IDigest messageDigest)
    {
      byte[] numArray = new byte[8192];
      int length;
      while ((length = data.Read(numArray, 0, numArray.Length)) > 0)
        messageDigest.BlockUpdate(numArray, 0, length);
      byte[] output = new byte[messageDigest.GetDigestSize()];
      messageDigest.DoFinal(output, 0);
      return output;
    }

    public static string GetDigest(string oid)
    {
      string str;
      return DigestAlgorithms.digestNames.TryGetValue(oid, out str) ? str : oid;
    }

    public static string GetAllowedDigests(string name)
    {
      string allowedDigests;
      DigestAlgorithms.allowedDigests.TryGetValue(name.ToUpperInvariant(), out allowedDigests);
      return allowedDigests;
    }

    public static byte[] Digest(string algo, byte[] b, int offset, int len)
    {
      return DigestAlgorithms.Digest(DigestUtilities.GetDigest(algo), b, offset, len);
    }

    public static byte[] Digest(string algo, byte[] b)
    {
      return DigestAlgorithms.Digest(DigestUtilities.GetDigest(algo), b, 0, b.Length);
    }

    public static byte[] Digest(IDigest d, byte[] b, int offset, int len)
    {
      d.BlockUpdate(b, offset, len);
      byte[] output = new byte[d.GetDigestSize()];
      d.DoFinal(output, 0);
      return output;
    }

    public static byte[] Digest(IDigest d, byte[] b) => DigestAlgorithms.Digest(d, b, 0, b.Length);
  }