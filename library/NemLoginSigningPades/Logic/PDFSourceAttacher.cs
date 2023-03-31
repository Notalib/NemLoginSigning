using iTextSharp.text.pdf;
using NemLoginSigningCore.Core;
using NemLoginSigningCore.Format;
using NemLoginSigningCore.Logic;
using System;
using System.IO;
using System.Net;

namespace NemLoginSigningPades.Logic
{
    public class PDFSourceAttacher : ISourceAttacher
    {
        public void Attach(TransformationContext ctx)
        {
            if (ctx == null)
            {
                throw new ArgumentNullException(nameof(ctx));
            }

            var xmlSignersDocument = ctx.SignersDocument as XmlSignersDocument;

            if (xmlSignersDocument == null)
            {
                throw new ArgumentNullException($"{nameof(xmlSignersDocument)} can not be null");
            }

            var pdfDocumentData = ctx.DataToBeSigned.GetData();

            MemoryStream ms = new MemoryStream();
            PdfReader reader = new PdfReader(pdfDocumentData);

            PdfStamper stamper = new PdfStamper(reader, ms);

            var xmlFileSpecification = PdfFileSpecification.FileEmbedded(stamper.Writer, null, xmlSignersDocument.SignersDocumentFile.Name, xmlSignersDocument.SignersDocumentFile.GetData());
            var xslFileSpecification = PdfFileSpecification.FileEmbedded(stamper.Writer, null, xmlSignersDocument.XsltFile.Name, xmlSignersDocument.XsltFile.GetData());

            stamper.AddFileAttachment("NemLog-In XML Attachment", xmlFileSpecification);
            stamper.AddFileAttachment("NemLog-In XSL Attachment", xslFileSpecification);

            stamper.Close();
            
            ctx.DataToBeSigned = new PadesDataToBeSigned(ms.ToArray(), ctx.DataToBeSigned.FileName);
        }

        public bool CanAttach(Transformation transformation)
        {
            if (transformation == null)
            {
                throw new ArgumentNullException(nameof(transformation));
            }

            return (transformation.SdFormat == DocumentFormat.XML && transformation.SignatureFormat == SignatureFormat.PAdES);
        }
    }
}