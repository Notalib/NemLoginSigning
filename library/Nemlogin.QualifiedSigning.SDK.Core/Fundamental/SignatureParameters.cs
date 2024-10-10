using AutoMapper;

using Nemlogin.QualifiedSigning.SDK.Core.Enums;
using Nemlogin.QualifiedSigning.SDK.Core.Exceptions;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Nemlogin.QualifiedSigning.SDK.Core.Fundamental;

/// <summary>
/// Defines the signature parameters to be passed on to the signing client along with the signing text.
/// </summary>
public class SignatureParameters
{
    private static int REFERENCE_TEXT_MAX_LENGTH = 50;
    private const int VERSION = 1;
    private const string DTBS_DIGEST_ALGORITHM = "SHA-256";

    public SignatureParameters() { }

    public SignatureParameters(SignatureParameters signatureParameters)
    {
        MapperConfiguration config = new MapperConfiguration(cfg => cfg.CreateMap<SignatureParameters, SignatureParameters>());
        Mapper mapper = new Mapper(config);
        mapper.Map(signatureParameters, this);
    }

    public int? Version => VERSION;

    [JsonConverter(typeof(StringEnumConverter))]
    public FlowType? FlowType { get; set; }

    public string EntityID { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public DocumentFormat? DocumentFormat { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public virtual SignatureFormat? SignatureFormat { get; set; }

    public string DtbsDigest { get; set; }

    public string DtbsDigestAlgorithm { get; set; } = DTBS_DIGEST_ALGORITHM;

    public string DtbsSignedInfo { get; internal set; }

    public string ReferenceText { get; set; }

    public int? MinAge { get; set; }

    public string SignerSubjectNameID { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public Language? PreferredLanguage { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public SsnPersistenceLevel? SsnPersistenceLevel { get; set; }

    public bool? AnonymizeSigner { get; set; }

    [JsonConverter(typeof(StringEnumConverter))]
    public AcceptedCertificatePolicies? AcceptedCertificatePolicies { get; set; }

    public void Validate()
    {
        // Check common SP and Broker flow mandatory parameters
        if (Version == null || FlowType == null || string.IsNullOrEmpty(EntityID) || DocumentFormat == null || SignatureFormat == null || string.IsNullOrEmpty(DtbsDigestAlgorithm) || string.IsNullOrEmpty(DtbsSignedInfo))
        {
            throw new InvalidSignatureParametersException("Missing mandatory parameters");
        }

        // SP specific mandatory parameters
        if (FlowType == Enums.FlowType.ServiceProvider)
        {
            if (ReferenceText == null || DtbsDigest == null)
            {
                throw new InvalidSignatureParametersException("Missing mandatory parameters for SP flow");
            }

            if (ReferenceText.Length > REFERENCE_TEXT_MAX_LENGTH)
            {
                throw new InvalidSignatureParametersException($"Reference text exceeds the maximum allowed length of {REFERENCE_TEXT_MAX_LENGTH.ToString()}");
            }
        }

        // Check Broker-flow specific parameters
        if (FlowType == Enums.FlowType.Broker)
        {
            if (!string.IsNullOrEmpty(DtbsDigest) || !string.IsNullOrEmpty(ReferenceText) || MinAge != 0 && PreferredLanguage != null || SsnPersistenceLevel != null
                || !string.IsNullOrEmpty(SignerSubjectNameID) || AnonymizeSigner != null || AcceptedCertificatePolicies.HasValue)
            {
                throw new InvalidSignatureParametersException("Invalid parameters for Broker Flow");
            }
        }
    }

    public void WithDtbsSignedInfo(string dtbsSignedInfo)
    {
        DtbsSignedInfo = dtbsSignedInfo;
    }

    public class SignatureParametersBuilder
    {
        private readonly SignatureParameters builderTemplate;

        public SignatureParametersBuilder()
        {
            builderTemplate = new SignatureParameters();
        }

        public SignatureParametersBuilder WithFlowType(FlowType flowType)
        {
            builderTemplate.FlowType = flowType;
            return this;
        }

        public SignatureParametersBuilder WithSignersDocumentFormat(DocumentFormat signersDocumentFormat)
        {
            builderTemplate.DocumentFormat = signersDocumentFormat;
            return this;
        }

        public SignatureParametersBuilder WithSignatureFormat(SignatureFormat signatureFormat)
        {
            builderTemplate.SignatureFormat = signatureFormat;
            return this;
        }

        public SignatureParametersBuilder WithValidTransformation(Transformation transformation)
        {
            if (transformation == null)
            {
                throw new ArgumentNullException(nameof(Transformation));
            }

            builderTemplate.DocumentFormat = transformation.SdFormat;
            builderTemplate.SignatureFormat = transformation.SignatureFormat;
            return this;
        }

        public SignatureParametersBuilder WithDtbsDigest(string dtbsDigest)
        {
            builderTemplate.DtbsDigest = dtbsDigest;
            return this;
        }

        public SignatureParametersBuilder WithDtbsDigestAlgorithm(string dtbsDigestAlgorithm)
        {
            builderTemplate.DtbsDigestAlgorithm = dtbsDigestAlgorithm;
            return this;
        }

        public SignatureParametersBuilder WithDtbsSignedInfo(string dtbsSignedInfo)
        {
            builderTemplate.DtbsSignedInfo = dtbsSignedInfo;
            return this;
        }

        public SignatureParametersBuilder WithReferenceText(string referenceText)
        {
            builderTemplate.ReferenceText = referenceText;
            return this;
        }

        public SignatureParametersBuilder WithMinAge(int minAge)
        {
            builderTemplate.MinAge = minAge;
            return this;
        }

        public SignatureParametersBuilder WithSignerSubjectNameID(string signersSubjectNameID)
        {
            builderTemplate.SignerSubjectNameID = signersSubjectNameID;
            return this;
        }

        public SignatureParametersBuilder WithPreferredLanguage(Language preferredLanguage)
        {
            builderTemplate.PreferredLanguage = preferredLanguage;
            return this;
        }

        public SignatureParametersBuilder WithEntityID(string entityID)
        {
            builderTemplate.EntityID = entityID;
            return this;
        }

        public SignatureParametersBuilder WithSsnPersistenceLevel(SsnPersistenceLevel ssnPersistenceLevel)
        {
            builderTemplate.SsnPersistenceLevel = ssnPersistenceLevel;
            return this;
        }

        public SignatureParametersBuilder WithAnonymizeSigner(bool anonymizeSigner)
        {
            builderTemplate.AnonymizeSigner = anonymizeSigner;
            return this;
        }

        public SignatureParametersBuilder WithAcceptedCertificatePolicies(AcceptedCertificatePolicies acceptedCertificatePolicies)
        {
            builderTemplate.AcceptedCertificatePolicies = acceptedCertificatePolicies;
            return this;
        }
        public SignatureParameters Build()
        {
            return builderTemplate;
        }
    }
}