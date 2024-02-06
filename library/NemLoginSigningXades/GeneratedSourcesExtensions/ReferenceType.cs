using System;

namespace NemLoginSigningXades.GeneratedSources
{
    public partial class ReferenceType
    {
        public ReferenceType WithTransforms(TransformsType transformsType)
        {
            ArgumentNullException.ThrowIfNull(transformsType);

            Transforms = transformsType.Transform;
            return this;
        }
    }
}
