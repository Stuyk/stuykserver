using GTANetworkServer;
using GTANetworkShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace stuykserver.Util
{
    public class Util : Script
    {

        // Used to convert database x,y,z to Vector3.
        public Vector3 convertToVector3(object x, object y, object z)
        {
            return new Vector3(Convert.ToInt32(x), Convert.ToInt32(y), Convert.ToInt32(z));
        }

        // Used to check if the string is a valid username.
        public bool isValidUsername(string input)
        {
            string pattern = "^(([A-Z][a-z]+)(([ _][A-Z][a-z]+)|([ _][A-z]+[ _][A-Z][a-z]+)))$";
            bool returnBool = Regex.IsMatch(input, pattern);
            return returnBool;
        }

        // Check if Admin.
        public bool isAdmin(Client player)
        {
            return Convert.ToBoolean(API.getEntitySyncedData(player, "Admin"));
        }
    }
}
