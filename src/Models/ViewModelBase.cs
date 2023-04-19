using System;

namespace NemLoginSigningWebApp.Models
{
    /// <summary>
    /// Baseclass for all model classes for the WebApp.
    /// </summary>
    public abstract class ViewModelBase
    {
        public string Language { get; set; }
    }
}