using Nemlogin.QualifiedSigning.SDK.Core.Enums;
using Nemlogin.QualifiedSigning.SDK.Core.Fundamental;

namespace Nemlogin.QualifiedSigning.Common.Services;

    public class TransformationPropertiesService : ITransformationPropertiesService
    {
        private const string KeyPrefix = "nemlogin.signing.pdf-generator.";
        private readonly string[] _fontExtensions = { "ttf", "otf" };

        public TransformationProperties GetTransformationProperties(SignersDocument signersDocument, SignatureFormat signatureFormat)
        {
            string keyPrefix = KeyPrefix;

            if (signersDocument == null)
            {
                throw new ArgumentNullException(nameof(signersDocument));
            }

            if (signatureFormat != SignatureFormat.PAdES || signersDocument.DocumentFormat == DocumentFormat.PDF)
                return new TransformationProperties();
            string path = signersDocument.SignersDocumentFile.Path;

            //Look for font with same name
            foreach (var fontExtension in _fontExtensions)
            {
                var fontFile = Path.ChangeExtension(path, fontExtension);
                if (File.Exists(fontFile))
                {
                    TransformationProperties properties = new()
                    {
                        { keyPrefix + "fonts", "embed, default" },
                        { keyPrefix + "font[0].name", Path.GetFileName(fontFile) },
                        { keyPrefix + "font[0].path", Path.GetFullPath(fontFile) },
                        { keyPrefix + "body-font", Path.GetFileNameWithoutExtension(fontFile) },
                        { keyPrefix + "monospace-font", "medium Courier, " + Path.GetFileNameWithoutExtension(fontFile) }
                    };

                    return properties;
                }
            }

            return new TransformationProperties();
        }
    }