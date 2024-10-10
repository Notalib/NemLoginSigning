using System;
using System.IO;

using iTextSharp.text.pdf;
using NemLoginSigningCore.Core;
using NemLoginSigningCore.Format;
using NemLoginSigningCore.Logic;

namespace NemLoginSigningPades.Logic
{
    public class PDFSourceAttacher : ISourceAttacher
    {
        public void Attach(TransformationContext context)
        {
            ArgumentNullException.ThrowIfNull(context);

            XmlSignersDocument xmlSignersDocument = context.SignersDocument as XmlSignersDocument;

            ArgumentNullException.ThrowIfNull(context?.SignersDocument);

            byte[] pdfDocumentData = context.DataToBeSigned.GetData();

            MemoryStream ms = new MemoryStream();
            PdfReader reader = new PdfReader(pdfDocumentData);

            PdfStamper stamper = new PdfStamper(reader, ms);

            PdfFileSpecification xmlFileSpecification = PdfFileSpecification.FileEmbedded(stamper.Writer, null, xmlSignersDocument.SignersDocumentFile.Name, xmlSignersDocument.SignersDocumentFile.GetData());
            PdfFileSpecification xslFileSpecification = PdfFileSpecification.FileEmbedded(stamper.Writer, null, xmlSignersDocument.XsltFile.Name, xmlSignersDocument.XsltFile.GetData());

            stamper.AddFileAttachment("NemLog-In XML Attachment", xmlFileSpecification);
            stamper.AddFileAttachment("NemLog-In XSL Attachment", xslFileSpecification);

            stamper.Close();

            context.DataToBeSigned = new PadesDataToBeSigned(ms.ToArray(), context.DataToBeSigned.FileName);
        }

        public bool CanAttach(Transformation transformation)
        {
            ArgumentNullException.ThrowIfNull(transformation);

            return (transformation.SdFormat == DocumentFormat.XML && transformation.SignatureFormat == SignatureFormat.PAdES);
        }
    }
}
