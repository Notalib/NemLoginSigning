using Nemlogin.QualifiedSigning.SDK.Core.Fundamental;

using PdfSharp;
using PdfSharp.Drawing;

namespace Nemlogin.QualifiedSigning.SDK.Pades.Logic;

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

    private const string DEFAULT_COLOR_PROFILE = "Nemlogin.QualifiedSigning.SDK.Pades.Resources.sRGB.icc";

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
        return _properties.TryGetValue(propertyKey, out string property) ? property : defaultValue;
    }
}