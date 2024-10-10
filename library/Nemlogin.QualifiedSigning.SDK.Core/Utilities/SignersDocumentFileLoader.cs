using System.Net;

namespace Nemlogin.QualifiedSigning.SDK.Core.Utilities;

public class SignersDocumentFileLoader
{
    public static byte[] LoadDataFromFileFromPath(string path)
    {
        return File.ReadAllBytes(path);
    }

    public static byte[] LoadDataFromUri(Uri uri)
    {
        using WebClient client = new WebClient();
        return client.DownloadData(uri);
    }
}