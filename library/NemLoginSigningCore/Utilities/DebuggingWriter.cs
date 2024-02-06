using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;

namespace NemLoginSigningCore.Utilities
{
    public static class DebuggingWriter
    {
        public static void WriteXMLDebugFormatted(string message, string xml)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xml);

            MemoryStream mStream = new MemoryStream();

            using (XmlTextWriter writer = new XmlTextWriter(mStream, Encoding.Unicode))
            using (StreamReader sReader = new StreamReader(mStream))
            {
                writer.Formatting = Formatting.Indented;

                xmlDocument.WriteContentTo(writer);
                writer.Flush();
                mStream.Flush();

                mStream.Position = 0;

                string formattedXml = sReader.ReadToEnd();

                DoWrite(message, formattedXml);
            }
        }

        public static void WriteString(string title, string output)
        {
            DoWrite(title, output);
        }

        private static void DoWrite(string title, string output)
        {
            Debug.WriteLine(string.Empty);
            Debug.WriteLine($"---------{title}---------");
            Debug.WriteLine(title);
            Debug.WriteLine(output);
            Debug.WriteLine("--------------------------------");
        }
    }
}