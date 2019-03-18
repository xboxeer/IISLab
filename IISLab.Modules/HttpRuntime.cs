using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IISLab.Web
{
    public class HttpRuntime
    {
        private static HttpRuntime runtime = new HttpRuntime();

        private HttpRuntime() { }

        public static HttpRuntime GetHttpRuntime()
        {
            return runtime;
        }

        public HttpApplication HttpApplication { get; set; }


        public HttpRuntime(HttpNativeRuntime nativeRuntime)
        {
            HttpApplication = HttpApplicationFactory.GetHttpApplication();          
        }
    }
}
