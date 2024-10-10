using System.Diagnostics;
using System.Reflection;
using System.Xml;
using System.Xml.Xsl;
using Nemlogin.QualifiedSigning.SDK.Core.Enums;
using Nemlogin.QualifiedSigning.SDK.Core.Exceptions;
using Nemlogin.QualifiedSigning.SDK.Core.Fundamental;

namespace Nemlogin.QualifiedSigning.SDK.Pades.Logic.Transformators;

public class Text2PDFTransformator : PdfFormatTransformationService
{
    public override bool CanTransform(Transformation transformation)
    {
        return transformation.SignatureFormat == SignatureFormat.PAdES &&
            transformation.SdFormat == DocumentFormat.TEXT;
    }

    protected override string GenerateHtml(TransformationContext ctx)
    {
        Stopwatch sw = Stopwatch.StartNew();

        PlainTextSignersDocument plainTextSignersDocument = (PlainTextSignersDocument)ctx.SignersDocument;

        try
        {
            XmlDocument document = new XmlDocument();
            XmlElement root = document.CreateElement("data");

            document.AppendChild(root);

            var textFile = plainTextSignersDocument.DataAsText();

            using (StringReader reader = new StringReader(textFile))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    XmlElement xmlelement = document.CreateElement(string.Empty, "line", string.Empty);
                    XmlText xmlText = document.CreateTextNode(line);
                    xmlelement.AppendChild(xmlText);
                    root.AppendChild(xmlelement);
                }
            }

            var streamReader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("Nemlogin.QualifiedSigning.SDK.Pades.xslt.text-to-html.xsl"));

            XmlReader xsltReader = XmlReader.Create(streamReader);

            XslCompiledTransform xslt = new XslCompiledTransform();
            xslt.Load(xsltReader);

            StringWriter stringWriter = new StringWriter();

            XmlTextWriter writer = new XmlTextWriter(stringWriter);
            writer.Formatting = Formatting.Indented;

            XsltArgumentList argList = new XsltArgumentList();
            argList.AddParam("useMonoSpaceFont", "", plainTextSignersDocument.UseMonoSpaceFont.ToString().ToLower());

            xslt.Transform(document.CreateNavigator(), argList, writer, null);

            string html = stringWriter.ToString();

            return html;
        }
        catch (Exception e)
        {
            string logMessage = $"Error initially transforming {plainTextSignersDocument.SignersDocumentFile.Name} from TXT to HTML";
            throw new TransformationException(logMessage, ErrorCode.SDK007, e);
        }
    }
}