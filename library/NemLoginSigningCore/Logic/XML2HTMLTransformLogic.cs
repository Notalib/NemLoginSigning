using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Xsl;

namespace NemLoginSigningCore.Logic
{
    /// <summary>
    /// Transformation from XML -> HTML.
    /// </summary>
    public class XML2HTMLTransformLogic
    {
        public string Transform(string xml, string xslt)
        {
            StringReader xmlStringReader = new StringReader(xml);
            StringReader xsltStringReader = new StringReader(xslt);

            XmlReader xmlReader = XmlReader.Create(xmlStringReader);
            XmlReader xsltReader = XmlReader.Create(xsltStringReader);

            XslCompiledTransform xsltTransformer = new XslCompiledTransform();
            xsltTransformer.Load(xsltReader);

            StringBuilder output = new StringBuilder();

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.UTF8;

            using (XmlWriter writer = XmlWriter.Create(output, settings))
            {
                xsltTransformer.Transform(xmlReader, writer);
            }

            return output.ToString();
        }
    }
}
