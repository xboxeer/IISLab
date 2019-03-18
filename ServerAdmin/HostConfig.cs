using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using System.Xml;
using System.IO;
using ServerAdmin;
using System.Text.RegularExpressions;

namespace IISLab.ServerAdmin
{
    public class HostConfig
    {
        public const string IISROOTVAR = "AppHostConfigPath";

        public List<NativeModuleConfig> GlobalModules = new List<NativeModuleConfig>();

        public List<Site> Sites { get; private set; }

        public Dictionary<string,string> StaticContentMapping { get; set; }

        private static ILog ilog = LogManager.GetLogger(typeof(HostConfig));

        private static HostConfig hostConfigInstance = null;
        
        public static HostConfig GetHostConfig()
        {
            if (hostConfigInstance == null)
            {
                hostConfigInstance = ReadConfig();
            }
            return hostConfigInstance;
        }

        private static HostConfig ReadConfig()
        {
            var hostConfig = new HostConfig()
            {
                Sites = new List<Site>(),
                StaticContentMapping=new Dictionary<string, string>()
            };
            var configPath =Path.Combine(System.Environment.GetEnvironmentVariable(IISROOTVAR),"AppHost.config");
            ilog.Info($"Get config from {configPath}");
            XmlDocument appHostConfig = new XmlDocument();
            appHostConfig.Load(configPath);
            
            foreach (XmlNode globalModule in appHostConfig.SelectNodes("/configuration/system.webServer/globalModules/add"))
            {
                var name = globalModule.SelectSingleNode("@name")?.Value;
                var image = globalModule.SelectSingleNode("@image")?.Value;
                hostConfig.GlobalModules.Add(new NativeModuleConfig()
                {
                    Name = name,
                    Image = image
                });
            }

            foreach (XmlNode staticContent in appHostConfig.SelectNodes("/configuration/system.webServer/staticContent/mimeMap"))
            {
                var fileExtension = staticContent.SelectSingleNode("@fileExtension").Value;
                var mimeType = staticContent.SelectSingleNode("@mimeType").Value;
                hostConfig.StaticContentMapping.Add(fileExtension, mimeType);
            }

            foreach (XmlNode site in appHostConfig.SelectNodes("/configuration/system.applicationHost/sites/site"))
            {
                var name = site.SelectSingleNode("@name")?.Value;
                var id = site.SelectSingleNode("@id")?.Value;
                var appPool = site.SelectSingleNode("application/@applicationPool")?.Value;
                var binding = site.SelectSingleNode("bindings/binding[1]/@bindingInformation")?.Value;
                var physicalPath = site.SelectSingleNode("application/virtualDirectory/@physicalPath")?.Value;
                var port = binding.Substring(binding.IndexOf(":") + 1, binding.LastIndexOf(":") - binding.IndexOf(":")-1);
                var host = binding.Substring(binding.LastIndexOf(":") + 1, binding.Length - binding.LastIndexOf(":") - 1);
                var ip = binding.Substring(0, binding.IndexOf(":"));
                var newSite = new Site()
                {
                    Name = name,
                    ID = id,
                    IP = ip=="*"?"127.0.0.1":ip,
                    Host = host,
                    Port = port,
                    PhysicalPath = physicalPath,
                    AppPool= appPool
                };
                newSite.GlobalNativeModules = hostConfig.GlobalModules;
                hostConfig.Sites.Add(newSite);
            }
            return hostConfig;
        }

        private HostConfig() { }

        public Site GetRequestingSite(string request)
        {
            Regex regex = new Regex(@"Host:\s?(\w+):(\d+)");
            var hostLine = regex.Match(request).Value;
            var hostDetails = hostLine.Split(':');
            var hostName = hostDetails[1].Trim();
            var port = hostDetails[2].Trim();
            var site = this.Sites.Where(item => item.Host == hostName && item.Port == port).FirstOrDefault();
            return site;
        }
    }
}
