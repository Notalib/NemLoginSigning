using System.Globalization;
using Nemlogin.QualifiedSigning.SDK.Core.Fundamental;

namespace Nemlogin.QualifiedSigning.Common.Services;

/// <summary>
/// Support class and methods for the controller to load files and signproperties to the WebApp 
/// and for calling the signing workflow.
/// </summary>
public class SignersDocumentLoader : ISignersDocumentLoader
{
    public SignersDocumentLoader() { }

    public IEnumerable<SignersDocument> GetFiles()
    {
        string[] fileExtensions = { ".TXT", ".PDF", ".XML", ".HTML" };

        string directory = string.Empty;

        directory = Directory.Exists(".\\wwwroot\\content\\UploadedFiles") ? 
                    ".\\wwwroot\\content\\UploadedFiles" : 
                    ".\\wwwroot\\content\\Files";

        var files = Directory.EnumerateFiles(directory).Where(f => fileExtensions.Contains(Path.GetExtension(f).ToUpperInvariant()));

        if (!files.Any())
        {
            directory = ".\\wwwroot\\content\\Files";
            files = Directory.EnumerateFiles(directory).Where(f => fileExtensions.Contains(Path.GetExtension(f).ToUpperInvariant()));
        }

        List<SignersDocument> signersDocumentList = new List<SignersDocument>();

        foreach (var item in files)
        {
            signersDocumentList.Add(CreateSignersDocumentFromFile(item));
        }

        return signersDocumentList;
    }

    public SignersDocument CreateSignersDocumentFromFile(string filePath)
    {
        string fileName = System.IO.Path.GetFileName(filePath);
        SignersDocumentFile signersDocumentFile = new SignersDocumentFile.SignersDocumentFileBuilder()
               .WithName(fileName)
               .WithPath(filePath)
               .Build();

        var signProperties = GetSignProperties(filePath);

        SignersDocument signersDocument = null;

        var fileExtension = Path.GetExtension(fileName);

        switch (fileExtension)
        {
            case ".pdf":
                signersDocument = new PdfSignersDocument(signersDocumentFile, signProperties);
                break;
            case ".html":
                signersDocument = new HtmlSignersDocument(signersDocumentFile, signProperties);
                break;
            case ".txt":
                bool useMonoSpace = fileName.ToUpper(CultureInfo.InvariantCulture).Contains("-MONOSPACE");
                signersDocument = new PlainTextSignersDocument(signersDocumentFile, useMonoSpace, signProperties);
                break;
            case ".xml":
                signersDocument = new XmlSignersDocument(signersDocumentFile, GetxsltDocumentFile(filePath), signProperties);
                break;
        }

        return signersDocument;
    }

    private SignersDocumentFile GetxsltDocumentFile(string filePath)
    {
        string xsltFilename = string.Empty;

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"XML File not found {nameof(filePath)}");
        }

        if (!File.Exists(Path.ChangeExtension(filePath, "xsl")))
        {
            if (!File.Exists(Path.ChangeExtension(filePath, "xslt")))
            {
                throw new FileNotFoundException($"XSL/XSLT File not found for: {nameof(filePath)}");
            }

            xsltFilename = Path.ChangeExtension(filePath, "xslt");
        }
        else
        {
            xsltFilename = Path.ChangeExtension(filePath, "xsl");
        }
        
        var path = Path.GetFullPath(xsltFilename);

        return new SignersDocumentFile.SignersDocumentFileBuilder()
            .WithName(xsltFilename)
            .WithPath(path)
            .Build();
    }

    private SignProperties GetSignProperties(string filePath)
    {
        var propertyFile = GetPropertyFileName(filePath);

        if (string.IsNullOrEmpty(propertyFile) || !File.Exists(propertyFile))
        {
            return new SignProperties();
        }

        var properties = File.ReadAllLines(propertyFile).ToList();

        SignProperties signProperties = new SignProperties();

        foreach (var item in properties)
        {
            var property = item.Split(new[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
            signProperties.Add(property[0], new SignPropertyValue(property[1], SignPropertyValue.SignPropertyValueType.StringValue));
        }

        return signProperties;
    }

    private string GetPropertyFileName(string filePath)
    {
        var propertiesFile = $"{Path.GetFileNameWithoutExtension(filePath)}.properties";
        return Path.Combine(Path.GetDirectoryName(filePath) ?? string.Empty, propertiesFile);
    }
}