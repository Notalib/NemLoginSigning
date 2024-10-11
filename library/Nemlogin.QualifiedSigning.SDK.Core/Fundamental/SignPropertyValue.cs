namespace Nemlogin.QualifiedSigning.SDK.Core.Fundamental;

/// <summary>
/// Defines SignPropertyValues to be used in XAdES SignProperties DTBS.
/// Properties can be either of type Binary or type String.
/// </summary>

public class SignPropertyValue
{
    public string Value { get; private set; }

    public SignPropertyValue(SignPropertyValue signPropertyValue)
    {
        Value = signPropertyValue.Value;
        Type = signPropertyValue.Type;
    }

    public SignPropertyValue(string value, SignPropertyValueType type)
    {
        Value = value;
        Type = type;
    }

    public SignPropertyValue(byte[] value)
    {
        Value = Convert.ToBase64String(value);
        Type = SignPropertyValueType.BinaryValue;
    }

    public SignPropertyValueType Type { get; set; }


    public enum SignPropertyValueType
    {
        StringValue,
        BinaryValue,
    }
}