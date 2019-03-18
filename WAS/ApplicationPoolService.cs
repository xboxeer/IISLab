using IISLab.ServerAdmin;
using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IISLab.WAS
{
    public class ApplicationPoolService
    {
        private string workerProcessPath = Path.Combine(System.Environment.GetEnvironmentVariable(HostConfig.IISROOTVAR), "WorkerProcess.exe");

        private Dictionary<string, int> appPool = new Dictionary<string, int>();

        private object lockObj = new object();

        private ILog log = LogManager.GetLogger(typeof(ApplicationPoolService));

        public HostConfig HostConfig { get; private set; }

        public string ProcessHttpRequest(string requestText)
        {
            return string.Empty;
        }

        public void InitConfig()
        {
            this.HostConfig = HostConfig.GetHostConfig();
        }

        public Process GetWorkerProcessFromAppPool(string appPoolName)
        {
            lock (lockObj)
            {
                
                if (!appPool.ContainsKey(appPoolName))
                {
                    log.Info($"No worker process found for appPool {appPoolName}, starting new one");
                    var workProcessId = this.LaunchWorkPerrocess();
                    appPool.Add(appPoolName, workProcessId);
                    log.Info($"New worker process launched for appPool {appPoolName}");
                }
                Process workerProcess = null;
                try
                {
                    workerProcess = Process.GetProcessById(appPool[appPoolName]);
                }
                catch(Exception e)
                {
                    log.Error($"Failed getting worker process for apppool {appPoolName}");
                    log.Error(e);
                    log.Info($"Launching new worker process for apppool {appPoolName}");
                    var workProcessId = this.LaunchWorkPerrocess();
                    appPool[appPoolName] = workProcessId;
                    workerProcess = Process.GetProcessById(workProcessId);
                }
                return workerProcess;
            }
        }

        public void WorkerProcessRecyle(int processId)
        {
            var workerProcess = Process.GetProcessById(processId);
            if(workerProcess!=null)
            {
                workerProcess.Close();
            }
        }

        private bool WorkerProcessHealthCheck(int processId)
        {
            var workerProcess = Process.GetProcessById(processId);
            if (workerProcess != null)
            {
                return true;
            }
            return false;
        }

        private int LaunchWorkPerrocess()
        {
            var process = new Process();
            var startInfo = new ProcessStartInfo()
            {
                FileName = this.workerProcessPath,
            };
            process.StartInfo = startInfo;
            process.Start();
            return process.Id;
        }

        
    }
}
