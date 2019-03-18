using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IISLab.Web
{
    public class ManagedEngine : INativeHttpModuleSimulator
    {
        public void RegisterNativeModule(HttpNativeRuntime context)
        {
            HttpRuntime runtime = HttpRuntime.GetHttpRuntime();
            
            foreach(var managedModule in context.ManagedModules)
            {
                managedModule.Init(runtime.HttpApplication);
            }
        }
    }
}
