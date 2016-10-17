using ElasticMDWiki.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace ElasticMDWiki.Net.Controllers
{
    [RoutePrefix("markdown")]
    public class MarkdownController : ApiController
    {
        public ContentRepository ContentRepository { get; private set; }

        public MarkdownController()
            : this(new ContentRepository())
        { }

        public MarkdownController(ContentRepository contentRepository)
        {
            ContentRepository = contentRepository;
        }

        [HttpGet]
        [Route("{*path}")]
        public HttpResponseMessage GET(string path)
        {
            HttpResponseMessage response = null;

            if (!path.EndsWith(".md"))
            {
                path += ".md";
            }
            var doc = ContentRepository.Get(path);

            string markdown = $"This page doesn't have any content yet. Would you like to be the first to edit it? (Click Here)[editor/{path}]";

            if (doc != null)
            {
                markdown = ASCIIEncoding.UTF8.GetString(doc.ContentBytes);
            }

            response = Request.CreateResponse(HttpStatusCode.OK, new { text = markdown, tags = doc.Tags });
            response.Content.Headers.ContentType.MediaType = "application/json";

            return response;
        }

        [HttpPost]
        [Route("{*path}")]
        public IHttpActionResult Post(string path, [FromBody]UploadMarkdownRequest request)
        {
            if (request == null)
            {
                return BadRequest("null request");
            }

            if (!string.Equals(".md", Path.GetExtension(path)))
            {
                path += ".md";
            }

            ContentRepository.Update(new Content()
            {
                Name = path,
                ContentBytes = Encoding.UTF8.GetBytes(request.Text),
                ContentText = request.Text,
                LastModified = DateTime.UtcNow,
                PhysicalPath = $"es://{path}",
                Version = 0,
                Tags = request.Tags.ToList()
            });

            return Ok(new { status = "sucess" });
        }
    }
}
