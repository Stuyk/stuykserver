using GTANetworkServer;
using GTANetworkShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

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
            DestroyVehicle,
            Investigate,
            DisableBomb,
            TakeVehicle,
            DeliverVehicle
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
        int timer;
        Timer timerInstance;
        NetHandle target;
        NetHandle targetDelivery;
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

            foreach (Client player in allies)
            {
                API.setEntitySyncedData(player, "Mission_Delay", true);
            }

            foreach (Client player in enemies)
            {
                API.setEntitySyncedData(player, "Mission_Delay", true);
            }

            PointType lastType = missionPoints[0];

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

            if (API.doesEntityExist(target))
            {
                API.setEntityPositionFrozen(target, false);

                if (lastType == PointType.DestroyVehicle)
                {
                    API.setVehicleHealth(target, -900);
                    Vector3 vehiclePos = API.getEntityPosition(target);
                    API.createExplosion(ExplosionType.Flame, new Vector3(vehiclePos.X, vehiclePos.Y, vehiclePos.Z - 2));
                }

                if (lastType == PointType.DisableBomb)
                {
                    API.deleteEntity(target);
                }

                if (lastType == PointType.DeliverVehicle)
                {
                    API.deleteEntity(target);
                }
            }

            if (missionObjectives.Count == 0)
            {
                if (allyPoints > enemyPoints)
                {
                    foreach (Client player in allies)
                    {
                        API.triggerClientEvent(player, "missionWinScreen");
                        API.setEntitySyncedData(player, "Mission_Delay", false);
                    }

                    foreach (Client player in enemies)
                    {
                        API.triggerClientEvent(player, "missionLoseScreen");
                        API.setEntitySyncedData(player, "Mission_Delay", false);
                    }
                }

                if (allyPoints < enemyPoints)
                {
                    foreach (Client player in allies)
                    {
                        API.triggerClientEvent(player, "missionLoseScreen");
                        API.setEntitySyncedData(player, "Mission_Delay", false);
                    }

                    foreach (Client player in enemies)
                    {
                        API.triggerClientEvent(player, "missionWinScreen");
                        API.setEntitySyncedData(player, "Mission_Delay", false);
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

            switch (missionPoints[0])
            {
                case PointType.DestroyVehicle:
                    setupTargetVehicle();
                    break;
                case PointType.DisableBomb:
                    setupBombDevice();
                    break;
                case PointType.TakeVehicle:
                    setupTargetVehicle();
                    break;
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
                API.setEntitySyncedData(player, "Mission_Timer", timer);
                API.setEntitySyncedData(player, "Mission_Delay", false);
            }

            foreach (Client player in enemies)
            {
                API.setEntitySyncedData(player, "Mission_Position", missionObjectives[0]);
                API.setEntitySyncedData(player, "Mission_Task_Bar_Allies", allyBar);
                API.setEntitySyncedData(player, "Mission_Task_Bar_Enemies", enemyBar);
                API.setEntitySyncedData(player, "Mission_Type", missionPoints[0].ToString());
                API.setEntitySyncedData(player, "Mission_Points_Allies", allyPoints);
                API.setEntitySyncedData(player, "Mission_Points_Enemies", enemyPoints);
                API.setEntitySyncedData(player, "Mission_Timer", timer);
                API.setEntitySyncedData(player, "Mission_Delay", false);
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
        // Return Mission Owner
        public Client returnMissionOwner()
        {
            return missionOwner;
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
                API.setEntitySyncedData(missionOwner, "Mission_Timer", timer);
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
                API.setEntitySyncedData(player, "Mission_Timer", timer);
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
                API.setEntitySyncedData(player, "Mission_Timer", timer);
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
            target = API.createVehicle(targetVehicleType, missionObjectives[0].Add(new Vector3(0, 0, 1)), new Vector3(), 111, 111);
            syncTarget();

            if (missionPoints[0] == PointType.DestroyVehicle)
            {
                API.setEntityPositionFrozen(target, true);
                API.setVehicleLocked(target, true);
                allyBar = 100;
                enemyBar = 100;
                syncMissionBars();
                return;
            }

            if (missionPoints[0] == PointType.TakeVehicle)
            {
                API.setVehicleLocked(target, false);
                timer = 180000;
                timerInstance = new Timer();
                timerInstance.Interval = 1000;
                timerInstance.Elapsed += TimerInstance_Elapsed;
                timerInstance.Start();
                targetDelivery = target;
                return;
            }
        }
        // Setup Bomb Device
        public void setupBombDevice()
        {
            allyBar = 0;
            enemyBar = 0;
            GTANetworkServer.Object device = API.createObject(1764669601, missionObjectives[0].Add(new Vector3(0, 0, 0.04)), new Vector3(-90, -90, 0), 0);
            target = device;
            syncTarget();
            syncMissionBars();
            timer = 180000;
            timerInstance = new Timer();
            timerInstance.Interval = 1000;
            timerInstance.Elapsed += TimerInstance_Elapsed;
            timerInstance.Start();
        }

        private void TimerInstance_Elapsed(object sender, ElapsedEventArgs e)
        {
            timer = timer - 1000;

            switch(missionPoints[0])
            {
                case PointType.DisableBomb:
                    break;
                case PointType.TakeVehicle:
                    break;
                default:
                    timerInstance.Dispose();
                    return;
            }

            if (timer <= 0)
            {
                if (missionPoints[0] == PointType.DisableBomb)
                {
                    API.createExplosion(ExplosionType.Rocket, missionObjectives[0], 10f, 0);
                    API.deleteEntity(target);
                }

                if (missionPoints[0] == PointType.TakeVehicle)
                {
                    API.deleteEntity(target);
                }

                foreach (Client player in allies)
                {
                    API.triggerClientEvent(player, "missionLoseScreen");

                }

                foreach (Client player in enemies)
                {
                    API.triggerClientEvent(player, "missionLoseScreen");
                }
                timerInstance.Stop();
                timerInstance.Enabled = false;
                return;
            }

            foreach (Client player in allies)
            {
                API.setEntitySyncedData(player, "Mission_Timer", timer);
            }

            foreach (Client player in enemies)
            {
                API.setEntitySyncedData(player, "Mission_Timer", timer);
            }
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
        // Remove Ally
        public void removeAlly(Client player)
        {
            if (allies.Count > 1)
            {
                if (allies.Contains(player))
                {
                    allies.Remove(player);
                    API.setEntitySyncedData(player, "Mission_Started", false);
                    API.resetEntitySyncedData(player, "Mission_Target");
                    API.resetEntitySyncedData(player, "Mission_Opposition");
                    API.resetEntitySyncedData(player, "Mission_Position");
                    API.resetEntitySyncedData(player, "Mission_Type");
                    API.resetEntitySyncedData(player, "Mission_Points_Allies");
                    API.resetEntitySyncedData(player, "Mission_Points_Enemies");
                    API.resetEntitySyncedData(player, "Mission_Task_Bar_Allies");
                    API.resetEntitySyncedData(player, "Mission_Task_Bar_Enemies");
                    API.resetEntitySyncedData(player, "Mission_Timer");
                    API.sendChatMessageToPlayer(player, "~r~You have left the mission.");
                    return;
                }
                else
                {
                    return;
                }
            }
            else
            {
                Dispose();
            }
        }
        // Remove Enemy
        public void removeEnemy(Client player)
        {
            if (enemies.Count > 1)
            {
                if (enemies.Contains(player))
                {
                    enemies.Remove(player);
                    API.setEntitySyncedData(player, "Mission_Started", false);
                    API.resetEntitySyncedData(player, "Mission_Target");
                    API.resetEntitySyncedData(player, "Mission_Opposition");
                    API.resetEntitySyncedData(player, "Mission_Position");
                    API.resetEntitySyncedData(player, "Mission_Type");
                    API.resetEntitySyncedData(player, "Mission_Points_Allies");
                    API.resetEntitySyncedData(player, "Mission_Points_Enemies");
                    API.resetEntitySyncedData(player, "Mission_Task_Bar_Allies");
                    API.resetEntitySyncedData(player, "Mission_Task_Bar_Enemies");
                    API.resetEntitySyncedData(player, "Mission_Timer");
                    API.sendChatMessageToPlayer(player, "~r~You have left the mission.");
                    return;
                }
                else
                {
                    return;
                }
            }
            else
            {
                Dispose();
            }
        }
        public void Dispose()
        {
            if (allies.Count > 1)
            {
                if (allies.Contains(missionOwner))
                {
                    allies.Remove(missionOwner);
                    Client player = missionOwner;
                    API.setEntitySyncedData(player, "Mission_Started", false);
                    API.resetEntitySyncedData(player, "Mission_Target");
                    API.resetEntitySyncedData(player, "Mission_Opposition");
                    API.resetEntitySyncedData(player, "Mission_Position");
                    API.resetEntitySyncedData(player, "Mission_Type");
                    API.resetEntitySyncedData(player, "Mission_Points_Allies");
                    API.resetEntitySyncedData(player, "Mission_Points_Enemies");
                    API.resetEntitySyncedData(player, "Mission_Task_Bar_Allies");
                    API.resetEntitySyncedData(player, "Mission_Task_Bar_Enemies");
                    API.resetEntitySyncedData(player, "Mission_Timer");
                    API.sendChatMessageToPlayer(player, "~r~You have left the mission.");
                    return;
                }
            }

            if (enemies.Count > 1)
            {
                if (enemies.Contains(missionOwner))
                {
                    allies.Remove(missionOwner);
                    Client player = missionOwner;
                    API.setEntitySyncedData(player, "Mission_Started", false);
                    API.resetEntitySyncedData(player, "Mission_Target");
                    API.resetEntitySyncedData(player, "Mission_Opposition");
                    API.resetEntitySyncedData(player, "Mission_Position");
                    API.resetEntitySyncedData(player, "Mission_Type");
                    API.resetEntitySyncedData(player, "Mission_Points_Allies");
                    API.resetEntitySyncedData(player, "Mission_Points_Enemies");
                    API.resetEntitySyncedData(player, "Mission_Task_Bar_Allies");
                    API.resetEntitySyncedData(player, "Mission_Task_Bar_Enemies");
                    API.resetEntitySyncedData(player, "Mission_Timer");
                    API.sendChatMessageToPlayer(player, "~r~You have left the mission.");
                    return;
                }
            }

            foreach (Client player in allies)
            {
                player.resetData("Mission");
                API.setEntitySyncedData(player, "Mission_Started", false);
                API.resetEntitySyncedData(player, "Mission_Target");
                API.resetEntitySyncedData(player, "Mission_Opposition");
                API.resetEntitySyncedData(player, "Mission_Position");
                API.resetEntitySyncedData(player, "Mission_Type");
                API.resetEntitySyncedData(player, "Mission_Points_Allies");
                API.resetEntitySyncedData(player, "Mission_Points_Enemies");
                API.resetEntitySyncedData(player, "Mission_Task_Bar_Allies");
                API.resetEntitySyncedData(player, "Mission_Task_Bar_Enemies");
                API.resetEntitySyncedData(player, "Mission_Timer");
                if (timerInstance != null)
                {
                    timerInstance.Dispose();
                }
            }

            foreach (Client player in enemies)
            {
                player.resetData("Mission");
                API.setEntitySyncedData(player, "Mission_Started", false);
                API.resetEntitySyncedData(player, "Mission_Target");
                API.resetEntitySyncedData(player, "Mission_Opposition");
                API.resetEntitySyncedData(player, "Mission_Position");
                API.resetEntitySyncedData(player, "Mission_Type");
                API.resetEntitySyncedData(player, "Mission_Points_Allies");
                API.resetEntitySyncedData(player, "Mission_Points_Enemies");
                API.resetEntitySyncedData(player, "Mission_Task_Bar_Allies");
                API.resetEntitySyncedData(player, "Mission_Task_Bar_Enemies");
                API.resetEntitySyncedData(player, "Mission_Timer");
                if (timerInstance != null)
                {
                    timerInstance.Dispose();
                }
            }

            if (API.doesEntityExist(target))
            {
                API.deleteEntity(target);
            }
            GC.SuppressFinalize(this);
        }
    }
}
