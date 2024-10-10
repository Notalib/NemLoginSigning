using Nemlogin.QualifiedSigning.SDK.Core;
using Nemlogin.QualifiedSigning.SDK.Core.Configuration;
using Nemlogin.QualifiedSigning.SDK.Core.DTO;
using Nemlogin.QualifiedSigning.SDK.Core.Enums;
using Nemlogin.QualifiedSigning.SDK.Core.Fundamental;
using Nemlogin.QualifiedSigning.SDK.Core.Services;
using Nemlogin.QualifiedSigning.SDK.Core.Utilities;

namespace Nemlogin.QualifiedSigning.Common.Services;

/// <summary>
/// Class responsible for handling the WebApp's Signing requests from the controller, 
/// generating the Signature Parameters, the TransformationContext and passing 
/// it to the SigningPayloadService.ProduceSigningPayload method.
/// </summary>
public class DocumentSigningService : IDocumentSigningService
{
    private readonly ISigningPayloadService _signingPayloadService;
    private readonly ISignersDocumentLoader _signersDocumentLoader;
    private readonly ITransformationPropertiesService _transformationPropertiesService;
    private readonly NemloginConfiguration _nemloginConfiguration;

    public DocumentSigningService(ISigningPayloadService signingPayloadService,
        ISignersDocumentLoader signersDocumentLoader,
        ITransformationPropertiesService transformationPropertiesService,
        NemloginConfiguration nemloginConfiguration)
    {
        if (nemloginConfiguration == null)
        {
            throw new ArgumentNullException(nameof(nemloginConfiguration));
        }

        _signingPayloadService = signingPayloadService ?? throw new ArgumentNullException(nameof(signingPayloadService));
        _signersDocumentLoader = signersDocumentLoader ?? throw new ArgumentNullException(nameof(signersDocumentLoader));
        _transformationPropertiesService = transformationPropertiesService ?? throw new ArgumentNullException(nameof(transformationPropertiesService));
        _nemloginConfiguration = nemloginConfiguration;
    }

    public SigningPayloadDTO GenerateSigningPayload(SignatureFormat signatureFormat, SignatureKeys signatureKeys, string language, string referenceText, string filePath)
    {
        var signersDocument = _signersDocumentLoader.CreateSignersDocumentFromFile(filePath);

        SignatureParameters signatureParameters = new SignatureParameters.SignatureParametersBuilder()
            .WithFlowType(FlowType.ServiceProvider)
            .WithPreferredLanguage((Language)(Enum.Parse(typeof(Language), language)))
            .WithReferenceText(referenceText)
            .WithSignersDocumentFormat(signersDocument.DocumentFormat)
            .WithSignatureFormat(signatureFormat)
            .WithEntityID(_nemloginConfiguration.SignatureKeysConfiguration.Single().EntityId)
            .Build();

        TransformationProperties transformationProperties = _transformationPropertiesService.GetTransformationProperties(signersDocument, signatureFormat);

        TransformationContext ctx = new TransformationContext(signersDocument, signatureKeys, signatureParameters, transformationProperties);

        var signingPayload = _signingPayloadService.ProduceSigningPayload(ctx);

        SigningPayloadDTO signingPayloadDto = new(signingPayload);

        // If "SaveDtbsToFolder" property is set in configuration we will save the signed file to 
        // the specified folder.
        if (!string.IsNullOrEmpty(_nemloginConfiguration.SaveDtbsToFolder))
        {
            SaveFileToDtbsFolder(signingPayload);
        }

        return signingPayloadDto;
    }

    /// <summary>
    /// Saves the signed file to the specified folder. If the path is empty or does not exist 
    /// the method does nothing.
    /// </summary>
    /// <param name="signingPayload"></param>
    private void SaveFileToDtbsFolder(SigningPayload signingPayload)
    {
        string relativePath = _nemloginConfiguration.SaveDtbsToFolder;
        if (string.IsNullOrEmpty(relativePath))
        {
            return;
        }

        string extension = signingPayload.DataToBeSigned.Format == SignatureFormat.PAdES ? "pdf" : "xml";
        FileWriter.WriteFileToPath(relativePath, "ProduceSigningPayload_End", extension, signingPayload.DataToBeSigned.GetData());
    }
}