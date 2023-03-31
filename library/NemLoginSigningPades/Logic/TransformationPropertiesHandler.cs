using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using NemLoginSigningCore.Core;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace NemLoginSigningPades.Logic
{
    public class TransformationPropertiesHandler 
    {
        public static string KEY_PREFIX = "nemlogin.signing.pdf-generator.";
        public static string KEY_COLOR_PROFILE = KEY_PREFIX + "color-profile";
        public static string KEY_FONTS = KEY_PREFIX + "fonts";
        public static string KEY_FONT_NAME = KEY_PREFIX + "font[x].name";
        public static string KEY_FONT_PATH = KEY_PREFIX + "font[x].path";
        public static string KEY_BODY_FONT = KEY_PREFIX + "body-font";
        public static string KEY_MONOSPACE_FONT = KEY_PREFIX + "monospace-font";
        public static string KEY_PAGE_SIZE = KEY_PREFIX + "page-size";
        public static string KEY_PAGE_ORIENTATION = KEY_PREFIX + "page-orientation";
        public static string KEY_PAGE_MARGIN = KEY_PREFIX + "page-margin";
        
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
            var pageLayout = PageSize.GetRectangle(GetProperty(KEY_PAGE_SIZE, "a4"));

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
            float outResult;
            float.TryParse(GetProperty(KEY_PAGE_MARGIN, "1"), out outResult);

            return outResult;
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

            var property = GetProperty(KEY_FONTS, "default").Split(',');

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
