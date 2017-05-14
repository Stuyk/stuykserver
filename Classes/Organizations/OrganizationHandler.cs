using GTANetworkServer;
using GTANetworkShared;
using stuykserver.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stuykserver.Classes
{
    public class OrganizationHandler : Script
    {
        DatabaseHandler db = new DatabaseHandler();
        Dictionary<int, Organization> organizationList = new Dictionary<int, Organization>();

        public OrganizationHandler()
        {
            API.onResourceStart += API_onResourceStart;
        }

        private void API_onResourceStart()
        {
            initializeOrganizations();
        }

        private void initializeOrganizations()
        {
            organizationList.Clear();

            string query = "SELECT * FROM Organization";
            DataTable result = API.exported.database.executeQueryWithResult(query);

            foreach (DataRow row in result.Rows)
            {
                Organization org = new Organization(row);
                int id = Convert.ToInt32(row["ID"]);
                organizationList.Add(id, org);
            }
        }

        public Organization returnOrganization(int id)
        {
            if (organizationList.ContainsKey(id))
            {
                return organizationList[id];
            }
            return null;
        }

        public string fetchOrgMessage(int id)
        {
            if (organizationList.ContainsKey(id))
            {
                return organizationList[id].returnOrganizationMessage();
            }

            return "INVALID ID";
        }
    }
}
