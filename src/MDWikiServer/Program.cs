using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace ElasticMDWiki.Net
{
    class Program
    {
        WebServer _server;

        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<Program>(s =>
                {
                    s.ConstructUsing(hostSettings => new Program());
                    s.WhenStarted(hostControl => hostControl.Start());
                    s.WhenStopped(hostControl => hostControl.Stop());
                });

                x.SetDescription("ElasticMDWikiDotNet");
                x.SetDisplayName("ElasticMDWikiDotNet");
                x.SetServiceName("ElasticMDWikiDotNet");

                x.RunAsNetworkService();
                x.StartAutomatically();
            });
        }

        public void Start()
        {
            _server = new WebServer(Settings.Port);
            _server.Start();

            Debug.WriteLine("");
            Debug.WriteLine($"ElasticMDWiki.Net Web Server Up and listening");
            Debug.WriteLine("");
            Debug.WriteLine($"Configuration");
            Debug.WriteLine($"-----------------------------------------------------");
            Debug.WriteLine($"Port          : {Settings.Port}");
            Debug.WriteLine($"Elastic Host  : {Settings.ElasticHost}");
            Debug.WriteLine($"-----------------------------------------------------");
            Debug.WriteLine("");
            Debug.WriteLine($"Get started at http://localhost:{Settings.Port}/index.html");
            Debug.WriteLine($"If you haven't setup navigation.md as yet set it up at http://localhost:{Settings.Port}/editor?file=navigation.md");
            Debug.WriteLine("");
        }

        public void Stop()
        {
            _server.Dispose();
            Debug.WriteLine($"ElasticMDWiki.Net Web Server stopped");
        }
    }
}
