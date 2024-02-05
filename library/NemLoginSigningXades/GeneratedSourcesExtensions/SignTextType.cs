using System;
using System.Linq;

using NemLoginSigningCore.Core;

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
            Item = xmlDocumentType;
            return this;
        }

        public SignTextType WithHTMLDocument(HTMLDocumentType htmlDocumentType)
        {
            Item = htmlDocumentType;
            return this;
        }

        public SignTextType WithPDFDocument(PDFDocumentType pdfDocumentType)
        {
            Item = pdfDocumentType;
            return this;
        }

        public XMLDocumentType GetXMLDocument()
        {
            if (Item.GetType() == typeof(XMLDocumentType))
            {
                return (XMLDocumentType)Item;
            }

            return null;
        }

        public PlainTextType GetPlainTextDocument()
        {
            if (Item.GetType() == typeof(PlainTextType))
            {
                return (PlainTextType)Item;
            }

            return null;
        }

        public PDFDocumentType GetPDFDocument()
        {
            if (Item.GetType() == typeof(PDFDocumentType))
            {
                return (PDFDocumentType)Item;
            }

            return null;
        }

        public HTMLDocumentType GetHTMLDocument()
        {
            if (Item.GetType() == typeof(HTMLDocumentType))
            {
                return (HTMLDocumentType)Item;
            }

            return null;
        }

        public SignTextType WithProperties(SignProperties signProperties)
        {
            if (signProperties == null)
            {
                throw new ArgumentNullException(nameof(signProperties));
            }

            if (!signProperties.Any())
            {
                return this;
            }

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

            Properties = properties;

            return this;
        }
    }
}
