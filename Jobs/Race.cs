using GTANetworkServer;
using GTANetworkShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stuykserver.Jobs
{
    public class Race : Script, IDisposable
    {
        public Race() { }

        // Players in Race
        List<Client> players;
        // ColShapes
        ColShape startColShape;
        ColShape endColShape;
        // Positions
        Vector3 startPosition;
        Vector3 endPosition;
        // Wages / Information
        double raceWager;
        TextLabel startLabel;
        TextLabel wagerLabel;
        TextLabel endLabel;
        TextLabel countdownLabel;

        public Race(Vector3 startPos, Client player, double wager)
        {
            // Setup our instance to take on additional players.
            players = new List<Client>();
            players.Add(player);

            // Get the starting position.
            setStartPosition(startPos);
            
            // Setup the wager to join the race.
            setRaceWager(wager);

            // Setup the labels.
            setWagerLabel(wager);

            API.sendChatMessageToPlayer(players[0], "~y~Setup your end point with ~b~/finishposition");
        }

        // Race Functions
        public void setEndPosition(Vector3 position)
        {
            endPosition = position;
            endColShape = API.createCylinderColShape(position, 10f, 5f);
            setEndLabel();
            // Add Markers Locally
        }

        public void setStartPosition(Vector3 position)
        {
            startPosition = position;
            startColShape = API.createCylinderColShape(position, 10f, 5f);
            setStartLabel();
            setStartMarker();
            // Add Markers Locally
        }

        public void setStartMarker()
        {
            foreach (Client player in players)
            {
                API.triggerClientEvent(player, "pushMarker", 25, startPosition, new Vector3(10, 10, 10), 100, 255, 212, 0, player.rotation);
            }
        }

        public void setStartMarkerIndividual(Client player)
        {
            API.triggerClientEvent(player, "pushMarker", 25, startPosition, new Vector3(10, 10, 10), 100, 255, 212, 0, player.rotation);
        }

        public void setEndMarkerIndividual(Client player)
        {
            API.triggerClientEvent(player, "pushMarker", 4, endPosition, new Vector3(10, 10, 10), 100, 255, 212, 0, player.rotation);
        }

        public void setEndMarker()
        {
            foreach (Client player in players)
            {
                API.triggerClientEvent(player, "pushMarker", 4, endPosition, new Vector3(10, 10, 10), 100, 255, 212, 0, player.rotation);
            }
        }

        public void setRaceWager(double wager)
        {
            raceWager = wager;
        }

        public void setWagerLabel(double wager)
        {
            wagerLabel = API.createTextLabel(string.Format("~o~Entry Fee: ~g~${0}", wager), startPosition, 10f, 1f);
            // Add text labels locally.
        }

        public void setStartLabel()
        {
            startLabel = API.createTextLabel("~y~Race", new Vector3(startPosition.X, startPosition.Y, startPosition.Z + 1f), 10f, 1f);
            // Add text labels locally.
        }

        public void setEndLabel()
        {
            endLabel = API.createTextLabel("~b~Finish", new Vector3(endPosition.X, endPosition.Y, endPosition.Z + 1f), 10f, 1f);
            // Add text labels locally.
        }

        // Utility
        public bool isPlayerRacing(Client player)
        {
            if (players.Contains(player))
            {
                return true;
            }
            return false;
        }

        // Dispose Instance of the Race
        public void Dispose()
        {
            foreach (Client player in players)
            {
                API.triggerClientEvent(player, "removeMarkers");
                API.sendChatMessageToPlayer(player, "~r~The race was cancelled.");
            }

            // Delete Start
            if (startColShape != null)
            {
                API.deleteColShape(startColShape);
            }

            // Delete End
            if (endColShape != null)
            {
                API.deleteColShape(endColShape);
            }

            // Delete Text Labels
            if (startLabel != null)
            {
                API.deleteEntity(startLabel);
            }

            if (endLabel != null)
            {
                API.deleteEntity(startLabel);
            }

            if (wagerLabel != null)
            {
                API.deleteEntity(wagerLabel);
            }

            if (countdownLabel != null)
            {
                API.deleteEntity(startLabel);
            }
        }
    }
}
