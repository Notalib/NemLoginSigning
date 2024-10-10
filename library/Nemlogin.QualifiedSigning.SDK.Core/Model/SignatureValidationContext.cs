namespace Nemlogin.QualifiedSigning.SDK.Core.Model;

    /// <summary>
    /// Encapsulates the context needed for calling the { SigningValidationService.Validate() } method.
    /// </summary>
    public class SignatureValidationContext
    {
        public string ValidationServiceUrl { get; private set; }

        public int Timeout { get; private set; }

        public string DocumentName { get; private set; }

        private byte[] documentData;

        public byte[] GetDocumentData()
        {
            return (byte[])documentData.Clone();
        }

        public SignatureValidationContext(SignatureValidationContext ctx)
        {
            if (ctx != null)
            {
                ValidationServiceUrl = ctx.ValidationServiceUrl;
                Timeout = ctx.Timeout;
                DocumentName = ctx.DocumentName;
                documentData = ctx.GetDocumentData();
            }
        }

        public class SignatureValidationContextBuilder
        {
            SignatureValidationContext template = new SignatureValidationContext(null);
            
            public SignatureValidationContextBuilder WithValidationServiceUrl(string url)
            {
                template.ValidationServiceUrl = url;
                return this;
            }

            public SignatureValidationContextBuilder WithTimeout(int timeout)
            {
                template.Timeout = timeout;
                return this;
            }

            public SignatureValidationContextBuilder WithDocumentName(string documentName)
            {
                template.DocumentName = documentName;
                return this;
            }

            public SignatureValidationContextBuilder WithDocumentData(byte[] documentData)
            {
                template.documentData = documentData;
                return this;
            }

            public SignatureValidationContextBuilder WithDocumentPath(string path)
            {
                if (!File.Exists(path))
                    throw new FileNotFoundException("Could not find file to validate");

                template.documentData = File.ReadAllBytes(path);

                if (string.IsNullOrEmpty(template.DocumentName))
                {
                    template.DocumentName = Path.GetFileName(path);
                }

                return this;
            }

            public SignatureValidationContextBuilder WithDocumentUri(Uri uri)
            {
                if (!uri.IsFile)
                {
                    throw new FileNotFoundException("Could not find file to validate");
                }

                template.documentData = File.ReadAllBytes(uri.LocalPath);

                if (string.IsNullOrEmpty(template.DocumentName))
                {
                    template.DocumentName = Path.GetFileName(uri.LocalPath);
                }

                return this;
            }

            public SignatureValidationContext Build()
            {
                if (string.IsNullOrEmpty(template.ValidationServiceUrl))
                    throw new ArgumentNullException("Missing validationServiceUrl value");


                if (string.IsNullOrEmpty(template.DocumentName))
                    throw new ArgumentNullException("Missing documentName value");
                
                if (template.documentData == null)
                    throw new ArgumentNullException("Missing documentData value");

                return new SignatureValidationContext(template);
            }
        }
    }