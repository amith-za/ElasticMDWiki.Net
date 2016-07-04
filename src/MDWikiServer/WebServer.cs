using Microsoft.Owin.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticMDWiki.Net
{
    public class WebServer : IDisposable
    {

        IDisposable _webApp = null;
        int _port = 0;
        string _baseAddress = null;


        public WebServer(int port)
        {
            this._port = port;
            _baseAddress = string.Format("http://*:{0}", port);
        }

        ~WebServer()
        {
            Dispose(false);
        }

        public void Start()
        {
            _webApp = WebApp.Start<Startup>(_baseAddress);
        }

        public void Stop()
        {
            Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_webApp != null)
                {
                    _webApp.Dispose();
                    _webApp = null;
                }
            }
        }

    }
}
