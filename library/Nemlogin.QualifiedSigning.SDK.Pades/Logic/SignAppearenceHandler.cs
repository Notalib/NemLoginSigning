using System.Globalization;

using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf.Annotations;

namespace Nemlogin.QualifiedSigning.SDK.Pades.Logic;

public class SignAppearanceHandler : IAnnotationAppearanceHandler
{

    public void DrawAppearance(XGraphics gfx, XRect rect)
    {
        string text = "Signed by Qualified Signing - Location: Denmark - Date: " + DateTime.Now.ToString(CultureInfo.InvariantCulture);
        XFont font = new("Verdana", 7.0, XFontStyleEx.Regular);
        XTextFormatter xTextFormatter = new(gfx);
        XPoint xPoint = new(0.0, 0.0);
        xTextFormatter.DrawString(text, font, new XSolidBrush(XColor.FromKnownColor(XKnownColor.Black)),
            new XRect(xPoint.X, xPoint.Y, rect.Width - xPoint.X, rect.Height), XStringFormats.TopLeft);
    }
}