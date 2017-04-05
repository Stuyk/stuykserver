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
    public class Time : Script
    {
        // How much time do we want to add every time the timer does a heartbeat. HH / MM / SS
        static TimeSpan addOnTime = new TimeSpan(0, 5, 0);
        // At what time of day do we want our server to start at when we launch the script. EX: 12:00:00
        static TimeSpan serverStartTime = new TimeSpan(12, 0, 0);
        // Create a Timer so we can call it for other functions.
        Timer serverTimer;

        public Time()
        {
            API.onResourceStart += API_onResourceStart;
        }

        private void API_onResourceStart()
        {
            // Set the time to our start time.
            API.setTime(serverStartTime.Hours, serverStartTime.Minutes);

            // Create a timer that will run every minute.
            serverTimer = new Timer();
            serverTimer.Interval = 60000;

            // This is what function will start everytime the timer ticks to the interval time.
            serverTimer.Elapsed += Elapsed_Daylight_Timer;

            // Enable it right when the resource starts.
            serverTimer.Enabled = true;

            // Initially start the weather off as Sunny.
            API.setWeather(0);
        }

        private void Elapsed_Daylight_Timer(object sender, ElapsedEventArgs e)
        {
            // Pull the current server time.
            TimeSpan serverTime = API.getTime();
            // Add on our Timespan static.
            serverTime += addOnTime;
            // Set the server time to our new time.
            API.setTime(serverTime.Hours, serverTime.Minutes);

            // Current Time
            API.consoleOutput("In-Game Time is now: {0}", serverTime);

            // If the minutes are at exactly. :00 Let's toss in some weather.
            if (serverTime.Minutes == 0)
            {
                // Create a random that selects a weather ID from 0 to 8.
                Random random = new Random();
                int weatherID = random.Next(0, 8);
                API.setWeather(weatherID);

                // Let the console know what weather is going on.
                switch (weatherID)
                {
                    case 0:
                        API.consoleOutput("Weather: Extra Sunny");
                        break;
                    case 1:
                        API.consoleOutput("Weather: Clear");
                        break;
                    case 2:
                        API.consoleOutput("Weather: Clouds");
                        break;
                    case 3:
                        API.consoleOutput("Weather: Smog");
                        break;
                    case 4:
                        API.consoleOutput("Weather: Foggy");
                        break;
                    case 5:
                        API.consoleOutput("Weather: Overcast");
                        break;
                    case 6:
                        API.consoleOutput("Weather: Rain");
                        break;
                    case 7:
                        API.consoleOutput("Weather: Thunder");
                        break;
                    case 8:
                        API.consoleOutput("Weather: Light Rain");
                        break;
                }
            }
        }

        [Command("stoptime")]
        public void cmdStopTime(Client player)
        {
            serverTimer.Stop();
            API.sendChatMessageToPlayer(player, "The server time has been stopped.");
        }

        [Command("resumetime")]
        public void cmdStartTime(Client player)
        {
            serverTimer.Start();
            API.sendChatMessageToPlayer(player, "The server time has been resumed.");
        }
    }
}
