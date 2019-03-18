using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IISLab.Web
{
    public interface IManagedHttpModule
    {
        void Dispose();
        void Init(HttpApplication context);
    }
}
