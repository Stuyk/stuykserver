using GTANetworkServer;
using GTANetworkShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stuykserver.Classes
{
    public class MissionHandler : Script
    {
        public MissionHandler()
        {
            API.onClientEventTrigger += API_onClientEventTrigger;
        }

        private void API_onClientEventTrigger(Client player, string eventName, params object[] arguments)
        {
            if (!player.hasData("Mission") && player.getData("Mission") != null)
            {
                return;
            }

            MissionClass mission = player.getData("Mission");

            switch (eventName)
            {
                case "updateMissionBar":
                    if (arguments.Length > 0)
                    {
                        mission.updateMissionBar(player, Convert.ToInt32(arguments[0]));
                    }
                    return;
                case "shiftMissionObjectives":
                    mission.shiftMissionObjectives(player, (Vector3)arguments[0]);
                    return;
            }
        }

        [Command("startDebugMission")]
        public void cmdStartDebugMission(Client player)
        {
            MissionClass mission = new MissionClass(player);
            mission.addObjective(new Vector3(-34.8390, -104.8744, 56.3878), MissionClass.PointType.Waypoint);
            mission.addObjective(new Vector3(-13.0416, -143.2909, 55.6454), MissionClass.PointType.DestroyVehicle);
            mission.setTargetVehicleType(VehicleHash.Buffalo);
            mission.addObjective(new Vector3(-41.5859, -98.7301, 57.3881), MissionClass.PointType.Hack);
            mission.addObjective(new Vector3(-29.2809, -93.2767, 56.2543), MissionClass.PointType.Capture);
            mission.addObjective(new Vector3(-27.0519, -80.7949, 56.2536), MissionClass.PointType.Waypoint);
            mission.addObjective(new Vector3(-27.1417, -77.5539, 56.8771), MissionClass.PointType.Destroy);
            
            mission.startMission();
        }

        [Command("addplayer")]
        public void cmdAddPlayer(Client player, string target)
        {
            Client tgt = API.getPlayerFromName(target);
            if (tgt != null)
            {
                MissionClass mission = player.getData("Mission");
                mission.addAlly(tgt);
            }
            
        }


    }
}
