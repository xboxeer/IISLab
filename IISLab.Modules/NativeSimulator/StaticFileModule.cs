using IISLab.ServerAdmin;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IISLab.Web.NativeSimulator
{
    /// <summary>
    /// In real IIS, a native module actually expose a RegisterModule Function for IIS to execute the request
    /// </summary>
    public class StaticFileModule : INativeHttpModuleSimulator
    {
        private ILog log = LogManager.GetLogger(typeof(StaticFileModule));

        public StaticFileModule() { }

        public void RegisterNativeModule(HttpNativeRuntime nativeRuntime)
        {
            log.Info($"Register Native Module in PipeLine {this.GetType().ToString()}");
            nativeRuntime.ExecuteHandler += NativeRuntime_ExecuteHandler;
        }

        private void NativeRuntime_ExecuteHandler(object sender, HttpNativeContext e)
        {
            log.Info($"{this.GetType().ToString()} executing Handler");
            var request = e.Request;
            var lines = request.Replace("\r\n", "\r").Split('\r');
            var requestLines = lines[0].Split(' ');
            var httpMethod = requestLines[0];
            var url = requestLines[1].Replace("/","\\");
            Console.WriteLine($"Requesting URL:{url}, Requesting Site PhysicalPaht: {e.Runtime.PhysicalPath}");
            var resourcePath = e.Runtime.PhysicalPath + url;
            Console.WriteLine($"Mapp request to {resourcePath}");
            if (!File.Exists(resourcePath))
            {
                var body = Encoding.UTF8.GetBytes("404 Not Found");
                e.ResponseHeader = this.GetResponse("404", "Not Found", "text/html", body);
                e.ResponseBody = body;
            }
            else
            {
                ///I assume static file module in real IIS would skip the extension that already registed by other handler
                var extensionName = Path.GetExtension(resourcePath);
                var config = HostConfig.GetHostConfig();
                if (config.StaticContentMapping.ContainsKey(extensionName.Trim()))
                {
                    log.Info($"Request Resource is {extensionName}, StaticFileModule takes over it");
                    var mimeType = config.StaticContentMapping[extensionName];
                    var body = File.ReadAllBytes(resourcePath);
                    var responseHeader = this.GetResponse("200", "ok", mimeType, body);
                    e.ResponseHeader = responseHeader;
                    e.ResponseBody = body;
                }
                else
                {
                    var body = Encoding.UTF8.GetBytes($"500 No Handler For {extensionName} file");
                    e.ResponseHeader = this.GetResponse("500", "Internal Error", "text/html", body);
                    e.ResponseBody = body;
                }
            }
            
        }

        private string GetResponse(string stateCode,string StateDescription,string ContentType,byte[] body)
        {
            string strResponse = string.Format(@"HTTP/1.1 {0} {1}
Content-Type: {2}
Accept-Ranges: bytes
Server: IISLab/0.1
X-Powered-By: IISLab
Date: {3} 
Content-Length: {4}

", stateCode, StateDescription, ContentType, string.Format("{0:R}", DateTime.Now), body.Length);

            return strResponse;

        }
    }
}
