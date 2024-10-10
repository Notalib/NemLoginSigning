namespace Nemlogin.QualifiedSigning.SDK.Xades.GeneratedSources;

public partial class TransformsType
{
    public TransformsType WithTransform(TransformType transformType)
    {
        TransformType[] transformTypeArray = new TransformType[] { transformType };
        this.Transform = transformTypeArray;
        return this;
    }

    public static TransformsType CreateTransformsType()
    {
        return new TransformsType();
    }
}