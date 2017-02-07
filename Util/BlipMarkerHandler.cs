﻿using GTANetworkServer;
using GTANetworkShared;

namespace stuykserver.Util
{
    public class BlipMarkerHandler : Script
    {
        Main main = new Main();
        SpawnPoints spawnPoints = new SpawnPoints();

        public BlipMarkerHandler()
        {
            InitializeServerSpawnPoints();
            InitializeClothingShops();
            InitializeFishingPoints();
            InitializePizzaShops();

            API.onResourceStart += API_onResourceStart;
        }

        private void API_onResourceStart()
        {
            API.consoleOutput("Started: BlipMarkerHandler");
        }

        // Creates Markers and Blips for Spawn Points
        public void InitializeServerSpawnPoints()
        {
            foreach (Vector3 spawnPoint in spawnPoints.ServerSpawnPoints)
            {
                var marker = API.createMarker(1, new Vector3(spawnPoint.X, spawnPoint.Y, spawnPoint.Z - 4), new Vector3(), new Vector3(), new Vector3(2, 2, 5), 75, 255, 255, 0, 0);
                var blip = API.createBlip(spawnPoint);
                API.setBlipSprite(blip, 162);
            }
        }

        // Creates Markers and Blips for Clothing Shops
        public void InitializeClothingShops()
        {
            foreach (Vector3 shop in spawnPoints.ClothingSpawnPoints)
            {
                API.createMarker(1, new Vector3(shop.X, shop.Y, shop.Z - 4), new Vector3(), new Vector3(), new Vector3(2, 2, 5), 125, 0, 255, 0, 0);
                API.createTextLabel("~y~Usage: ~w~/skin HASHID", shop, 20, 0.5f);
                API.createTextLabel("~y~Skins: ~w~www.slice.wikidot.com", new Vector3(shop.X, shop.Y, shop.Z - 0.3), 20, 0.4f);
                var blip = API.createBlip(shop);
                API.setBlipSprite(blip, 73);
            }
        }

        // Creates Marks and Blips for Fishing Points
        private void InitializeFishingPoints()
        {
            foreach (Vector3 fishingPoint in spawnPoints.FishingSpawnPoints)
            {
                var marker = API.createMarker(1, new Vector3(fishingPoint.X, fishingPoint.Y, fishingPoint.Z - 4), new Vector3(), new Vector3(), new Vector3(4, 4, 5), 75, 255, 255, 0, 0);
                API.createTextLabel("~y~Usage: ~w~/beginjob", fishingPoint, 20, 0.5f);
                API.createTextLabel("~w~You can fish here.", new Vector3(fishingPoint.X, fishingPoint.Y, fishingPoint.Z - 0.3), 20, 0.4f);
                var blip = API.createBlip(fishingPoint);
                API.setBlipSprite(blip, 68);
            }
        }

        public void InitializePizzaShops()
        {
            foreach (Vector3 pizzaPoint in spawnPoints.PizzaDeliveryPoints)
            {
                var marker = API.createMarker(1, new Vector3(pizzaPoint.X, pizzaPoint.Y, pizzaPoint.Z - 4), new Vector3(), new Vector3(), new Vector3(2, 2, 5), 75, 70, 210, 240, 0);
                API.createTextLabel("~y~Usage: ~w~/beginjob", pizzaPoint, 20, 0.5f);
                API.createTextLabel("~w~Pizza delivery job, deliver pizzas.", new Vector3(pizzaPoint.X, pizzaPoint.Y, pizzaPoint.Z - 0.3), 20, 0.4f);
                var blip = API.createBlip(pizzaPoint);
                API.setBlipSprite(blip, 500);
            }
        }
    }
}
