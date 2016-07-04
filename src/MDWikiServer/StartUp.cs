using Owin;
using Swashbuckle.Application;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace ElasticMDWiki.Net
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            var config = new HttpConfiguration();

            var appXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);

            var formatters = config.Formatters;
            var jsonFormatter = formatters.JsonFormatter;
            var settings = jsonFormatter.SerializerSettings;
            settings.Formatting = Newtonsoft.Json.Formatting.Indented;
            settings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();

            config.EnableSwagger(
                c =>
                {
                    c.SingleApiVersion("v1", "ElasticMDWiki.Net API");
                })
            .EnableSwaggerUi();

            config.MapHttpAttributeRoutes();

            var fileSystem = new Microsoft.Owin.FileSystems.EmbeddedResourceFileSystem(@"ElasticMDWiki.Net.StaticContent");

            var contentTypeProvider = new Microsoft.Owin.StaticFiles.ContentTypes.FileExtensionContentTypeProvider();
            contentTypeProvider.Mappings.Add(".md", "text/markdown");

            appBuilder.UseStaticFiles(
                new Microsoft.Owin.StaticFiles.StaticFileOptions
                {
                    ServeUnknownFileTypes = true,
                    FileSystem = new HybridElasticsearchFileSystem(fileSystem, new ContentRepository()),
                    ContentTypeProvider = contentTypeProvider
                });

            // TODO: Figure out how to make this work. Does not seem to work with HybridElasticsearchFileSystem for some reason.
            //appBuilder.UseDefaultFiles(
            //    new Microsoft.Owin.StaticFiles.DefaultFilesOptions
            //    {
            //        DefaultFileNames = new List<string> { "index.html" }
            //    });

            appBuilder.UseWebApi(config);
        }
    }
}
