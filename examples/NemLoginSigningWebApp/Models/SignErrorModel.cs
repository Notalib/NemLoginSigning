using System;
using System.Collections.Generic;

namespace NemLoginSigningWebApp.Models
{
    /// <summary>
    /// Used for displaying errors from the signing on the SignError.cshtml page.
    /// </summary>
    public class SignErrorModel : ViewModelBase
    {
        public SignErrorModel() { }

        public int HttpStatusCode { get; set; }

        public string Timestamp { get; set; }

        public string Message { get; set; }

        public List<DetailedSigningClientError> details = new List<DetailedSigningClientError>();
    }
}
