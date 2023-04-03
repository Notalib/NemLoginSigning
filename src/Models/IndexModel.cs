using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NemLoginSigningCore.Core;

namespace NemLoginSigningWebApp.Models
{
    /// <summary>
    /// Class for representing the files to sign showed on the index.cshtml page
    /// </summary>
    public class IndexModel : ViewModelBase
    {
        public IEnumerable<SignersDocument> Files { get; set; }
    }
}