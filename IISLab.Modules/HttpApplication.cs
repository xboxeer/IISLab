using System;
using System.Collections.Generic;

namespace IISLab.Web
{
    /// <summary>
    /// In IIS6 HttpApplication would init managed module here, but in IIS7, managed module is handlered in intergrated pipeline
    /// and run against Managed Engine
    /// </summary>
    public class HttpApplication
    {
        public HttpApplcationStatus Status { get; set; }

        private void ExecuteRequest(HttpContext context)
        {

        }

        
    }
    public enum HttpApplcationStatus
    {
        Run,
        Idel
    }
}