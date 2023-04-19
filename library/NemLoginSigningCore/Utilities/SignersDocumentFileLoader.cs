using System;
using System.IO;
using System.Net;

namespace NemLoginSigningCore.Utilities
{
    public class SignersDocumentFileLoader
    {
        public static byte[] LoadDataFromFileFromPath(string path)
        {
            return File.ReadAllBytes(path);
        }

        public static byte[] LoadDataFromUri(Uri uri)
        {
            using (WebClient client = new WebClient())
            {
                return client.DownloadData(uri);
            }
        }
    }
}