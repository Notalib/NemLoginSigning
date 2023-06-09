﻿using System;
using System.Collections.Generic;
using System.Text;

namespace NemLoginSigningXades.GeneratedSources
{
    public partial class SignedInfoType
    {
        public SignedInfoType withCanonicalizationMethod(CanonicalizationMethodType canonicalizationMethodType)
        {
            CanonicalizationMethod = canonicalizationMethodType;
            return this;
        }

        public SignedInfoType WithSignatureMethod(SignatureMethodType signatureMethod)
        {
            SignatureMethod = signatureMethod;
            return this;
        }

        public SignedInfoType WithReference(ReferenceType referenceType)
        {
            Reference = new ReferenceType[] { referenceType };
            return this;
        }
    }
}
