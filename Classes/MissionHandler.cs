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

        private void checkIfInMission(Client player)
        {
            if (!player.hasData("Mission"))
            {
                return;
            }

            MissionClass mission = (MissionClass)player.getData("Mission");
            mission.Dispose();
        }

        [Command("startDebugMission")]
        public void cmdStartDebugMission(Client player)
        {
            checkIfInMission(player);
            MissionClass mission = new MissionClass(player);
            mission.addObjective(new Vector3(-34.8390, -104.8744, 56.3878), MissionClass.PointType.Waypoint);
            mission.addObjective(new Vector3(-34.8390, -104.8744, 56.3878), MissionClass.PointType.DisableBomb);
            mission.addObjective(new Vector3(-13.0416, -143.2909, 55.6454), MissionClass.PointType.Investigate);
            mission.addObjective(new Vector3(-34.8390, -104.8744, 56.3878), MissionClass.PointType.Waypoint);
            mission.addObjective(new Vector3(-13.0416, -143.2909, 55.6454), MissionClass.PointType.DestroyVehicle);
            mission.setTargetVehicleType(VehicleHash.Buffalo);
            mission.addObjective(new Vector3(-41.5859, -98.7301, 57.3881), MissionClass.PointType.Hack);
            mission.addObjective(new Vector3(-29.2809, -93.2767, 56.2543), MissionClass.PointType.Capture);
            mission.addObjective(new Vector3(-27.0519, -80.7949, 56.2536), MissionClass.PointType.Waypoint);
            mission.addObjective(new Vector3(-27.1417, -77.5539, 56.8771), MissionClass.PointType.Destroy);
            mission.startMission();
        }

        [Command("startdebugmission2")]
        public void cmdstartdebugmission2(Client player)
        {
            checkIfInMission(player);
            MissionClass mission = new MissionClass(player);
            mission.addObjective(new Vector3(-56.7974, -66.2038, 58.2891), MissionClass.PointType.Investigate);
            mission.addObjective(new Vector3(-81.4844, -49.0375, 62), MissionClass.PointType.DestroyVehicle);
            mission.setTargetVehicleType(VehicleHash.BfInjection);
            mission.addObjective(new Vector3(-44.2143, -72.4286, 61.0867), MissionClass.PointType.Waypoint);
            mission.addObjective(new Vector3(-46.3358, -93.9127, 62.0233), MissionClass.PointType.Waypoint);
            mission.addObjective(new Vector3(-42.1593, -93.1439, 62.3606), MissionClass.PointType.Hack);
            mission.addObjective(new Vector3(-40.0173, -87.2542, 62.3501), MissionClass.PointType.Hack);
            mission.addObjective(new Vector3(-44.2143, -72.4286, 61.0867), MissionClass.PointType.Waypoint);
            mission.addObjective(new Vector3(-27.2388, -65.4641, 62.5742), MissionClass.PointType.Waypoint);
            mission.addObjective(new Vector3(-16.9001, -48.0706, 64.1972), MissionClass.PointType.Waypoint);
            mission.addObjective(new Vector3(-8.8815, -26.4883, 67.9976), MissionClass.PointType.Investigate);
            mission.addObjective(new Vector3(-19.9214, -22.1637, 67.9974), MissionClass.PointType.DisableBomb);
            mission.addObjective(new Vector3(-48.6407, -109.3436, 43.1382), MissionClass.PointType.Destroy);
            mission.addObjective(new Vector3(-44.1565, -15.0770, 68.5187), MissionClass.PointType.Waypoint);
        }

        [Command("gta")]
        public void cmdGrandTheftAuto(Client player)
        {
            checkIfInMission(player);
            MissionClass mission = new MissionClass(player);
            mission.addObjective(new Vector3(9.5198, -214.4477, 51.6366), MissionClass.PointType.Waypoint);
            mission.addObjective(new Vector3(2.3846, -203.8672, 51.7419), MissionClass.PointType.TakeVehicle);
            mission.setTargetVehicleType(VehicleHash.Washington);
            mission.addObjective(new Vector3(-734.9340, -361.9490, 34.0274), MissionClass.PointType.Waypoint);
            mission.addObjective(new Vector3(-758.4518, -420.8088, 34.6613), MissionClass.PointType.DeliverVehicle);
        }

        [Command("addplayer")]
        public void cmdAddPlayer(Client player, string target)
        {
            Client tgt = API.getPlayerFromName(target);
            if (tgt != null)
            {
                if (Convert.ToBoolean(tgt.getSyncedData("Mission_Started")))
                {
                    API.sendChatMessageToPlayer(player, "~r~Player is already in a mission.");
                    return;
                }
                tgt.setSyncedData("Mission_Invite", player);
                API.sendChatMessageToPlayer(tgt, string.Format("~g~You were invited to a mission by {0}, type ~y~/maccept ~g~to join or ~r~/mreject", API.getPlayerName(player)));
            }
        }

        [Command("maccept")]
        public void cmdAcceptMissionInvite(Client player)
        {
            if (!player.hasSyncedData("Mission_Invite"))
            {
                return;
            }

            Client inviter = API.getPlayerFromHandle(player.getSyncedData("Mission_Invite"));
            MissionClass mission = (MissionClass)inviter.getData("Mission");
            mission.addAlly(player);

            player.resetSyncedData("Mission_Invite");
        }

        [Command("mreject")]
        public void cmdMissionRejectInvite(Client player)
        {
            if (!player.hasSyncedData("Mission_Invite"))
            {
                return;
            }

            player.resetSyncedData("Mission_Invite");
        }

        [Command("mcancel")]
        public void cmdMissionCancel(Client player)
        {
            if (!player.hasData("Mission"))
            {
                return;
            }

            MissionClass mission = (MissionClass)player.getData("Mission");
            if (player == mission.returnMissionOwner())
            {
                mission.Dispose();
            }
            else
            {
                string opposition = Convert.ToString(player.getSyncedData("Mission_Opposition"));
                switch (opposition)
                {
                    case "Ally":
                        mission.removeAlly(player);
                        return;
                    case "Enemy":
                        mission.removeEnemy(player);
                        return;
                }
            }
        }
    }
}
