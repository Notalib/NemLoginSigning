using System.Threading.Tasks;

using NemLoginSigningDTO.UUIDMatch;

namespace NemLoginSigningWebApp.Logic
{
    public interface IUUIDMatchClient
    {
        Task<SubjectMatchesSignerResult> SubjectMatchesSigner(SubjectMatchesSignerDTO request);
    }
}