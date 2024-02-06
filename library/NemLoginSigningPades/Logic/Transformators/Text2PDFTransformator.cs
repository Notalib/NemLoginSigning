using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Xsl;

using Microsoft.Extensions.Logging;
using NemLoginSigningCore.Core;
using NemLoginSigningCore.Exceptions;
using NemLoginSigningCore.Format;

namespace NemLoginSigningPades.Logic.Transformators
{
    public class Text2PDFTransformator : PdfFormatTransformationService
    {
        public override bool CanTransform(Transformation transformation)
        {
            return transformation.SignatureFormat == SignatureFormat.PAdES &&
                transformation.SdFormat == DocumentFormat.TEXT;
        }

        protected override string GenerateHTML(TransformationContext ctx, ILogger logger)
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

                var streamReader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("NemLoginSigningPades.xslt.text-to-html.xsl"));

                XmlReader xsltReader = XmlReader.Create(streamReader);

                XslCompiledTransform xslt = new XslCompiledTransform();
                xslt.Load(xsltReader);

                StringWriter stringWriter = new StringWriter();

                XmlTextWriter writer = new XmlTextWriter(stringWriter);
                writer.Formatting = Formatting.Indented;

                XsltArgumentList argList = new XsltArgumentList();
                argList.AddParam("useMonoSpaceFont", string.Empty, plainTextSignersDocument.UseMonoSpaceFont.ToString().ToLower());

                xslt.Transform(document.CreateNavigator(), argList, writer, null);

                string html = stringWriter.ToString();

                logger.LogInformation("Initially transformed {Name} from TXT to HTML in {MilliSeconds} ms", plainTextSignersDocument.SignersDocumentFile.Name, sw.ElapsedMilliseconds);

                return html;
            }
            catch (Exception e)
            {
                string logMessage = "Error initially transforming {Name} from TXT to HTML";
                logger.LogError("Error initially transforming {Name} from TXT to HTML", plainTextSignersDocument.SignersDocumentFile.Name);
                throw new TransformationException(logMessage, ErrorCode.SDK007, e);
            }
        }
    }
}
