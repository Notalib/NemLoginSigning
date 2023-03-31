using System;
using NemLoginSigningCore.Core;

namespace NemLoginSigningCore.Logic
{
    public class NullSourceAttacher : ISourceAttacher
    {
        /// <summary>
        /// Only PDF files need to have source files attached so this is a NullSourceAttacher
        /// created for other scenarios.
        /// </summary>
        /// <param name="ctx"></param>
        public void Attach(TransformationContext ctx)
        {
            // Do not attach anything for this transformation
        }

        public bool CanAttach(Transformation transformation)
        {
            if (transformation == null)
            {
                throw new ArgumentNullException(nameof(transformation));
            }

            return !(transformation.SdFormat == Format.DocumentFormat.XML && transformation.SignatureFormat == Format.SignatureFormat.PAdES);
        }
    }
}
