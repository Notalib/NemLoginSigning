using System;
using System.Collections.Generic;
using System.Text;

namespace NemLoginSigningXades.GeneratedSources
{
    public partial class TransformType
    {
        public static TransformType CreateTransformType()
        {
            return new TransformType();
        }

        public TransformType WithAlgorithm(string algorithm)
        {
            this.Algorithm = algorithm;
            return this;
        }
    }
}
