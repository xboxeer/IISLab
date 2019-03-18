using ServerAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IISLab.ServerAdmin
{
    public class Site
    {
        public string Name { get; set; }

        public string ID { get; set; }

        public string IP { get; set; }

        public string Host { get; set; }

        public string Port { get; set; }

        public string Protocol { get; set; }

        public string PhysicalPath { get; set; }

        public string AppPool { get; set; }

        public List<NativeModuleConfig> GlobalNativeModules { get; set; }

        public List<ManagedModuleConfig> Modules { get; set; }

        //public List<IHttpHandler> Handlers { get; set; }
    }
}
