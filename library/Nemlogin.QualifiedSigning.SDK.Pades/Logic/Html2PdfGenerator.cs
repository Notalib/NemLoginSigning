using System.Text;
using HtmlAgilityPack;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;

namespace Nemlogin.QualifiedSigning.SDK.Pades.Logic;

public class Html2PdfGenerator
{
    public byte[] GeneratePdfDocument(string inputHtml, TransformationPropertiesHandler propertiesHandler)
    {
        // Rectangle pageSize = propertiesHandler.GetPageSizeAndOrientation();
                    
        float pageMargin = propertiesHandler.GetPageMargin();
        float pageMarginInPoints = Utilities.MillimetersToPoints(pageMargin * 10);

        string bodyStyle = "body { font-family: " + propertiesHandler.GetBodyFont() + "; }"; 
        string monospaceStyle = "pre { font: " + propertiesHandler.GetMonospaceFont() + "; }";

        var doc = new HtmlDocument();
        doc.LoadHtml(inputHtml);

        HtmlNode headNode = doc.DocumentNode.SelectSingleNode("//head");
        HtmlNode htmlNode = doc.DocumentNode.SelectSingleNode("//html");

        if (headNode == null)
        {
            headNode = HtmlNode.CreateNode("<head><style>" + bodyStyle + " " + monospaceStyle + "</style></head>");
            htmlNode.PrependChild(headNode);
        }
        else
        {
            HtmlNode style = HtmlNode.CreateNode("<style>" + bodyStyle + " " + monospaceStyle + "</style>");
            headNode.PrependChild(style);
        }

        var modifiedHtml = doc.DocumentNode.InnerHtml;

        // Missing not closed meta tag which is valid html but not valid xHtml
        // Below is parsing the html with HtmlAgilityPack and fixing these tags
        string xHtml = CloseTagsToXhtmlCompliance(modifiedHtml);

        var pdfDocument = new Document(pageSize, pageMarginInPoints, pageMarginInPoints, pageMarginInPoints, pageMarginInPoints);

        using var ms = new MemoryStream();
        PdfWriter writer = PdfWriter.GetInstance(pdfDocument, ms);
        writer.SetPdfVersion(PdfWriter.PDF_VERSION_1_7);
        writer.SetDefaultColorspace(PdfName.DEVICERGB, PdfName.DEVICERGB);

        pdfDocument.Open();

        // propertiesHandler.ApplyColorProfile(writer);

        using (MemoryStream msXhtml = new MemoryStream(Encoding.UTF8.GetBytes(xHtml)))
        {
            var xmlWorkerHelper = XMLWorkerHelper.GetInstance();
                
            // xmlWorkerHelper.ParseXHtml(writer, pdfDocument, msXhtml, null, Encoding.UTF8, propertiesHandler.GetFontProvider());
        }

        pdfDocument.Close();

        return ms.ToArray();
    }

    private string CloseTagsToXhtmlCompliance(string html)
    {
        StringWriter writer = new StringWriter();

        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        doc.OptionWriteEmptyNodes = true;
        doc.Save(writer);

        return writer.ToString();
    }
}