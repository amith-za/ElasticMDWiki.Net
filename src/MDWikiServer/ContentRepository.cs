using Nest;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticMDWiki.Net
{
    public class ContentRepository
    {
        const string indexName = "contentitems";
        const string typeName = "contentitem";

        public Content Get(string fileName)
        {
            var client = CreateClient();
            var response =
                client.Get<Content>(
                    new GetRequest(indexName,
                                   new TypeName()
                                   {
                                       Type = typeof(Content),
                                       Name = typeName
                                   }, new Id(fileName)));

            if (response.ApiCall.HttpStatusCode != 200 &
                response.ApiCall.HttpStatusCode != 404)
            {
                throw new Exception(response.ApiCall.HttpStatusCode.ToString());
            }

            return response.Source;
        }

        public void Update(Content document)
        {
            var client = CreateClient();

            var response =
                client.Index(document, i => i.Index(indexName).Type(typeName).Id(document.Name));

            if (response.ApiCall.HttpStatusCode != 200 &
                response.ApiCall.HttpStatusCode != 201)
            {
                throw new Exception($"Failed to save '{document.Name}'");
            }
        }

        ElasticClient CreateClient()
        {
            var cs = new ConnectionSettings(new Uri(Settings.ElasticHost));

            return new ElasticClient(cs);
        }
    }
}
