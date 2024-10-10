using System.Collections.Generic;
using System.IO;
using System.Linq;

using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Extensions.Logging;
using NemLoginSigningCore.Core;
using NemLoginSigningCore.Logging;
using NemLoginSigningPades.Logic;
using NemLoginSigningValidation;
using NemLoginSigningValidation.PDFValidation;
using Xunit;

using static NemLoginSigningPades.Logic.TransformationPropertiesHandler;

namespace NemloginSigningTest
{
    public class PDFTransformationPropertiesTest : SigningTestBase
    {
        private readonly ILogger _logger;

        public PDFTransformationPropertiesTest()
        {
            _logger = LoggerCreator.CreateLogger<PDFTransformationPropertiesTest>();
        }

        [Fact]
        public void PageSizeAndMarginTest()
        {
            // Arrange
            TransformationProperties properties = new TransformationProperties();

            properties.Add(KEY_PAGE_SIZE, "a4");
            properties.Add(KEY_PAGE_MARGIN, "10");
            properties.Add(KEY_PAGE_ORIENTATION, "landscape");

            string html = "<html><head><style>body { font-family: Helvetica; }</style></head><body>test</body></html>";

            // Act
            byte[] pdfDocument = GeneratePDFDocument(properties, html);

            PdfReader reader = new PdfReader(pdfDocument);

            // Assert
            int pageRoration = reader.GetPageRotation(1);
            Rectangle pageSize = reader.GetPageSize(1);

            Assert.True(pageRoration == 90);
            Assert.Equal(PageSize.A4.Width, pageSize.Width);
            Assert.Equal(PageSize.A4.Height, pageSize.Height);
        }

        [Fact]
        public void FontHandlingTest()
        {
            // Arrange
            TransformationProperties properties = new TransformationProperties();
            properties.Add(KEY_FONTS, "default");

            string html = "<html><head><style> body { font-family: Helvetica; } </style></head><body><p>test</p></body></html>";

            byte[] pdfDocument = GeneratePDFDocument(properties, html);

            IEnumerable<PdfFontDescriptor> fonts = GetFonts(pdfDocument);

            Assert.Single(fonts);

            PdfFontDescriptor descriptor = fonts.Single();
            PdfName fontName = descriptor.FontName;

            Assert.Contains("Helvetica", fontName.ToString());

            properties = new TransformationProperties();
            properties.Add(KEY_FONTS, "embed");
            properties.Add(KEY_FONT_NAME.Replace("[x]", "[0]"), "Karla");
            properties.Add(KEY_FONT_PATH.Replace("[x]", "[0]"), Path.Combine(@"Resources", "Fonts", "Karla-Bold.ttf"));

            html = "<html><head><style> body { font-family: Karla; } </style></head><body><p>test</p></body></html>";

            pdfDocument = GeneratePDFDocument(properties, html);

            fonts = GetFonts(pdfDocument);

            Assert.Contains(fonts, x => x.Embedded == true && x.FontName.ToString().Contains("Karla"));
        }

        [Fact]
        public void ColorProfileDefaultTest()
        {
            // Arrange
            TransformationProperties properties = new TransformationProperties();
            properties.Add(KEY_COLOR_PROFILE, "default");

            string html = "<html><body><p>test</p></body></html>";

            // Act
            byte[] pdfDocument = GeneratePDFDocument(properties, html);

            // Assert
            IEnumerable<PdfObject> outputIntents = GetOutputIntents(pdfDocument);
            Assert.Single(outputIntents);

            properties = new TransformationProperties();
            properties.Add(KEY_COLOR_PROFILE, Path.Combine(@"Resources", "ColorProfiles", "test-sRGB.icc"));
        }

        [Fact]
        public void ColorProfileNoneTest()
        {
            // Arrange
            TransformationProperties properties = new TransformationProperties();
            properties.Add(KEY_COLOR_PROFILE, "none");

            string html = "<html><body><p>test</p></body></html>";

            // Act
            byte[] pdfDocument = GeneratePDFDocument(properties, html);

            // Assert
            IEnumerable<PdfObject> outputIntents = GetOutputIntents(pdfDocument);
            Assert.Empty(outputIntents);
        }

        [Fact]
        public void ColorProfileSpecificTest()
        {
            // Arrange
            TransformationProperties properties = new TransformationProperties();
            properties.Add(KEY_COLOR_PROFILE, Path.Join(@"./Resources", "ColorProfiles", "test-sRGB.icc"));

            string html = "<html><body><p>test</p></body></html>";

            // Act
            byte[] pdfDocument = GeneratePDFDocument(properties, html);

            // Assert
            IEnumerable<PdfObject> outputIntents = GetOutputIntents(pdfDocument);
            Assert.Single(outputIntents);
        }

        private byte[] GeneratePDFDocument(TransformationProperties properties, string html)
        {
            Html2PDFGenerator html2PdfGenerator = new Html2PDFGenerator();
            byte[] document = html2PdfGenerator.GeneratePDFDocument(html, new TransformationPropertiesHandler(properties));

            return document;
        }

        private IEnumerable<PdfFontDescriptor> GetFonts(byte[] document)
        {
            PDFValidator pdfValidator = new PDFValidator(_logger);
            PdfFontValidator pdfFontValidator = new PdfFontValidator();

            IEnumerable<PdfObject> pdfObjects;

            using (PdfReader reader = new PdfReader(document))
            {
                pdfObjects = pdfValidator.GetPdfObjects(reader);
            }

            return pdfFontValidator.ScanForFonts(pdfObjects);
        }

        private IEnumerable<PdfObject> GetOutputIntents(byte[] document)
        {
            PDFValidator pdfValidator = new PDFValidator(_logger);
            IEnumerable<PdfObject> pdfObjects;

            List<PdfObject> outputIntents = new List<PdfObject>();

            using (PdfReader reader = new PdfReader(document))
            {
                pdfObjects = pdfValidator.GetPdfObjects(reader);
            }

            foreach (PdfObject item in pdfObjects)
            {
                if (item == null)
                {
                    continue;
                }

                if (item.IsDictionary())
                {
                    PdfDictionary pdfDictionary = (PdfDictionary)item;
                    PdfObject type = pdfDictionary.Get(PdfName.TYPE);

                    if (type != null && type.IsName())
                    {
                        PdfName pdfName = (PdfName)type;
                        if (pdfName.DecodeName() == PdfName.CATALOG.DecodeName())
                        {
                            PdfObject subTypePdfObject = pdfDictionary.Get(PdfName.OUTPUTINTENTS);

                            if (subTypePdfObject != null)
                            {
                                outputIntents.Add(subTypePdfObject);
                            }
                        }
                    }
                }
            }

            return outputIntents;
        }
    }
}
