using System.ComponentModel;
namespace Nemlogin.QualifiedSigning.SDK.Core.Enums;

/// <summary>
/// NemLog-In Signing SDK error codes
/// </summary>
public enum ErrorCode
{
    [Description("Error loading SD")]
    SDK001,

    [Description("Invalid Signature Parameters")]
    SDK002,

    [Description("Service Implementation unavailable")]
    SDK003,

    [Description("Error JWS-signing Signing Payload")]
    SDK004,

    [Description("Error generating DTBS signature template")]
    SDK005,

    [Description("Error computing DTBS digest")]
    SDK006,

    [Description("Error transforming SD to PDF DTBS document")]
    SDK007,

    [Description("Error adding attachments to PDF DTBS document")]
    SDK008,

    [Description("Error transforming SD to XML DTBS document")]
    SDK009,

    [Description("Error validating SD")]
    SDK010,

    [Description("Error validating document signature")]
    SDK011,
}