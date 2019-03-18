using IISLab.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IISLab.Modules.Managed
{
    public class ManagedModule : IManagedHttpModule
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Init(HttpApplication context)
        {
            throw new NotImplementedException();
        }
    }
}
