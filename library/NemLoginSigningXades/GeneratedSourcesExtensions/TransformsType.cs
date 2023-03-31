using System;
using System.Collections.Generic;
using System.Text;

namespace NemLoginSigningXades.GeneratedSources
{
    public partial class TransformsType
    {
        public TransformsType WithTransform(TransformType transformType)
        {
            TransformType[] transformTypeArray = new TransformType[] { transformType };
            Transform = transformTypeArray;
            return this;
        }

        public static TransformsType CreateTransformsType()
        {
            return new TransformsType();
        }
    }
}
