using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

using NemLoginSigningCore.Core;
using NemLoginSigningCore.Format;
using NemLoginSigningService.Services;

using static NemLoginSigningCore.Core.SignatureParameters;
using static NemLoginSigningCore.Core.SignersDocumentFile;

namespace NemloginSigningTest
{
    /// <summary>
    /// Testing multiple files transforming Signers Documents and
    /// generating Data-To-Be-Signed with the use of 'SigningPayloadService'.
    /// Tests is run with both Service Provider and Broker flow.
    /// Input for tests are placed in folder "./Resources/SignersDocuments" in the project.
    /// The Data-To-Be-Signed result files can be written to the "Target" folder in the executing
    /// directory. For this to happen set the property "SaveToFolder: true" in appsettings.json
    /// in the settings region "TestSettings".
    /// </summary>
    public class SigningPayloadTests : SigningTestBase
    {
        private const string SignersDocuments = "./Resources/SignersDocuments";
        private const string DtbsTargetFolder = "./Target";

        private SigningPayloadService _signingPayloadService = new SigningPayloadService(new NullLogger<SigningPayloadService>());

        [Theory]
        [MemberData(nameof(LoadAllFilesWithTypes), parameters: [SignersDocuments, new[] { "*.pdf", "*.txt", "*.html", "*.xml" }])]
        public void TransformAllToFormatsTheory(string file)
        {
            TransformSignersDocumentToPdfUsingSpFlow(file);
            TransformSignersDocumentToXmlUsingSpFlow(file);
            TransformSignersDocumentToPdfUsingBrokerFlow(file);
            TransformSignersDocumentToXmlUsingBrokerFlow(file);
        }

        private void TransformSignersDocumentToXmlUsingBrokerFlow(string file)
        {
            Transform(file, FlowType.Broker, SignatureFormat.XAdES);
        }

        private void TransformSignersDocumentToPdfUsingBrokerFlow(string file)
        {
            Transform(file, FlowType.Broker, SignatureFormat.PAdES);
        }

        private void TransformSignersDocumentToXmlUsingSpFlow(string file)
        {
            Transform(file, FlowType.ServiceProvider, SignatureFormat.XAdES);
        }

        private void TransformSignersDocumentToPdfUsingSpFlow(string file)
        {
            Transform(file, FlowType.ServiceProvider, SignatureFormat.PAdES);
        }

        private void Transform(string filePath, FlowType flowType, SignatureFormat signatureFormat)
        {
            // Arrange
            string fileName = Path.GetFileName(filePath);
            string extension = Path.GetExtension(fileName);

            extension = string.IsNullOrEmpty(extension) ? extension : extension.Substring(1, extension.Length - 1);

            SignersDocumentFile signersDocumentFile = new SignersDocumentFileBuilder()
                .WithPath(filePath)
                .Build();

            SignersDocument signersDocument = null;

            switch (extension)
            {
                case "pdf":
                    signersDocument = new PdfSignersDocument(signersDocumentFile);
                    break;
                case "txt":
                    signersDocument = new PlainTextSignersDocument(signersDocumentFile, false);
                    break;
                case "html":
                    signersDocument = new HtmlSignersDocument(signersDocumentFile);
                    break;
                case "xml":
                    signersDocument = new XmlSignersDocument(signersDocumentFile, GetxsltDocumentFile(filePath));
                    break;
                default:
                    break;
            }

            string referenceText = flowType == FlowType.ServiceProvider ? $"Signing {fileName}" : string.Empty;

            SignatureParameters signatureParameters = new SignatureParametersBuilder()
                .WithFlowType(flowType)
                .WithSignersDocumentFormat(signersDocument.DocumentFormat)
                .WithSignatureFormat(signatureFormat)
                .WithReferenceText(referenceText)
                .WithEntityID(EntityID)
                .Build();

            // Transform the SignersDocument to PDF
            TransformationContext ctx = new TransformationContext(signersDocument, SignatureKeys, signatureParameters);

            SigningPayload signingPayload = null;

            // Act & Assert (assert does not throw exception)
            signingPayload = _signingPayloadService.ProduceSigningPayload(ctx);

            // Save the generated dtbs to a file if setup
            DataToBeSigned dtbs = signingPayload.DataToBeSigned;

            string dtbsFileName = $"{flowType}_{signersDocument.DocumentFormat}_{signersDocument.SignersDocumentFile.Name}";

            string dtbsFilePath = Path.Combine(DtbsTargetFolder, dtbsFileName);

            var config = TestHelper.GetConfiguration(TestConstants.ConfigurationNemlogin);
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
                else
                {
                    xsltFilename = Path.ChangeExtension(filePath, "xslt");
                }
            }
            else
            {
                xsltFilename = Path.ChangeExtension(filePath, "xsl");
            }

            var path = Path.GetFullPath(xsltFilename);

            return new SignersDocumentFileBuilder()
                .WithName(xsltFilename)
                .WithPath(path)
                .Build();
        }

        public static IEnumerable<object[]> LoadAllFilesWithTypes(string path, string[] extensions)
        {
            List<object[]> files = new List<object[]>();

            foreach (var extension in extensions)
            {
                var fe = Directory.GetFiles(path, "*.*");
                var f = Directory.GetFiles(path, extension);
                Directory.GetFiles(path, extension).ToList().ForEach(f => files.Add(new object[] { f }));
            }

            return files;
        }
    }
}
