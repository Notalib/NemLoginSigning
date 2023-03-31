using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using NemLoginSigningXades.GeneratedSources;
using NemLoginSigningCore.Utilities;

namespace NemLoginSigningXades.Util
{
    /// <summary>
    /// Logic for Serializing/Deserializing XML.
    /// </summary>
    public static class XMLSerializer
    {
        public static byte[] Serialize(SignedDocumentType obj)
        {
            MemoryStream memoryStream = new MemoryStream();

            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();

            // Ads namespaces needed for signing client to validate
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "http://dk.gov.certifikat/nemlogin/v0.0.1#");
            namespaces.Add("ds", "http://www.w3.org/2000/09/xmldsig#");

            xmlWriterSettings.Encoding = new UTF8Encoding();

            using (XmlWriter xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings))
            {
                XmlSerializer xmlSerialiser = new XmlSerializer(typeof(SignedDocumentType));
                xmlSerialiser.Serialize(xmlWriter, obj, namespaces);
            }

            string xmlString = Encoding.UTF8.GetString(memoryStream.ToArray());
            DebuggingWriter.WriteXMLDebugFormatted(nameof(SignedDocumentType), xmlString);

            return memoryStream.ToArray();
        }

        public static byte[] Serialize(SignatureType obj)
        {
            return Serialize<SignatureType>(obj);
        }

        public static byte[] Serialize(SignTextType obj)
        {
            MemoryStream memoryStream = new MemoryStream();

            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "http://dk.gov.certifikat/nemlogin/v0.0.1#");

            xmlWriterSettings.Encoding = new UTF8Encoding();

            using (XmlWriter xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings))
            {
                XmlSerializer xmlSerialiser = new XmlSerializer(typeof(SignTextType));
                xmlSerialiser.Serialize(xmlWriter, obj, namespaces);
            }

            string xmlString = Encoding.UTF8.GetString(memoryStream.ToArray());
            DebuggingWriter.WriteXMLDebugFormatted(nameof(SignTextType), xmlString);

            return memoryStream.ToArray();
        }

        private static byte[] Serialize<T>(T obj)
        {
            MemoryStream memoryStream = new MemoryStream();

            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "http://dk.gov.certifikat/nemlogin/v0.0.1#");

            xmlWriterSettings.Encoding = new UTF8Encoding();

            using (XmlWriter xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings))
            {
                XmlSerializer xmlSerialiser = new XmlSerializer(typeof(T));
                xmlSerialiser.Serialize(xmlWriter, obj, namespaces);
            }

            string xmlString = Encoding.UTF8.GetString(memoryStream.ToArray());
            DebuggingWriter.WriteXMLDebugFormatted(nameof(T), xmlString);

            return memoryStream.ToArray();
        }

        public static T Deserialize<T>(byte[] data)
        {
            MemoryStream memoryStream = new MemoryStream(data);

            XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();

            using (XmlReader xmlReader = XmlReader.Create(memoryStream, xmlReaderSettings))
            {
                XmlSerializer xmlSerialiser = new XmlSerializer(typeof(T));

                var deserializedObject = xmlSerialiser.Deserialize(xmlReader);

                string xmlString = Encoding.UTF8.GetString(memoryStream.ToArray());
                DebuggingWriter.WriteXMLDebugFormatted(nameof(T), xmlString);

                return (T)deserializedObject;
            }
        }
    }
}