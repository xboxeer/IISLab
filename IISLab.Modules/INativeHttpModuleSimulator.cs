using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IISLab.Web
{
    /// <summary>
    /// A native Module written actually expose a function named RegisteModule
    /// Refer to https://docs.microsoft.com/en-us/iis/develop/runtime-extensibility/develop-a-native-cc-module-for-iis
    /// </summary>
    public interface INativeHttpModuleSimulator
    {
        void RegisterNativeModule(HttpNativeRuntime nativeRuntime);
    }
}
