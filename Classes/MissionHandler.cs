using GTANetworkServer;
using GTANetworkShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stuykserver.Classes
{
    public class MissionHandler : Script,IDisposable
    {
        public enum MissionType
        {
            PaperRoute
        }

        public enum PointType
        {
            DestroyImmediately,
            Capture,
            Destroy
        }

        //=============================
        // Mission Information
        //=============================
        Client missionOwner; // Mission Owner
        List<Client> missionAllies; // Current Assigned Players
        List<Client> missionOpposition; // Current Opposition
        NetHandle missionTarget; // Current Target
        List<NetHandle> missionTargets; // Other Targets
        ColShape missionColShape; // Current Target
        List<ColShape> missionColShapes; // Other Targets
        MissionType missionType; // The Mission Type

        public MissionHandler()
        {
            API.onEntityEnterColShape += API_onEntityEnterColShape;
        }

        private void API_onEntityEnterColShape(ColShape colshape, NetHandle entity)
        {
            if (API.getEntityType(entity) == EntityType.Player)
            {
                if (colshape.hasData("Owner"))
                {
                    Client player = API.getPlayerFromHandle(entity);
                    if (colshape.getData("Owner") == player)
                    {
                        switch ((MissionType)colshape.getData("PointType"))
                        {
                            case MissionType.PaperRoute:

                                return;
                        }
                    }
                }
            }
        }

        public void setupColShape(Vector3 position, float range, PointType type)
        {
            ColShape colshape = API.createCylinderColShape(position, range, 5f);
            colshape.setData("Owner", missionOwner);
            colshape.setData("PointType", type);
            // Setup Our First Collision Target
            if (missionColShapes.Count - 1 == -1)
            {
                missionColShape = colshape;
            }
            // Add it to our list.
            missionColShapes.Add(colshape);
        }

        public void Dispose()
        {
            // CODE CLEANUP
        }
    }
}
