using System.Text;
using Nemlogin.QualifiedSigning.SDK.Core.Enums;

namespace Nemlogin.QualifiedSigning.SDK.Core.Fundamental;

/// <summary>
/// Encapsulates the original SD (Signer's Document) to be signed by the signing component.
/// </summary>
public abstract class SignersDocument
{
    public SignersDocument() { }
                
    public DocumentFormat DocumentFormat { get; private set; }

    public SignersDocumentFile SignersDocumentFile { get; private set; }

    public SignProperties SignProperties { get; private set; }

    public SignersDocument(DocumentFormat signersDocumentFormat, SignersDocumentFile signersDocumentFile, SignProperties signProperties)
    {
        DocumentFormat = signersDocumentFormat;
        SignersDocumentFile = signersDocumentFile;
        SignProperties = signProperties;
    }

    public string DataAsText()
    {
        return Encoding.UTF8.GetString(SignersDocumentFile.GetData());
    }
}