using GTANetworkServer;
using GTANetworkShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stuykserver.Classes
{
    public class MissionClass : Script,IDisposable
    {
        public enum PointType
        {
            Waypoint,
            Capture,
            Destroy,
            Hack,
            DestroyVehicle
        }
        // Variables
        List<Vector3> missionObjectives;
        List<PointType> missionPoints;
        Vector3 currentObjective;
        List<Client> allies;
        List<Client> enemies;
        Client missionOwner;
        int allyBar;
        int enemyBar;
        int allyPoints;
        int enemyPoints;
        NetHandle target;
        VehicleHash targetVehicleType;
        // Useless constructor.
        public MissionClass() { }
        // Main Mission Constructor, call this first.
        public MissionClass(Client player)
        {
            missionObjectives = new List<Vector3>();
            missionPoints = new List<PointType>();
            allies = new List<Client>();
            enemies = new List<Client>();
            allies.Add(player);
            missionOwner = player;
            player.setData("Mission", this); // Give the player a Mission Instance.
            allyPoints = 0;
            enemyPoints = 0;
        }
        // Sync Mission Objectives
        public void shiftMissionObjectives(Client winner, Vector3 currentObjectivePosition)
        {
            if (currentObjective != (Vector3)winner.getSyncedData("Mission_Position"))
            {
                return;
            }
            currentObjective = null;

            if (missionObjectives.Count > 0)
            {
                missionObjectives.Remove(missionObjectives.First());
                missionPoints.Remove(missionPoints.First());
            }

            if (allies.Contains(winner))
            {
                allyPoints += 1;
            } 

            if (enemies.Contains(winner))
            {
                enemyPoints += 1;
            }

            if (missionObjectives.Count == 0)
            {
                if (allyPoints > enemyPoints)
                {
                    foreach (Client player in allies)
                    {
                        API.triggerClientEvent(player, "missionWinScreen");
                    }

                    foreach (Client player in enemies)
                    {
                        API.triggerClientEvent(player, "missionLoseScreen");
                    }
                }

                if (allyPoints < enemyPoints)
                {
                    foreach (Client player in allies)
                    {
                        API.triggerClientEvent(player, "missionLoseScreen");
                    }

                    foreach (Client player in enemies)
                    {
                        API.triggerClientEvent(player, "missionWinScreen");
                    }
                }

                if (allyPoints == enemyPoints)
                {
                    foreach (Client player in allies)
                    {
                        API.triggerClientEvent(player, "missionTie");
                    }

                    foreach (Client player in enemies)
                    {
                        API.triggerClientEvent(player, "missionTie");
                    }
                }
                return;
            }

            currentObjective = missionObjectives[0];
            if (API.doesEntityExist(target) && API.getEntityType(target) == EntityType.Vehicle)
            {
                API.deleteEntity(target);
            }
            if (missionPoints[0] == PointType.DestroyVehicle)
            {
                setupTargetVehicle();
            }
            allyBar = 50; // Set to 50
            enemyBar = 50; // Set to 50

            foreach (Client player in allies)
            {
                API.setEntitySyncedData(player, "Mission_Position", missionObjectives[0]);
                API.setEntitySyncedData(player, "Mission_Task_Bar_Allies", allyBar);
                API.setEntitySyncedData(player, "Mission_Task_Bar_Enemies", enemyBar);
                API.setEntitySyncedData(player, "Mission_Type", missionPoints[0].ToString());
                API.setEntitySyncedData(player, "Mission_Points_Allies", allyPoints);
                API.setEntitySyncedData(player, "Mission_Points_Enemies", enemyPoints);
                API.setEntitySyncedData(player, "Mission_Target", target);
            }

            foreach (Client player in enemies)
            {
                API.setEntitySyncedData(player, "Mission_Position", missionObjectives[0]);
                API.setEntitySyncedData(player, "Mission_Task_Bar_Allies", allyBar);
                API.setEntitySyncedData(player, "Mission_Task_Bar_Enemies", enemyBar);
                API.setEntitySyncedData(player, "Mission_Type", missionPoints[0].ToString());
                API.setEntitySyncedData(player, "Mission_Points_Allies", allyPoints);
                API.setEntitySyncedData(player, "Mission_Points_Enemies", enemyPoints);
                API.setEntitySyncedData(player, "Mission_Target", target);
            }
        }
        // Sync Current Mission Bars
        public void syncMissionBars()
        {
            foreach (Client player in allies)
            {
                API.setEntitySyncedData(player, "Mission_Task_Bar_Allies", allyBar);
                API.setEntitySyncedData(player, "Mission_Task_Bar_Enemies", enemyBar);
            }

            foreach (Client player in enemies)
            {
                API.setEntitySyncedData(player, "Mission_Task_Bar_Allies", allyBar);
                API.setEntitySyncedData(player, "Mission_Task_Bar_Enemies", enemyBar);
            }
        }
        // Update Mission Bars
        public void updateMissionBar(Client player, int amount)
        {
            if (allies.Contains(player))
            {
                allyBar = amount;
            }

            if (enemies.Contains(player))
            {
                enemyBar = amount;
            }

            syncMissionBars();
        }
        // Push objective to this instance.
        public void addObjective(Vector3 position, PointType pointType)
        {
            missionObjectives.Add(position);
            missionPoints.Add(pointType);
            // Setup synced data for local use.
            if (currentObjective == null)
            {
                currentObjective = position;
                API.setEntitySyncedData(missionOwner, "Mission_Target", target);
                API.setEntitySyncedData(missionOwner, "Mission_Opposition", "Ally");
                API.setEntitySyncedData(missionOwner, "Mission_Position", position);
                API.setEntitySyncedData(missionOwner, "Mission_Type", missionPoints[0].ToString());
                API.setEntitySyncedData(missionOwner, "Mission_Points_Allies", allyPoints);
                API.setEntitySyncedData(missionOwner, "Mission_Points_Enemies", enemyPoints);
                API.setEntitySyncedData(missionOwner, "Mission_Task_Bar_Allies", allyBar);
                API.setEntitySyncedData(missionOwner, "Mission_Task_Bar_Enemies", enemyBar);
            }
        }
        // Push objectives to joiner, if ally.
        public void addAlly(Client player)
        {
            if (!allies.Contains(player))
            {
                allies.Add(player);
                player.setData("Mission", this);
                API.setEntitySyncedData(player, "Mission_Target", target);
                API.setEntitySyncedData(player, "Mission_Opposition", "Ally");
                API.setEntitySyncedData(player, "Mission_Position", missionObjectives[0]);
                API.setEntitySyncedData(player, "Mission_Type", missionPoints[0].ToString());
                API.setEntitySyncedData(player, "Mission_Points_Allies", allyPoints);
                API.setEntitySyncedData(player, "Mission_Points_Enemies", enemyPoints);
                API.setEntitySyncedData(player, "Mission_Task_Bar_Allies", allyBar);
                API.setEntitySyncedData(player, "Mission_Task_Bar_Enemies", enemyBar);
                API.triggerClientEvent(player, "startMission");
            }
        }
        // Push objectives to joiner, if enemy.
        public void addEnemy(Client player)
        {
            if (!enemies.Contains(player))
            {
                enemies.Add(player);
                player.setData("Mission", this);
                API.setEntitySyncedData(player, "Mission_Target", target);
                API.setEntitySyncedData(player, "Mission_Opposition", "Enemy");
                API.setEntitySyncedData(player, "Mission_Position", missionObjectives[0]);
                API.setEntitySyncedData(player, "Mission_Type", missionPoints[0].ToString());
                API.setEntitySyncedData(player, "Mission_Points_Allies", allyPoints);
                API.setEntitySyncedData(player, "Mission_Points_Enemies", enemyPoints);
                API.setEntitySyncedData(player, "Mission_Task_Bar_Allies", allyBar);
                API.setEntitySyncedData(player, "Mission_Task_Bar_Enemies", enemyBar);
                API.triggerClientEvent(player, "startMission");
            }
        }
        // Start Mission
        public void startMission()
        {
            foreach (Client player in allies)
            {
                API.setEntitySyncedData(player, "Mission_Started", true);
            }

            foreach (Client player in enemies)
            {
                API.setEntitySyncedData(player, "Mission_Started", true);
            }
        }
        // Setup Target Vehicle
        public void setupTargetVehicle()
        {
            allyBar = 100;
            enemyBar = 100;
            target = API.createVehicle(targetVehicleType, missionObjectives[0].Add(new Vector3(0, 0, 1)), new Vector3(), 111, 111);
            API.setVehicleLocked(target, true);
            syncTarget();
            syncMissionBars();
        }
        // Sync the Target for all players.
        public void syncTarget()
        {
            foreach (Client player in allies)
            {
                API.setEntitySyncedData(player, "Mission_Target", target);
            }

            foreach (Client player in enemies)
            {
                API.setEntitySyncedData(player, "Mission_Target", target);
            }
        }
        // Set Target Vehicle Type
        public void setTargetVehicleType(VehicleHash type)
        {
            targetVehicleType = type;
        }
        public void Dispose()
        {
            // CODE CLEANUP
        }
    }
}
