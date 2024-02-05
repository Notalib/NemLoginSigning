using System;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;
using NemLoginSigningCore.Logging;
using NemLoginSigningCore.Utilities;

namespace NemLoginSigningCore.Core
{
    /// <summary>
    ///  Wraps the actual file of a SignersDocument}.
    /// </summary>
    public class SignersDocumentFile
    {
        public static readonly string DEFAULTNAME = "unnamed";
        public static readonly int MAXFILESIZE = 1024 * 1024 * 20; // 20 mb

        private byte[] _data;

        public DateTime? CreationTime { get; protected set; }

        public DateTime? LastModified { get; protected set; }

        public string Name { get; protected set; }

        // One of the following file sources
        public string Path { get; protected set; }

        public Uri Uri { get; set; }

        public byte[] GetData()
        {
            if (_data == null)
            {
                if (!string.IsNullOrEmpty(Path))
                {
                    _data = SignersDocumentFileLoader.LoadDataFromFileFromPath(Path);
                }

                if (Uri != null)
                {
                    _data = SignersDocumentFileLoader.LoadDataFromUri(Uri);
                }
            }

            return _data;
        }

        public string GetDataAsString()
        {
            return Encoding.UTF8.GetString(GetData());
        }

        public class SignersDocumentFileBuilder
        {
            private SignersDocumentFile _builderTemplate;

            public SignersDocumentFileBuilder()
            {
                _builderTemplate = new SignersDocumentFile();
            }

            public SignersDocumentFileBuilder WithCreationTime(DateTime? creationTime)
            {
                _builderTemplate.CreationTime = creationTime;
                return this;
            }

            public SignersDocumentFileBuilder WithLastModified(DateTime? lastModified)
            {
                _builderTemplate.LastModified = lastModified;
                return this;
            }

            public SignersDocumentFileBuilder WithName(string name)
            {
                _builderTemplate.Name = name;
                return this;
            }

            public SignersDocumentFileBuilder WithPath(string path)
            {
                _builderTemplate.Path = path;
                return this;
            }

            public SignersDocumentFileBuilder WithUri(Uri uri)
            {
                _builderTemplate.Uri = uri;
                return this;
            }

            public SignersDocumentFileBuilder WithData(byte[] data)
            {
                _builderTemplate._data = data;
                return this;
            }

            public SignersDocumentFile Build()
            {
                TrySetCreationTime();

                TrySetLastModified();

                TrySetName();

                return _builderTemplate;
            }

            private void TrySetName()
            {
                if (string.IsNullOrEmpty(_builderTemplate.Name))
                {
                    if (!string.IsNullOrEmpty(_builderTemplate.Path))
                    {
                        _builderTemplate.Name = System.IO.Path.GetFileName(_builderTemplate.Path);
                    }
                    else if (_builderTemplate.Uri != null && _builderTemplate.Uri.IsFile)
                    {
                        _builderTemplate.Name = System.IO.Path.GetFileName(_builderTemplate.Uri.LocalPath);
                    }
                    else
                    {
                        _builderTemplate.Name = DEFAULTNAME;
                    }
                }
            }

            private void TrySetLastModified()
            {
                var logger = LoggerCreator.CreateLogger<SignersDocumentFile>();

                if (_builderTemplate.LastModified == null && !string.IsNullOrEmpty(_builderTemplate.Path))
                {
                    try
                    {
                        _builderTemplate.LastModified = File.GetLastWriteTime(_builderTemplate.Path);
                    }
                    catch (Exception)
                    {
                        logger.LogWarning($"Cannot read lastmodified time of file: {_builderTemplate.Path}");
                    }
                }
            }

            private void TrySetCreationTime()
            {
                var logger = LoggerCreator.CreateLogger<SignersDocumentFile>();

                if (_builderTemplate.CreationTime == null && !string.IsNullOrEmpty(_builderTemplate.Path))
                {
                    try
                    {
                        _builderTemplate.CreationTime = File.GetCreationTime(_builderTemplate.Path);
                        logger.LogWarning("Cannot read creation time of file: ");
                    }
                    catch (Exception)
                    {
                        logger.LogWarning($"Cannot read creation time of file: {_builderTemplate.Path}");
                    }
                }
            }
        }
    }
}
