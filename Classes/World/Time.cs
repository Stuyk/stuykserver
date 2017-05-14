using GTANetworkServer;
using System;

namespace stuykserver.Classes
{
    public class Time : Script
    {
        static TimeSpan addOnTime = new TimeSpan(0, 2, 0);
        private TimeSpan serverTime = new TimeSpan(11, 55, 0);
        private int tick;

        public Time()
        {
            API.onResourceStart += API_onResourceStart;
            API.onUpdate += API_onUpdate;
        }

        private void API_onUpdate()
        {
            if (tick > 7200)
            { // 120 Tickrate
                updateServerTime();
                updateServerWeather();
                tick = 0;
            }
            tick += 1;
        }

        private void updateServerTime()
        {
            serverTime = API.getTime();
            serverTime = serverTime.Add(addOnTime);
            API.setTime(serverTime.Hours, serverTime.Minutes);
        }

        private void API_onResourceStart()
        {
            API.setWeather(0);
            API.setTime(serverTime.Hours, serverTime.Minutes);
        }

        private void updateServerWeather()
        {
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
    }
}
