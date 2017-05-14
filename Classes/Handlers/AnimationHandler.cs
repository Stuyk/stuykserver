using GTANetworkServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stuykserver.Classes
{
    public class AnimationHandler : Script
    {
        [Flags]
        public enum AnimationFlags
        {
            Loop = 1 << 0,
            StopOnLastFrame = 1 << 1,
            OnlyAnimateUpperBody = 1 << 4,
            AllowPlayerControl = 1 << 5,
            Cancellable = 1 << 7
        }

        public AnimationHandler()
        {
            API.onClientEventTrigger += API_onClientEventTrigger;
        }

        private void API_onClientEventTrigger(Client player, string eventName, params object[] arguments)
        {
            if (eventName == "playAnimation")
            {
                API.playPlayerAnimation(player, (int)(AnimationFlags.Loop), arguments[0].ToString(), arguments[1].ToString());
                return;
            }

            if (eventName == "stopAnimation")
            {
                for (int i = 0; i < 10; i++)
                {
                    API.stopPlayerAnimation(player);
                }
                return;
            }
        }
    }
}
