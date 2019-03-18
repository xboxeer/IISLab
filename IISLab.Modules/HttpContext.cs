using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IISLab.Web
{
    public class HttpContext
    {
        public HttpRequest Request { get; private set; }
        public HttpResponse Response { get; private set; }

        public HttpContext(string requestData)
        {
            Request = this.GetHttpRequest(requestData);
            
        }

        private HttpRequest GetHttpRequest(string requestData)
        {
            var request = new HttpRequest();
            return request;
        }
    }
}
