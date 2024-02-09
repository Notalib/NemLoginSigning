using System;
using System.IO;
using System.Net.Http;

namespace NemLoginSigningCore.Utilities
{
    public static class SignersDocumentFileLoader
    {
        public static byte[] LoadDataFromFileFromPath(string path)
        {
            return File.ReadAllBytes(path);
        }

        public static byte[] LoadDataFromUri(Uri uri)
        {
            using HttpClient client = new();
            return client.GetByteArrayAsync(uri).ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}
