using System;
using System.IO;
using System.Linq;

namespace NemLoginSigningDTO.Signing;

public class SigningDocumentDTO
{
    /// <summary>
    /// Accepted filetypes extentions.
    /// </summary>
    private static readonly string[] ValidFileExtensions = { ".TXT", ".PDF", ".XML", ".HTML", ".HTM" };

    /// <summary>
    /// Name of document file that is to be signed
    /// </summary>
    public string FileName { get; set; }

    /// <summary>
    /// Document content in Base64
    /// </summary>
    public string EncodedContent { get; set; }

    /// <summary>
    /// Gets Document content size without base64 encoding
    /// </summary>
    public int UnencodedSize => EncodedContent.Length * 3 / 4;

    /// <summary>
    /// Get if ContentsBase64 is too large to submit to NemLogin
    ///
    /// 3.1.1: Signer’s Document Size Restriction
    /// The SD must have a size of at most 20 MB.
    /// </summary>
    public bool IsContentTooLarge => UnencodedSize > 20 * 1024 * 1024;

    /// <summary>
    /// Gets the decoded document content stored in ContentsBase64
    /// </summary>
    /// <returns>Document content as a byte[]</returns>
    public byte[] GetContent()
    {
        return Convert.FromBase64String(EncodedContent);
    }

    /// <summary>
    /// Checks if DTO is well formed.
    /// </summary>
    /// <returns></returns>
    public bool Validate()
    {
        return !string.IsNullOrWhiteSpace(EncodedContent) && !string.IsNullOrWhiteSpace(FileName) && ValidFileExtensions.Contains(Path.GetExtension(FileName).ToUpperInvariant());
    }
}
