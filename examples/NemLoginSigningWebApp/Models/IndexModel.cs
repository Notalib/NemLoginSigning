using System;
using System.Linq;
using System.Threading.Tasks;
using NemLoginSigningCore.Core;
using System.Collections.Generic;

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