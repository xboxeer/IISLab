using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using log4net;
using IISLab.Web;
using IISLab.ServerAdmin;
using System.IO;
using System.Reflection;

namespace WorkerProcess
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = HostConfig.GetHostConfig();
            var nativeModules = GetNativeModules(config);
            var managedModules = GetManagedModule(config);
            HttpNativeRuntime runtime = null;
            
            try
            {
                int workerProcessId = Process.GetCurrentProcess().Id;
                ILog log = LogManager.GetLogger("WorkerProcess");
                using (NamedPipeClientStream pipeStream = new NamedPipeClientStream($"{workerProcessId}.Request"))
                {
                    Byte[] bytes = new Byte[10];
                    Char[] chars = new Char[10];
                    
                    using (NamedPipeServerStream responsePipeStream = new NamedPipeServerStream($"{workerProcessId}.Response",
                    PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Message, PipeOptions.None))
                    {
                        
                        while (true)
                        {
                            string message = null;
                            if (!pipeStream.IsConnected)
                            {
                                pipeStream.Connect();
                                pipeStream.ReadMode = PipeTransmissionMode.Message;
                            }
                            do
                            {
                                int numBytes = pipeStream.Read(bytes, 0, bytes.Length);
                                int numChars = Encoding.UTF8.GetChars(bytes, 0, numBytes, chars, 0);
                                if (numBytes > 0)
                                {
                                    message += new String(chars, 0, numChars);
                                }
                            } while (!pipeStream.IsMessageComplete);

                            if (!string.IsNullOrEmpty(message))
                            {
                                if (runtime == null)
                                {
                                    runtime = new HttpNativeRuntime(nativeModules, managedModules);
                                    var site = config.GetRequestingSite(message);
                                    runtime.PhysicalPath = site.PhysicalPath;
                                }
                                responsePipeStream.WaitForConnection();
                                var data=runtime.ProcessRequest(message);
                                responsePipeStream.Write(data, 0, data.Length);
                                responsePipeStream.Disconnect();
                            }
                            
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Console.Write(e);
                Console.Read();
            }
        }

        public static List<INativeHttpModuleSimulator> GetNativeModules(HostConfig config)
        {
            List<INativeHttpModuleSimulator> nativeModule = new List<INativeHttpModuleSimulator>();
            foreach (var moduleConfig in config.GlobalModules)
            {
                var module = Activator.CreateInstance("IISLab.Web", moduleConfig.Image).Unwrap() as INativeHttpModuleSimulator;
                nativeModule.Add(module);
            }
            return nativeModule;
        }

        public static List<IManagedHttpModule> GetManagedModule(HostConfig config)
        {
            return new List<IManagedHttpModule>();
        }

        
    }
}
