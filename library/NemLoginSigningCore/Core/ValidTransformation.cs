using System.Collections.Generic;
using System.Linq;

using NemLoginSigningCore.Exceptions;
using NemLoginSigningCore.Format;

namespace NemLoginSigningCore.Core
{
    /// <summary>
    /// Defines the valid transformations.
    /// </summary>
    public static class ValidTransformation
    {
        public static IEnumerable<Transformation> Transformations
        {
            get
            {
                return new List<Transformation>
                {
                    new Transformation(DocumentFormat.TEXT, SignatureFormat.XAdES, ViewFormat.TEXT),
                    new Transformation(DocumentFormat.TEXT, SignatureFormat.PAdES, ViewFormat.PDF),
                    new Transformation(DocumentFormat.HTML, SignatureFormat.XAdES, ViewFormat.HTML),
                    new Transformation(DocumentFormat.HTML, SignatureFormat.PAdES, ViewFormat.PDF),
                    new Transformation(DocumentFormat.XML, SignatureFormat.XAdES, ViewFormat.HTML),
                    new Transformation(DocumentFormat.XML, SignatureFormat.PAdES, ViewFormat.PDF),
                    new Transformation(DocumentFormat.PDF, SignatureFormat.XAdES, ViewFormat.PDF),
                    new Transformation(DocumentFormat.PDF, SignatureFormat.PAdES, ViewFormat.PDF),
                };
            }
        }

        public static Transformation GetTransformation(DocumentFormat sdFormat, SignatureFormat dtbsFormat)
        {
            return Transformations.FirstOrDefault(t => t.SdFormat == sdFormat && t.SignatureFormat == dtbsFormat)
                   ?? throw new InvalidSignatureParametersException($"No valid format for sdFormat: {sdFormat}, dtbsFormat: {dtbsFormat}");
        }
    }
}