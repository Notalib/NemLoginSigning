using System;
using System.IO;
using System.Linq;
using System.Reflection;

using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using NemLoginSigningCore.Core;

namespace NemLoginSigningPades.Logic
{
    public class TransformationPropertiesHandler
    {
        public const string KEY_PREFIX = "nemlogin.signing.pdf-generator.";
        public const string KEY_COLOR_PROFILE = KEY_PREFIX + "color-profile";
        public const string KEY_FONTS = KEY_PREFIX + "fonts";
        public const string KEY_FONT_NAME = KEY_PREFIX + "font[x].name";
        public const string KEY_FONT_PATH = KEY_PREFIX + "font[x].path";
        public const string KEY_BODY_FONT = KEY_PREFIX + "body-font";
        public const string KEY_MONOSPACE_FONT = KEY_PREFIX + "monospace-font";
        public const string KEY_PAGE_SIZE = KEY_PREFIX + "page-size";
        public const string KEY_PAGE_ORIENTATION = KEY_PREFIX + "page-orientation";
        public const string KEY_PAGE_MARGIN = KEY_PREFIX + "page-margin";

        private const string DEFAULT_COLOR_PROFILE = "NemLoginSigningPades.Resources.sRGB.icc";

        private readonly TransformationProperties _properties;

        public TransformationPropertiesHandler(TransformationProperties properties)
        {
            _properties = properties;
        }

        public string GetBodyFont()
        {
            return GetProperty(KEY_BODY_FONT, "medium Helvetica");
        }

        public string GetMonospaceFont()
        {
            return GetProperty(KEY_MONOSPACE_FONT, "medium Courier");
        }

        private string GetProperty(string propertyKey, string defaultValue)
        {
            return _properties.ContainsKey(propertyKey) ? _properties[propertyKey] : defaultValue;
        }

        public Rectangle GetPageSizeAndOrientation()
        {
            Rectangle pageLayout = PageSize.GetRectangle(GetProperty(KEY_PAGE_SIZE, "a4"));

            if (GetProperty(KEY_PAGE_ORIENTATION, "portrait") == "landscape")
            {
                return pageLayout.Rotate();
            }

            return pageLayout;
        }

        public string GetPageOrientation()
        {
            return GetProperty(KEY_PAGE_ORIENTATION, "portrait");
        }

        public float GetPageMargin()
        {
            if (float.TryParse(GetProperty(KEY_PAGE_MARGIN, "1"), out float outResult))
            {
                return outResult;
            }

            return default(float);
        }

        public void ApplyColorProfile(PdfWriter pdfWriter)
        {
            string colorProfilePath = GetProperty(KEY_COLOR_PROFILE, "default");
            byte[] colorProfileByteStream = null;

            if (string.Equals(colorProfilePath, "default", StringComparison.OrdinalIgnoreCase))
            {
                using (Stream iccStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(DEFAULT_COLOR_PROFILE))
                {
                    colorProfileByteStream = new byte[iccStream.Length];
                    iccStream.Read(colorProfileByteStream, 0, colorProfileByteStream.Length);
                }
            }
            else if (!string.Equals(colorProfilePath, "none", StringComparison.OrdinalIgnoreCase))
            {
                colorProfileByteStream = File.ReadAllBytes(colorProfilePath);
            }

            if (colorProfileByteStream != null)
            {
                ICC_Profile iccProfile = ICC_Profile.GetInstance(colorProfileByteStream);
                pdfWriter.SetOutputIntents(null, null, null, null, iccProfile);
            }
        }

        public IFontProvider GetFontProvider()
        {
            XMLWorkerFontProvider fontProvider = null;

            string[] property = GetProperty(KEY_FONTS, "default").Split(',');

            if (property.Contains("default"))
            {
                fontProvider = new XMLWorkerFontProvider { DefaultEmbedding = true };
            }

            if (property.Contains("embed"))
            {
                for (int i = 0; true; i++)
                {
                    string fontName = GetIndexedStringProperty(KEY_FONT_NAME, i);
                    string fontNameWithoutExtension = Path.GetFileNameWithoutExtension(fontName);
                    string fontPath = GetIndexedStringProperty(KEY_FONT_PATH, i);

                    if (string.IsNullOrEmpty(fontName) || string.IsNullOrEmpty(fontPath))
                    {
                        return fontProvider;
                    }

                    if (fontProvider == null)
                    {
                        fontProvider = new XMLWorkerFontProvider { DefaultEmbedding = true };
                    }

                    fontProvider.Register(fontPath, fontNameWithoutExtension);
                }
            }

            return fontProvider;
        }

        private string GetIndexedStringProperty(string key, int x)
        {
            return GetProperty(key.Replace("[x]", $"[{x}]"), null);
        }
    }
}
