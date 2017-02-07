using GTANetworkServer;
using GTANetworkShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace stuykserver.Util
{
    public class SpawnPoints : Script
    {
        public SpawnPoints()
        {
            API.onResourceStart += API_onResourceStart;
        }

        private void API_onResourceStart()
        {
            API.consoleOutput("Started: SpawnPoints");  // YOU BROKE THE DATA HANDLER FAGGOT
        }

        // Prebuilt Clothing Locations
        public List<Vector3> ClothingSpawnPoints = new List<Vector3>
        {
            new Vector3(-1484.783, -947.5916, 11), // Pier 1
            new Vector3(-1519.79, -917.5608, 10.16333) // Pier 2
        };

        // Prebuilt Spawn Locations
        public List<Vector3> ServerSpawnPoints = new List<Vector3>
        {
            new Vector3(-1537.53, -942.0224, 12)
        };

        // Prebuilt Fishing Locations
        public List<Vector3> FishingSpawnPoints = new List<Vector3>
        {
            new Vector3(-1749.341, -1138.289, 13.01846) // Pier
        };

        public List<Vector3> PizzaDeliveryPoints = new List<Vector3>
        {
            new Vector3(-1334.972, -1281.354, 4.835987)
        };

        public List<Vector3> PizzaDeliveryCarSpawns = new List<Vector3>
        {
            new Vector3(-1350.796, -1273.298, 4.83633)
        };
    }
}
