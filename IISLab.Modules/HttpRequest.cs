using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IISLab.Web
{
    public class HttpRequest
    {
        private HttpApplication state;
        public string ProcessRequest(string requestData)
        {
            var httpContext = new HttpContext(requestData);
            if (state == null)
            {
                state = HttpApplicationFactory.GetHttpApplication();
            }
            return string.Empty;
        }


    }
}
