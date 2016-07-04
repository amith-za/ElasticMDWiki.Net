using Nest;
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
    [RoutePrefix("search")]
    public class SearchController : ApiController
    {
        [HttpGet]
        [Route("")]
        public HttpResponseMessage Search(string query)
        {
            var client = CreateClient();

            var searchResults
                = client.Search<Content>(
                    s => s.Index("contentitems")
                          .Type("contentitem")
                          .Source(false)
                          .Highlight(h => h.Fields(hf => hf.Field(hff => hff.ContentText)).Order("score").FragmentSize(60))
                          .Query(q => q.SimpleQueryString(sq => sq.Fields(f => f.Field(c => c.ContentText)).Query(query))));

            var results = from highlight in searchResults.Highlights
                          from t in highlight.Value["contentText"].Highlights
                          select new { page = highlight.Key, highlight = t };

            return Request.CreateResponse(HttpStatusCode.OK, results);
        }

        ElasticClient CreateClient()
        {
            var cs = new ConnectionSettings(new Uri(Settings.ElasticHost));

            return new ElasticClient(cs);
        }
    }
}
