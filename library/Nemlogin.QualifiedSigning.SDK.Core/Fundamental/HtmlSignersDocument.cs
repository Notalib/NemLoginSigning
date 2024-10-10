using Nemlogin.QualifiedSigning.SDK.Core.Enums;

namespace Nemlogin.QualifiedSigning.SDK.Core.Fundamental;

/// <summary>
/// Specification of SignersDocument for HTML documents.
/// </summary>
public class HtmlSignersDocument : SignersDocument
{
    public HtmlSignersDocument(SignersDocumentFile signersDocumentFile) : this(signersDocumentFile, null) { } 
        

    public HtmlSignersDocument(SignersDocumentFile signersDocumentFile, SignProperties signProperties) :
        base(DocumentFormat.HTML, signersDocumentFile, signProperties) { }
}