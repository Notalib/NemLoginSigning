using System;

namespace NemLoginSigningWebApp.Models
{
    /// <summary>
    /// Represents the model for the Error.cshtml page if a general error should occur
    /// </summary>
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}