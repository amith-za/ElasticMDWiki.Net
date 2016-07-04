using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticMDWiki.Net
{
    public static class Settings
    {
        public static int Port
        {
            get
            {
                var port = 0;

                if (!Int32.TryParse(ConfigurationManager.AppSettings["port"] ?? "8081", out port))
                {
                    port = 8081;
                }

                return port;
            }
        }

        public static string ElasticHost
        {
            get
            {
                return ConfigurationManager.AppSettings["elastic:host"] ?? "http://localhost:9200";
            }
        }

    }
}
