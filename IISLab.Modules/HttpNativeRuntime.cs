using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IISLab.Web
{
    /// <summary>
    /// Let's consider HttpNativeRuntime as a native component of IIS that handles the IIS request pipeline
    /// </summary>
    public class HttpNativeRuntime
    {
        


        public event EventHandler<HttpNativeContext> ExecuteHandler;

        public List<INativeHttpModuleSimulator> NativeModules { get; set; }

        public List<IManagedHttpModule> ManagedModules { get; set; }

        public string PhysicalPath { get; set; }
        public HttpNativeRuntime(List<INativeHttpModuleSimulator> nativeModules,List<IManagedHttpModule> managedModules)
        {
            this.NativeModules = nativeModules;
            this.ManagedModules = managedModules;
            foreach (var nativeModule in this.NativeModules)
            {
                nativeModule.RegisterNativeModule(this);
            }
            
        }

        public void MapRequestHandler()
        {

        }

        public byte[] ProcessRequest(string request)
        {
            var nativeContext = new HttpNativeContext()
            {
                Request = request,
                ResponseHeader = string.Empty,
                Runtime = this
            };
            var managedRuntime = HttpRuntime.GetHttpRuntime();
            this.ExecuteHandler(this, nativeContext);
            var headerBytes = Encoding.UTF8.GetBytes(nativeContext.ResponseHeader);
            Console.WriteLine(nativeContext.ResponseHeader);
            var response = new byte[headerBytes.Length + nativeContext.ResponseBody.LongLength];
            headerBytes.CopyTo(response, 0);
            nativeContext.ResponseBody.CopyTo(response, headerBytes.Length);
            return response;
            //return nativeContext;
        }
    }
}
