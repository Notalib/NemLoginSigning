using System.Text;

using Nemlogin.QualifiedSigning.SDK.Core.Enums;

namespace Nemlogin.QualifiedSigning.SDK.Core.Fundamental;

/// <summary>
///  Specification of SignersDocument for XML documents.
/// </summary>
public class XmlSignersDocument : SignersDocument
{
    public SignersDocumentFile XsltFile { get; private set; }

    public XmlSignersDocument(SignersDocumentFile signersDocumentFile, SignersDocumentFile xsltFile, SignProperties signProperties) : base(DocumentFormat.XML, signersDocumentFile, signProperties)
    {
        XsltFile = xsltFile;
    }

    public XmlSignersDocument(SignersDocumentFile signersDocumentFile, SignersDocumentFile xsltFile) : base(DocumentFormat.XML, signersDocumentFile, null)
    {
        XsltFile = xsltFile;
    }

    public string XsltAsText()
    {
        return Encoding.UTF8.GetString(XsltFile.GetData());
    }
}