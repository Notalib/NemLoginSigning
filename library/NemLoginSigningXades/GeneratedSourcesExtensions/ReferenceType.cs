using System;
using System.Collections.Generic;
using System.Text;

namespace NemLoginSigningXades.GeneratedSources
{
    public partial class ReferenceType
    {
        public ReferenceType WithTransforms(TransformsType transformsType)
        {
            if (transformsType == null)
            {
                throw new ArgumentNullException(nameof(transformsType));
            }

            Transforms = transformsType.Transform;
            return this;
        }
    }
}
