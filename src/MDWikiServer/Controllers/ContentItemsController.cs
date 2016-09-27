using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace ElasticMDWiki.Net.Controllers
{
    [RoutePrefix("content-items")]
    public class ContentItemController : ApiController
    {
        public ContentRepository ContentRepository { get; private set; }

        public ContentItemController()
            : this(new ContentRepository())
        { }

        public ContentItemController(ContentRepository contentRepository)
        {
            ContentRepository = contentRepository;
        }

        [HttpGet]
        [Route()]
        public IHttpActionResult Get(int skip = 0, int take = 10)
        {
            var items = ContentRepository.GetAll(skip, take);

            return Ok(items.Select(x => new { fileName = x.Name, length = x.Length, LastModified = x.LastModified }));
        }

        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> Upload()
        {
            if (!Request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

            var provider = new MultipartMemoryStreamProvider();
            await Request.Content.ReadAsMultipartAsync(provider);
            foreach (var file in provider.Contents)
            {
                var filename = file.Headers.ContentDisposition.FileName.Trim('\"');
                var buffer = await file.ReadAsByteArrayAsync();
                ContentRepository.Update(new Content()
                {
                    Name = filename,
                    ContentBytes = buffer,
                    LastModified = DateTime.UtcNow,
                    PhysicalPath = $"es://{filename}",
                    Version = 0
                });
            }

            return Ok();
        }
    }
}
