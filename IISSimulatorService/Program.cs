using IISLab.ServerAdmin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using IISLab.W3Svc;

namespace IISLab.IISSimulatorService
{
    class Program
    {

        static void Main(string[] args)
        {
            System.Environment.SetEnvironmentVariable(HostConfig.IISROOTVAR, Directory.GetCurrentDirectory());
            var log = LogManager.GetLogger("IISSimulatorService.main");
            log.Info("Start WAS");
            var appPool = new WAS.ApplicationPoolService();
            log.Info("WAS Prepare HostConfig");
            appPool.InitConfig();
            log.Info("Starting W3Svc");
            W3Svc.HTTPListener.HostConfig = appPool.HostConfig;
            Task.Run(()=>W3Svc.HTTPListener.StartListen());
            log.Info("W3Svc started");
            Console.Read();
        }
    }
}
