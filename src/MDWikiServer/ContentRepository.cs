﻿using Nest;
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
                    new GetRequest(indexName, typeName, fileName));

            if (response.ApiCall.HttpStatusCode != 200 &
                response.ApiCall.HttpStatusCode != 404)
            {
                throw new Exception(response.ApiCall.HttpStatusCode.ToString());
            }

            return response.Source;
        }

        public List<Content> GetAll(int skip, int take)
        {
            var client = CreateClient();

            var response = client.Search<Content>(
                s => s.Index(indexName)
                      .Type(typeName)
                      .MatchAll()
                      .Skip(skip)
                      .Take(take));

            if (response.ApiCall.HttpStatusCode != 200)
            {
                throw new Exception(response.ApiCall.HttpStatusCode.ToString());
            }

            return response.Hits.Select(h =>
            {
                h.Source.Version = h.Version.HasValue ? h.Version.Value : 0;
                return h.Source;
            }).ToList();
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

        public void Delete(string path)
        {
            var client = CreateClient();

            var response =
                client.Delete(new DeleteRequest(indexName, typeName, path));

            if (response.ApiCall.HttpStatusCode != 200)
            {
                throw new Exception($"Failed to delete '{path}', response code {response?.ApiCall?.HttpStatusCode}");
            }
        }

        public bool Exists(string path)
        {
            var client = CreateClient();

            var response = client.DocumentExists(new DocumentExistsRequest(indexName, new TypeName()
            {
                Type = typeof(Content),
                Name = typeName
            },
            new Id(path)));

            if (response.ApiCall.HttpStatusCode != 200)
            {
                throw new Exception($"Failed to check if document '{path}' exists");
            }

            return response.Exists;
        }

        ElasticClient CreateClient()
        {
            var cs = new ConnectionSettings(new Uri(Settings.ElasticHost));

            return new ElasticClient(cs);
        }
    }
}
