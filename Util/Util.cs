using GTANetworkServer;
using GTANetworkShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stuykserver.Util
{
    public class Util : Script
    {
        public Vector3 convertToVector3(object x, object y, object z)
        {
            return new Vector3(Convert.ToInt32(x), Convert.ToInt32(y), Convert.ToInt32(z));
        }
    }
}
