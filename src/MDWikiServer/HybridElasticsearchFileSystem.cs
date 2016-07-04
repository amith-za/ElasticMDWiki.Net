using Microsoft.Owin.FileSystems;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ElasticMDWiki.Net
{
    public class HybridElasticsearchFileSystem : IFileSystem
    {

        IFileSystem _innerFilesystem = null;
        ContentRepository _contentRepository = null;

        public HybridElasticsearchFileSystem(IFileSystem innerFileSystem, ContentRepository contentRepository)
        {
            _innerFilesystem = innerFileSystem;
            _contentRepository = contentRepository;
        }

        public bool TryGetDirectoryContents(string subpath, out IEnumerable<IFileInfo> contents)
        {
            return _innerFilesystem.TryGetDirectoryContents(subpath, out contents);
        }

        public bool TryGetFileInfo(string subpath, out IFileInfo fileInfo)
        {
            _innerFilesystem.TryGetFileInfo(subpath, out fileInfo);

            if (fileInfo != null)
            {
                return true;
            }

            var fileName = Path.GetFileName(subpath);

            if (string.IsNullOrWhiteSpace(fileName))
            {
                return false;
            }

            var content = _contentRepository.Get(fileName);

            if (content != null)
            {
                if (string.Equals(".md", Path.GetExtension(subpath), StringComparison.InvariantCultureIgnoreCase) &
                    !string.Equals("navigation", Path.GetFileNameWithoutExtension(subpath), StringComparison.InvariantCultureIgnoreCase))
                {
                    var editorlink = $"\n\nTo edit this page [click here](../editor.html?page={Path.GetFileNameWithoutExtension(fileName)})";

                    var bytes = new List<byte>();
                    bytes.AddRange(content.ContentBytes);
                    bytes.AddRange(ASCIIEncoding.UTF8.GetBytes(editorlink));
                    content.ContentBytes = bytes.ToArray();
                }

                fileInfo = content;
                return true;
            }

            if (string.Equals(".md", Path.GetExtension(subpath)))
            {
                var editorlink = $"This page doesn't have any content yet. Would you like to be the first to edit it? [Click Here](../editor.html?page={Path.GetFileNameWithoutExtension(fileName)})";
                fileInfo = new Content
                {
                    ContentBytes = ASCIIEncoding.UTF8.GetBytes(editorlink),
                    LastModified = DateTime.UtcNow,
                    Name = fileName,
                    PhysicalPath = $"es://{fileName}",
                    Version = 0
                };

                return true;
            }

            return false;
        }
    }

}
