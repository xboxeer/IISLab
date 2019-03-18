using Microsoft.Web.Administration;
using System;
using System.Diagnostics;
using System.Web;

namespace IISLab
{
    public class StatusModule : IHttpModule
    {
        TraceSource tsSource=new TraceSource("tsSource");
        /// <summary>
        /// You will need to configure this module in the Web.config file of your
        /// web and register it with IIS before being able to use it. For more information
        /// see the following link: https://go.microsoft.com/?linkid=8101007
        /// </summary>
        #region IHttpModule Members

        public void Dispose()
        {
            //clean-up code here.
        }

        public void Init(HttpApplication context)
        {
            // Below is an example of how you can handle LogRequest event and provide 
            // custom logging implementation for it
            context.EndRequest += Context_EndRequest;
        }

        private void Context_EndRequest(object sender, EventArgs e)
        {
            tsSource.TraceEvent(TraceEventType.Start, 0, "[STATUS MODULE] START EndRequest");
            var application = (HttpApplication)sender;
            var context = application.Context;
            context.Response.Write("<br><font size=2 color=red>StatusModule:monitoring data</font><br>");
            context.Response.Write("<table style=\"font-size:8pt;font-family:Consolas\">");
            context.Response.Write("<tr style=\"background-color: black\">");
            context.Response.Write("<td><b><font color=\"white\" style=\"background-color: black\">WP PID</font></b></td>");
            context.Response.Write("<td><b><font color=\"white\" style=\"background-color: black\">AppPool ID</font></b></td></tr>");
            ServerManager smLocalHost = new ServerManager();
            foreach(var wp in smLocalHost.WorkerProcesses)
            {
                context.Response.Write("<tr>");
                context.Response.Write("<td><font size=1>" + wp.ProcessId.ToString() + "</font></td>");
                context.Response.Write("<td><font size=1>" + wp.AppPoolName + "</font></td>");
                context.Response.Write("</tr>");
            }
            context.Response.Write("</table>");

            tsSource.TraceEvent(TraceEventType.Stop, 0, "[STATUS MODULE] END EndRequest");
        }

        #endregion


    }
}
