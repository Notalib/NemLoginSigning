using HtmlAgilityPack;
using PuppeteerSharp;
using PuppeteerSharp.Media;


namespace Nemlogin.QualifiedSigning.SDK.Pades.Logic;

public class Html2PdfGeneratorV2
{

    static async Task<byte[]> ConvertHtmlToPdfAsync(string htmlContent)
    {
        try
        {
            // Configure PuppeteerSharp
            String[] args = { "--no-sandbox" };
            await new BrowserFetcher().DownloadAsync();
            await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions
            {
                Headless = true,
                Args = args
            });

            await using var page = await browser.NewPageAsync();
            await page.SetContentAsync(htmlContent);
            var pdfOptions = new PdfOptions
            {
                Format = PaperFormat.A4,
                MarginOptions = new MarginOptions() { Bottom = "50px", Left = "50px", Right = "50px", Top = "50px" },
                PrintBackground = true
            };

            return page.PdfDataAsync(pdfOptions).Result;
        }
        catch (Exception e)
        {
            throw new IOException("Failed to generate pdf file from html content.", e);
        }
    }

    public async Task<byte[]> GeneratePdfDocument(string inputHtml, TransformationPropertiesHandler propertiesHandler)
    {
        
        string bodyStyle = "body { font-family: " + propertiesHandler.GetBodyFont() + "; }";
        string monospaceStyle = "pre { font: " + propertiesHandler.GetMonospaceFont() + "; }";
        
        var doc = new HtmlDocument();
        doc.LoadHtml(inputHtml);
        
        HtmlNode headNode = doc.DocumentNode.SelectSingleNode("//head");
        HtmlNode htmlNode = doc.DocumentNode.SelectSingleNode("//html");
        
        if (headNode == null)
        {
            headNode = HtmlNode.CreateNode("<head><style>" + bodyStyle + " " + monospaceStyle + "</style></head>");
            htmlNode.PrependChild(headNode);
        }
        else
        {
            HtmlNode style = HtmlNode.CreateNode("<style>" + bodyStyle + " " + monospaceStyle + "</style>");
            headNode.PrependChild(style);
        }
        
        var modifiedHtml = doc.DocumentNode.OuterHtml; // Use OuterHtml to get the whole modified HTML content
        
        return await ConvertHtmlToPdfAsync(modifiedHtml);
    }
    private string CloseTagsToXhtmlCompliance(string html)
    {
        StringWriter writer = new StringWriter();

        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        doc.OptionWriteEmptyNodes = true;
        doc.Save(writer);

        return writer.ToString();
    }
}