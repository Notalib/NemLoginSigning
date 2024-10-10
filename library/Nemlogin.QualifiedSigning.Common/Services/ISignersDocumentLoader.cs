using Nemlogin.QualifiedSigning.SDK.Core.Fundamental;

namespace Nemlogin.QualifiedSigning.Common.Services;

/// <summary>
/// Interface defining the SignersDocumentLoader
/// </summary>
public interface ISignersDocumentLoader
{
    IEnumerable<SignersDocument> GetFiles();

    SignersDocument CreateSignersDocumentFromFile(string filePath);
}