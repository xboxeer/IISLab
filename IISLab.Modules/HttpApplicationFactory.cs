using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IISLab.Web
{
    public static class HttpApplicationFactory
    {
        private static object lockObj = new object();

        private static List<HttpApplication> httpApplicationPool = new List<HttpApplication>();
        
        public static HttpApplication GetHttpApplication()
        {
            lock (lockObj)
            {
                var httpApplication = httpApplicationPool.Where(httpAppPool => httpAppPool.Status == HttpApplcationStatus.Idel).FirstOrDefault();
                if (httpApplication == null)
                {
                    httpApplication = new HttpApplication()
                    {
                        Status = HttpApplcationStatus.Idel
                    };
                    httpApplicationPool.Add(httpApplication);
                }
                return httpApplication;
            }
            
        }
    }

    
}
