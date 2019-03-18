using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IISLab.WAS
{
    public class ApplicationPool
    {
        private Dictionary<string, int> appPool = new Dictionary<string, int>();

        public Dictionary<string,int> AppPool
        {
            get
            {
                return this.appPool;
            }
        }
    }
}
