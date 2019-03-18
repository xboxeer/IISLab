using IISLab.ServerAdmin;
using IISLab.WAS;
using log4net;
using System;
using System.Diagnostics;
using System.IO.Pipes;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IISLab.W3Svc
{
    ///In real IIS scenario, w3svc component actually calls HTTP.sys API to retrive HTTP Request
    ///Also http.sys handles  sending the response back
    ///in our case we directly use Scoket to simulate HTTP.sys
    public static class HTTPListener
    {
        public static HostConfig HostConfig { get; set; }

        private static ILog log = LogManager.GetLogger(typeof(HTTPListener));

        public static void StartListen()
        {
            foreach(var site in HostConfig.Sites)
            {
                log.Info($"Start listening site {site.Name} on Host {site.Host}, IP {site.IP}, Port {site.Port}");
                Task.Run(() => ListenOnAddress(site.IP, Convert.ToInt32(site.Port),site.AppPool));
            }
        }

        private static void ListenOnAddress(string ip,int port,string appPoolName)
        {
            var skt = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            skt.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
            skt.Listen(10);
            var appPoolService = new ApplicationPoolService();
            while (true)
            {
                var proxSkt = skt.Accept();
                var buffer = new byte[1024 * 1024 * 2];//In Real IIS, this should be configable
                var length = proxSkt.Receive(buffer, 0, buffer.Length, SocketFlags.None);
                var requestText = Encoding.UTF8.GetString(buffer, 0, length);
                log.Info($"W3SVC Received Http Request on IP {ip}, Port {port}");
                Console.WriteLine(requestText);
                //In Real IIS, HTTP.sys would check if the request has cache, if yes, send back cached response directly

                log.Info($"Get worker process for apppool {appPoolName}");
                var workerProcess = appPoolService.GetWorkerProcessFromAppPool(appPoolName);
                //A key point that IIS document does not specify is that how W3Svc will handle that sending message to worker process
                //The reason why it is w3svc sending request to worker process instead of WAS is that WAS can hold other applcation like WCF
                //So WAS should not coupled with a specific request format(such as HTTP)
                //That's why w3svc send the request to worker process
                //May be in other scenario, such as WCF, a WCF message receiver would use WAS to manage its WCF worker process, and WCF message receiver send message to WCF Worker process just like w3svc handles http request

                SendRequestToWorkerProcess(requestText, workerProcess, response => {
                    proxSkt.Send(Encoding.UTF8.GetBytes(response));
                    proxSkt.Shutdown(SocketShutdown.Both);
                    proxSkt.Close();
                });
            }
        }
        /// <summary>
        /// IIS document does not specify how w3svc send request to worker process, so here we use named pipeline to send message
        /// In real iis I assume they must be using windows api to do this
        /// </summary>
        /// <param name="request"></param>
        /// <param name="workerProcess"></param>
        /// <param name="appPoolName"></param>
        /// <param name="responseHandler"></param>
        private static void SendRequestToWorkerProcess(string request,Process workerProcess,Action<string> responseHandler)
        {
            
            using (NamedPipeServerStream pipeStream = new NamedPipeServerStream($"{workerProcess.Id.ToString()}.Request", 
                PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Message, PipeOptions.None))
            {
                if (!pipeStream.IsConnected)
                {
                    pipeStream.WaitForConnection();
                }
                var data = Encoding.UTF8.GetBytes(request);
                pipeStream.Write(data, 0, data.Length);
                
            }

            Byte[] bytes = new Byte[10];
            Char[] chars = new Char[10];
            using (NamedPipeClientStream pipeStream = new NamedPipeClientStream($"{workerProcess.Id.ToString()}.Response"))
            {
                string message = string.Empty;
                pipeStream.Connect();
                pipeStream.ReadMode = PipeTransmissionMode.Message;
                do
                {
                    int numBytes = pipeStream.Read(bytes, 0, bytes.Length);
                    int numChars = Encoding.UTF8.GetChars(bytes, 0, numBytes, chars, 0);
                    message += new String(chars, 0, numChars);
                } while (!pipeStream.IsMessageComplete);
                responseHandler(message);
            }
        }

        
    }
}
