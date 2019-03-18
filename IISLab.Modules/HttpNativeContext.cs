using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IISLab.Web
{
    public class HttpNativeContext
    {
        public string Request { get; set; }

        public string ResponseHeader { get; set; }

        public HttpNativeRuntime Runtime { get; set; }

        public byte[] ResponseBody { get; set; }
    }
}
