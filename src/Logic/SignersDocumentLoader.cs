using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NemLoginSigningCore.Core;
using NemLoginSigningWebApp.DTOs;
using static NemLoginSigningCore.Core.SignersDocumentFile;

namespace NemLoginSigningWebApp.Logic
{
    /// <summary>
    /// Support class and methods for the controller to load files and signproperties to the WebApp
    /// and for calling the signing workflow.
    /// </summary>
    public class SignersDocumentLoader : ISignersDocumentLoader
    {
        private static readonly string[] FileExtensions = { ".TXT", ".PDF", ".XML", ".HTML", ".HTM" };

        public bool UseMonospaceInTxt { get; set; } = false;

        public SignersDocumentLoader()
        {
        }

        public IEnumerable<SignersDocument> GetFiles()
        {
            string directory = Directory.Exists(".\\wwwroot\\content\\UploadedFiles") ?
                        ".\\wwwroot\\content\\UploadedFiles" :
                        ".\\wwwroot\\content\\Files";

            IEnumerable<string> files = Directory.EnumerateFiles(directory).Where(f => FileExtensions.Contains(Path.GetExtension(f).ToUpperInvariant()));

            if (!files.Any())
            {
                directory = ".\\wwwroot\\content\\Files";
                files = Directory.EnumerateFiles(directory).Where(f => FileExtensions.Contains(Path.GetExtension(f).ToUpperInvariant()));
            }

            List<SignersDocument> signersDocumentList = new List<SignersDocument>();

            foreach (string item in files)
            {
                signersDocumentList.Add(CreateSignersDocumentFromFile(item));
            }

            return signersDocumentList;
        }

        public SignersDocument CreateSignersDocumentFromFile(string filePath)
        {
            string fileName = System.IO.Path.GetFileName(filePath);

            SignersDocumentFile signersDocumentFile = new SignersDocumentFileBuilder()
                   .WithName(fileName)
                   .WithPath(filePath)
                   .Build();

            return GetSignersDocument(filePath, signersDocumentFile, GetSignProperties(filePath));
        }

        public SignersDocument CreateSignersDocumentFromContent(string fileName, byte[] content, SignProperties signProperties)
        {
            SignersDocumentFile signersDocumentFile = new SignersDocumentFileBuilder()
                .WithName(fileName)
                .WithData(content)
                .Build();

            return GetSignersDocument(fileName, signersDocumentFile, signProperties);
        }

        public SignersDocument CreateSignersDocumentFromSigningDocumentDTO(SigningDocumentDTO dto)
        {
            return CreateSignersDocumentFromContent(dto.FileName, Convert.FromBase64String(dto.EncodedContent), new SignProperties());
        }

        private SignersDocument GetSignersDocument(string filePath, SignersDocumentFile signersDocumentFile, SignProperties signProperties)
        {
            string fileExtension = Path.GetExtension(filePath).ToUpperInvariant();
            SignersDocument signersDocument = null;

            switch (fileExtension)
            {
                case ".PDF":
                    signersDocument = new PdfSignersDocument(signersDocumentFile, signProperties);
                    break;
                case ".HTML":
                case ".HTM":
                    signersDocument = new HtmlSignersDocument(signersDocumentFile, signProperties);
                    break;
                case ".TXT":
                    signersDocument = new PlainTextSignersDocument(signersDocumentFile, UseMonospaceInTxt, signProperties);
                    break;
                case ".XML":
                    signersDocument = new XmlSignersDocument(signersDocumentFile, GetXSLTDocumentFile(filePath), signProperties);
                    break;
            }

            return signersDocument;
        }

        private SignersDocumentFile GetXSLTDocumentFile(string filePath)
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

            string path = Path.GetFullPath(xsltFilename);

            return new SignersDocumentFileBuilder()
                .WithName(xsltFilename)
                .WithPath(path)
                .Build();
        }

        private SignProperties GetSignProperties(string filePath)
        {
            string propertyFile = GetPropertyFileName(filePath);

            if (string.IsNullOrEmpty(propertyFile) || !File.Exists(propertyFile))
            {
                return new SignProperties();
            }

            List<string> properties = File.ReadAllLines(propertyFile).ToList();

            SignProperties signProperties = new SignProperties();

            foreach (string item in properties)
            {
                string[] property = item.Split("=", StringSplitOptions.RemoveEmptyEntries);
                signProperties.Add(property[0], new SignPropertyValue(property[1], SignPropertyValue.SignPropertyValueType.StringValue));
            }

            return signProperties;
        }

        private string GetPropertyFileName(string filePath)
        {
            string propertiesFile = $"{Path.GetFileNameWithoutExtension(filePath)}.properties";

            return Path.Combine(Path.GetDirectoryName(filePath), propertiesFile);
        }
    }
}