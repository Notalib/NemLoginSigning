using System;
using System.IO;

namespace NemLoginSignatureValidationService.Model
{
    /// <summary>
    /// Encapsulates the context needed for calling the { SigningValidationService.Validate() } method.
    /// </summary>
    public class SignatureValidationContext
    {
        public string ValidationServiceUrl { get; private set; }

        public int Timeout { get; private set; }

        public string DocumentName { get; private set; }

        private byte[] _documentData;

        public byte[] GetDocumentData()
        {
            return (byte[])_documentData.Clone();
        }

        public SignatureValidationContext(SignatureValidationContext ctx)
        {
            if (ctx != null)
            {
                ValidationServiceUrl = ctx.ValidationServiceUrl;
                Timeout = ctx.Timeout;
                DocumentName = ctx.DocumentName;
                _documentData = ctx.GetDocumentData();
            }
        }

        public class SignatureValidationContextBuilder
        {
            private SignatureValidationContext _template = new SignatureValidationContext(null);

            public SignatureValidationContextBuilder WithValidationServiceUrl(string url)
            {
                _template.ValidationServiceUrl = url;
                return this;
            }

            public SignatureValidationContextBuilder WithTimeout(int timeout)
            {
                _template.Timeout = timeout;
                return this;
            }

            public SignatureValidationContextBuilder WithDocumentName(string documentName)
            {
                _template.DocumentName = documentName;
                return this;
            }

            public SignatureValidationContextBuilder WithDocumentData(byte[] documentData)
            {
                _template._documentData = documentData;
                return this;
            }

            public SignatureValidationContextBuilder WithDocumentPath(string path)
            {
                if (!File.Exists(path))
                {
                    throw new FileNotFoundException("Could not find file to validate");
                }

                _template._documentData = File.ReadAllBytes(path);

                if (string.IsNullOrEmpty(_template.DocumentName))
                {
                    _template.DocumentName = Path.GetFileName(path);
                }

                return this;
            }

            public SignatureValidationContextBuilder WithDocumentUri(Uri uri)
            {
                if (!uri.IsFile)
                {
                    throw new FileNotFoundException("Could not find file to validate");
                }

                _template._documentData = File.ReadAllBytes(uri.LocalPath);

                if (string.IsNullOrEmpty(_template.DocumentName))
                {
                    _template.DocumentName = Path.GetFileName(uri.LocalPath);
                }

                return this;
            }

            public SignatureValidationContext Build()
            {
                if (string.IsNullOrEmpty(_template.ValidationServiceUrl))
                {
                    throw new ArgumentNullException("Missing validationServiceUrl value");
                }

                if (string.IsNullOrEmpty(_template.DocumentName))
                {
                    throw new ArgumentNullException("Missing documentName value");
                }

                if (_template._documentData == null)
                {
                    throw new ArgumentNullException("Missing documentData value");
                }

                return new SignatureValidationContext(_template);
            }
        }
    }
}