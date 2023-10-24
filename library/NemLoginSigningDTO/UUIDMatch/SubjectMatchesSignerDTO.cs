namespace NemLoginSigningDTO.UUIDMatch
{
    public class SubjectMatchesSignerDTO
    {
        public string SubjectNameID { get; set; }

        public string SignerSubjectSerialNumber { get; set; }

        public string ServiceProviderEntityID { get; set; }
    }

    public enum SubjectMatchesSignerResult
    {
        Match,
        NoMatch,
        SubjectNotFound,
        SerialNotFound,
    }
}
