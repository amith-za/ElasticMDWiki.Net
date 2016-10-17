using Microsoft.Owin.FileSystems;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticMDWiki.Net
{
    public class Content : IFileInfo
    {
        public bool IsDirectory
        {
            get
            {
                return false;
            }
        }

        public DateTime LastModified { get; set; }

        public long Length
        {
            get
            {
                return ContentBytes == null ? 0 : ContentBytes.Length;
            }
        }

        public string Name { get; set; }

        public string PhysicalPath { get; set; }

        public byte[] ContentBytes { get; set; }

        public string ContentText { get; set; }

        public long Version { get; set; }

        public List<string> Tags { get; set; }

        public Stream CreateReadStream()
        {
            return new MemoryStream(ContentBytes);
        }
    }
}
