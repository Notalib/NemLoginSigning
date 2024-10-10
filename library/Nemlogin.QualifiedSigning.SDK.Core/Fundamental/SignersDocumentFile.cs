using System.Text;
using Nemlogin.QualifiedSigning.SDK.Core.Utilities;

namespace Nemlogin.QualifiedSigning.SDK.Core.Fundamental;

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
            private SignersDocumentFile builderTemplate;

            public SignersDocumentFileBuilder()
            {
                builderTemplate = new SignersDocumentFile();
            }

            public SignersDocumentFileBuilder WithCreationTime(DateTime? creationTime)
            {
                builderTemplate.CreationTime = creationTime;
                return this;
            }

            public SignersDocumentFileBuilder WithLastModified(DateTime? lastModified)
            {
                builderTemplate.LastModified = lastModified;
                return this;
            }

            public SignersDocumentFileBuilder WithName(string name)
            {
                builderTemplate.Name = name;
                return this;
            }

            public SignersDocumentFileBuilder WithPath(string path)
            {
                builderTemplate.Path = path;
                return this;
            }

            public SignersDocumentFileBuilder WithUri(Uri uri)
            {
                builderTemplate.Uri = uri;
                return this;
            }

            public SignersDocumentFileBuilder WithData(byte[] data)
            {
                builderTemplate._data = data;
                return this;
            }

            public SignersDocumentFile Build()
            {
                TrySetCreationTime();
                
                TrySetLastModified();
                
                TrySetName();

                return builderTemplate;
            }
            
            private void TrySetName()
            {
                if (string.IsNullOrEmpty(builderTemplate.Name))
                {
                    if (!string.IsNullOrEmpty(builderTemplate.Path))
                    {
                        builderTemplate.Name = System.IO.Path.GetFileName(builderTemplate.Path);
                    }
                    else if (builderTemplate.Uri != null && builderTemplate.Uri.IsFile)
                    {
                        builderTemplate.Name = System.IO.Path.GetFileName(builderTemplate.Uri.LocalPath);
                    }
                    else
                    {
                        builderTemplate.Name = DEFAULTNAME;
                    }
                }
            }

            private void TrySetLastModified()
            {
                if (builderTemplate.LastModified == null && !string.IsNullOrEmpty(builderTemplate.Path))
                {
                    try
                    {
                        builderTemplate.LastModified = File.GetLastWriteTime(builderTemplate.Path);
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }

            private void TrySetCreationTime()
            {
                if (builderTemplate.CreationTime == null && !string.IsNullOrEmpty(builderTemplate.Path))
                {
                    try
                    {
                        builderTemplate.CreationTime = File.GetCreationTime(builderTemplate.Path);
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
        }
    }