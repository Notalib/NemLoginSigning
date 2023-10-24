using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using NemLoginSigningDTO.UUIDMatch;
using NemLoginSigningCore.Configuration;
using NemLoginSigningWebApp.Logic;

namespace NemLoginSigningWebApp.Controllers
{
    [ApiController]
    [Route("UUIDMatch")]
    public class UUIDMatchController : ControllerBase
    {
        private readonly IUUIDMatchClient _matchClient;

        public UUIDMatchController(IUUIDMatchClient matchClient)
        {
            _matchClient = matchClient;
        }

        [HttpPost]
        [Route("SubjectMatchesSigner")]
        public async Task<IActionResult> SubjectMatchesSigner(SubjectMatchesSignerDTO dto)
        {
            return Ok(await _matchClient.SubjectMatchesSigner(dto));
        }
    }
}
