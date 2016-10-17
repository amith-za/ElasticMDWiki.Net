using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticMDWiki.Net.Controllers
{
    public class UploadMarkdownRequest
    {
        public string Text { get; set; }

        public string[] Tags { get; set; }
    }
}
