using NemLoginSigningCore.Core;
using System;
using System.Linq;
using System.Xml.Serialization;

namespace NemLoginSigningXades.GeneratedSources
{
    public partial class SignTextType
    {
        public SignTextType WithPlainText(PlainTextType plainTextType)
        {
            Item = plainTextType;
            return this;
        }

        public SignTextType WithXMLDocument(XMLDocumentType xmlDocumentType)
        {
            this.Item = xmlDocumentType;
            return this;
        }

        public SignTextType WithHTMLDocument(HTMLDocumentType HTMLDocumentType)
        {
            this.Item = HTMLDocumentType;
            return this;
        }


        public SignTextType WithPDFDocument(PDFDocumentType pdfDocumentType)
        {
            this.Item = pdfDocumentType;
            return this;
        }

        public XMLDocumentType GetXMLDocument()
        {
            if (this.Item.GetType() == typeof(XMLDocumentType))
            {
                return (XMLDocumentType)this.Item;
            }

            return null;
        }

        public PlainTextType GetPlainTextDocument()
        {
            if (this.Item.GetType() == typeof(PlainTextType))
            {
                return (PlainTextType)this.Item;
            }

            return null;
        }

        public PDFDocumentType GetPDFDocument()
        {
            if (this.Item.GetType() == typeof(PDFDocumentType))
            {
                return (PDFDocumentType)this.Item;
            }

            return null;
        }

        public HTMLDocumentType GetHTMLDocument()
        {
            if (this.Item.GetType() == typeof(HTMLDocumentType))
            {
                return (HTMLDocumentType)this.Item;
            }

            return null;
        }

        public SignTextType WithProperties(SignProperties signProperties)
        {
            if (signProperties == null)
                throw new ArgumentNullException(nameof(signProperties));

            if (!signProperties.Any())
                return this;

            PropertyType[] properties = new PropertyType[signProperties.Count];

            int i = 0;

            foreach (var item in signProperties)
            {
                PropertyType propertyType = null;

                if (item.Value.Type == SignPropertyValue.SignPropertyValueType.StringValue)
                {
                    propertyType = new PropertyType { Key = item.Key, Item = item.Value.Value };
                }
                
                if (item.Value.Type == SignPropertyValue.SignPropertyValueType.BinaryValue)
                {
                    propertyType = new PropertyType { Key = item.Key, Item = Convert.FromBase64String(item.Value.Value) };
                }

                properties[i] = propertyType;
                i++;
            }

            this.Properties = properties;

            return this;
        }
    }
}
