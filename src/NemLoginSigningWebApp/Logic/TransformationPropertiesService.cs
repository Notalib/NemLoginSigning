using System;
using System.IO;

using Nemlogin.QualifiedSigning.SDK.Core.Enums;
using Nemlogin.QualifiedSigning.SDK.Core.Fundamental;
using Nemlogin.QualifiedSigning.SDK.Pades.Logic;

namespace NemLoginSigningWebApp.Logic;

public class TransformationPropertiesService : ITransformationPropertiesService
{
    private readonly string[] _fontExtensions = { "ttf", "otf" };

    public TransformationProperties GetTransformationProperties(SignersDocument signersDocument, SignatureFormat signatureFormat)
    {
        string keyPrefix = TransformationPropertiesHandler.KEY_PREFIX;

        ArgumentNullException.ThrowIfNull(signersDocument);

        if (signatureFormat == SignatureFormat.PAdES && signersDocument.DocumentFormat != DocumentFormat.PDF)
        {
            try
            {
                string path = signersDocument.SignersDocumentFile.Path;
                string directoryName = Path.GetDirectoryName(path);

                // Look for font with same name
                foreach (string fontExtension in _fontExtensions)
                {
                    string fontFile = Path.ChangeExtension(path, fontExtension);
                    if (File.Exists(fontFile))
                    {
                        TransformationProperties properties = new();
                        properties.Add(keyPrefix + "fonts", "embed, default");
                        properties.Add(keyPrefix + "font[0].name", Path.GetFileName(fontFile));
                        properties.Add(keyPrefix + "font[0].path", Path.GetFullPath(fontFile));
                        properties.Add(keyPrefix + "body-font", Path.GetFileNameWithoutExtension(fontFile));
                        properties.Add(keyPrefix + "monospace-font", "medium Courier, " + Path.GetFileNameWithoutExtension(fontFile));

                        return properties;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        return new TransformationProperties();
    }
}
