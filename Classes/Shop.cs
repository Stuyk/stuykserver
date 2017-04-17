using GTANetworkServer;
using GTANetworkShared;
using stuykserver.Classes;
using stuykserver.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stuykserver.Classes
{
    public class Shop : Script, IDisposable
    {
        DatabaseHandler db = new DatabaseHandler();

        public Shop()
        {
            // Unused
        }

        public Shop(DataRow row)
        {
            shopID = Convert.ToInt32(row["ID"]);
            collisionID = shopID;
            shopOwner = Convert.ToInt32(row["PlayerID"]);
            setShopUnits(Convert.ToInt32(row["Units"]));
            shopBalance = Convert.ToInt32(row["Money"]);
            shopType = (ShopType)row["Type"];
            collisionPosition = new Vector3(Convert.ToSingle(row["PosX"]), Convert.ToSingle(row["PosY"]), Convert.ToSingle(row["PosZ"]));
            collisionRotation = new Vector3(Convert.ToSingle(row["RotX"]), Convert.ToSingle(row["RotY"]), Convert.ToSingle(row["RotZ"]));
            shopExit = new Vector3(Convert.ToSingle(row["ExitX"]), Convert.ToSingle(row["ExitY"]), Convert.ToSingle(row["ExitZ"]));
            shopCenterPoint = new Vector3(Convert.ToSingle(row["FocusX"]), Convert.ToSingle(row["FocusY"]), Convert.ToSingle(row["FocusZ"]));
            shopCameraPoint = new Vector3(Convert.ToSingle(row["CamX"]), Convert.ToSingle(row["CamY"]), Convert.ToSingle(row["CamZ"]));
            forSale = Convert.ToBoolean(row["ForSale"]);
            shopPrice = Convert.ToInt32(row["Price"]);
            range = Convert.ToSingle(row["Radius"]);
            height = Convert.ToSingle(row["Height"]);
            outsidePlayers = new List<Client>();
            insidePlayers = new Dictionary<Client, NetHandle>();
            shopEmployees = new Dictionary<string, EmployeeRank>();
            shopKeys = new List<Client>();
            shopEmployeePayrates = new Dictionary<EmployeeRank, int>();
            shopObjects = new List<GTANetworkServer.Object>();
            // Setup our Door
            selectableObject = API.createObject(-1652821467, collisionPosition, collisionRotation, 0);
            API.setEntityTransparency(selectableObject, 0);
            selectableObject.setData("Instance", this);
            selectableObject.setSyncedData("Type", shopType.ToString());
            setupBlip();
        }

        // SHOP OWNERSHIP PROPERTIES
        public enum EmployeeRank
        {
            Employee,
            AssistantManager,
            Manager,
            Owner
        }

        TextLabel textLabel;
        int shopID;
        int shopOwner; // Used to set the Owner of the shop.
        ShopType shopType; // Determines what icons go with the shop.
        Dictionary<string, EmployeeRank> shopEmployees; // Shop Employees with Ranks.
        Dictionary<EmployeeRank, int> shopEmployeePayrates; // Employee Ranks + PayRates.
        List<Client> shopKeys; // Exactly what it is.
        int shopBalance; // Amount of cash the shop has.
        int shopUnits; // Units the shop has.
        GTANetworkServer.Object selectableObject;

        // SHOP PROPERTIES
        public enum ShopType
        {
            None,
            Atm,
            Barbershop,
            Boats,
            Classic,
            Clothing,
            Commercial,
            Compacts,
            Coupes,
            Bicycles,
            Helicopters,
            Industrial,
            Modification,
            Motorcycles,
            Muscle,
            OffRoad,
            Planes,
            Police,
            SUVS,
            Sedans,
            Sports,
            Super,
            Utility,
            Vans,
            Fishing,
            FishingSale,
            Repair,
            FuelPump
        }

        ColShape collisionShape; // Used for the main point.
        int collisionID; // Pulled from the Database.
        float range; // Collision Range
        float height; // Collision Height
        Vector3 collisionPosition; // Position for the collision, point of Exit / Entrance.
        Vector3 collisionRotation;
        Blip collisionBlip; // Blip attached to the Collision.
        List<Client> outsidePlayers; // Players inside of the collision.
        Dictionary<Client, NetHandle> insidePlayers; // Players inside of an interior or shop.
        
        //Optional Properties
        Vector3 shopCenterPoint; // Used to determine where the player is positioned.
        Vector3 shopCameraPoint; // Used to determine where the camera faces the player from.
        Vector3 shopExit; // Used as the exit point. (Vehicle Shops use this.)
        ColShape shopExitCollision;
        List<GTANetworkServer.Object> shopObjects; // Used as a container for any shop objects.
        bool forSale; // Mostly used for HOUSING.
        int shopPrice; // Mostly used for HOUSING.
        int shopDimension; // Mostly used for HOUSING.

        //Shop Messages
        string shopMessage;

        public void saveShop()
        {
            string[] varNames = { "PlayerID", "Units", "Money", "ForSale", "Price" };
            string before = "UPDATE Shops SET";
            object[] data = { shopOwner, shopUnits, shopBalance, forSale, shopPrice };
            string after = string.Format("WHERE ID='{0}'", shopID);

            // Send all our data to generate the query and run it
            db.compileQuery(before, after, varNames, data);
        }

        // Setup Collision
        public void setupCollision()
        {
            //collisionShape = API.createCylinderColShape(collisionPosition, range, height);
            //collisionShape.setData("Instance", this);
            //collisionShape.setData("Type", "Shop");
        }

        // SHOP ID
        public int returnShopID()
        {
            return shopID;
        }

        // SHOP MESSAGE
        public string returnShopMessage()
        {
            return shopMessage;
        }

        // SHOP OWNER
        public void setShopOwner(int value)
        {
            shopOwner = value;
        }

        public int returnShopOwner()
        {
            return shopOwner;
        }

        // SHOP Exit Collision
        public void setExitCollision(ColShape collision)
        {
            shopExitCollision = collision;
        }

        public ColShape returnExitCollisions()
        {
            return shopExitCollision;
        }

        // EXIT POINT
        public void setExitPoint(Vector3 position)
        {
            shopExit = position;
        }

        public Vector3 returnExitPoint()
        {
            return shopExit;
        }

        // SHOP EMPLOYEES
        public void setShopEmployees(Dictionary<string, EmployeeRank> employees)
        {
            foreach (string employee in employees.Keys)
            {
                if (!shopEmployees.ContainsKey(employee))
                {
                    shopEmployees.Add(employee, employees[employee]);
                }
                else
                {
                    shopEmployees.Set(employee, employees[employee]);
                }
            }
        }

        public void removeShopEmployees(List<string> employees)
        {
            foreach (string employee in employees)
            {
                if (shopEmployees.ContainsKey(employee))
                {
                    shopEmployees.Remove(employee);
                }
            }
        }

        public Dictionary<string, EmployeeRank> returnShopEmployees()
        {
            return shopEmployees;
        }

        // SHOP EMPLOYEE PAYRATES
        public void setShopEmployeePayrates(Dictionary<EmployeeRank, int> payrates)
        {
            foreach (EmployeeRank rank in payrates.Keys)
            {
                if (!shopEmployeePayrates.ContainsKey(rank))
                {
                    shopEmployeePayrates.Add(rank, payrates[rank]);
                }
                else
                {
                    shopEmployeePayrates.Set(rank, payrates[rank]);
                }
            }
        }

        public Dictionary<EmployeeRank, int> returnShopEmployeePayrates()
        {
            return shopEmployeePayrates;
        }

        // SHOP KEYS
        public void addShopKeys(Client player)
        {
            if (!shopKeys.Contains(player))
            {
                shopKeys.Add(player);
            }
        }

        public void removeShopKeys(Client player)
        {
            if (shopKeys.Contains(player))
            {
                shopKeys.Remove(player);
            }
        }

        public void clearShopKeys()
        {
            shopKeys.Clear();
        }

        public List<Client> returnShopKeys()
        {
            return shopKeys;
        }

        // SHOP BALANCE
        public void setShopBalance(int balance)
        {
            shopBalance = balance;
        }

        public int returnShopBalance()
        {
            return shopBalance;
        }

        public int addShopBalance(int balance)
        {
            shopBalance += balance;
            return shopBalance;
        }
        
        public int subtractShopBalance(int balance)
        {
            shopBalance -= balance;
            return shopBalance;
        }

        // SHOP PROPERTIES SETTING / GETTING
        // Collision Shape
        public void setCollisionShape(ColShape newColShape)
        {
            collisionShape = newColShape;
        }

        public ColShape returnCollisionShape()
        {
            return collisionShape;
        }

        // Collision ID
        public void setCollisionID(int id)
        {
            collisionID = id;
        }

        public int returnCollisionID()
        {
            return collisionID;
        }

        // Collision Position
        public void setCollisionPosition(Vector3 vector)
        {
            collisionPosition = vector;
        }

        public Vector3 returnCollisionPosition()
        {
            return collisionPosition;
        }

        // Blip
        public void setBlip(Blip blip)
        {
            collisionBlip = blip;
        }

        public Blip returnBlip()
        {
            return collisionBlip;
        }

        public void setupBlip()
        {
            collisionBlip = API.createBlip(collisionPosition);

            if (collisionBlip != null)
            {
                switch (shopType)
                {
                    case ShopType.None:
                        API.setBlipSprite(collisionBlip, 66);
                        break;
                    case ShopType.Atm:
                        API.setBlipSprite(collisionBlip, 108);
                        API.setBlipColor(collisionBlip, 2);
                        textLabel = API.createTextLabel(shopType.ToString(), collisionPosition, 10f, 0.8f, false);
                        shopMessage = "I can make a transaction with a bank here.";
                        break;
                    case ShopType.Barbershop:
                        API.setBlipSprite(collisionBlip, 71);
                        textLabel = API.createTextLabel(shopType.ToString(), collisionPosition, 10f, 0.8f, false);
                        shopMessage = "I can get a haircut here.";
                        break;
                    case ShopType.Motorcycles:
                        API.setBlipSprite(collisionBlip, 226);
                        API.setBlipColor(collisionBlip, 73);
                        textLabel = API.createTextLabel(string.Format("Dealership: {0}", shopType.ToString()), collisionPosition, 10f, 0.8f, false);
                        shopMessage = "There appears to be a lot of Motorcycles around.";
                        break;
                    case ShopType.Helicopters:
                        API.setBlipSprite(collisionBlip, 43);
                        API.setBlipColor(collisionBlip, 73);
                        textLabel = API.createTextLabel(string.Format("Dealership: {0}", shopType.ToString()), collisionPosition, 10f, 0.8f, false);
                        shopMessage = "There appears to be a lot of Helicopters around.";
                        break;
                    case ShopType.Industrial:
                        API.setBlipSprite(collisionBlip, 318);
                        API.setBlipColor(collisionBlip, 73);
                        textLabel = API.createTextLabel(string.Format("Dealership: {0}", shopType.ToString()), collisionPosition, 20f, 0.8f, false);
                        shopMessage = "There appears to be a lot of Industrial Vehicles around.";
                        break;
                    case ShopType.Commercial:
                        API.setBlipSprite(collisionBlip, 477);
                        API.setBlipColor(collisionBlip, 73);
                        textLabel = API.createTextLabel(string.Format("Dealership: {0}", shopType.ToString()), collisionPosition, 10f, 0.8f, false);
                        shopMessage = "There appears to be a lot of Commercial Vehicles around.";
                        break;
                    case ShopType.Planes:
                        API.setBlipSprite(collisionBlip, 251);
                        API.setBlipColor(collisionBlip, 73);
                        textLabel = API.createTextLabel(string.Format("Dealership: {0}", shopType.ToString()), collisionPosition, 10f, 0.8f, false);
                        shopMessage = "There appears to be a lot of Planes around.";
                        break;
                    case ShopType.Super:
                        API.setBlipSprite(collisionBlip, 147);
                        API.setBlipColor(collisionBlip, 73);
                        textLabel = API.createTextLabel(string.Format("Dealership: {0}", shopType.ToString()), collisionPosition, 10f, 0.8f, false);
                        shopMessage = "There appears to be a lot of Super Vehicles around.";
                        break;
                    case ShopType.Boats:
                        API.setBlipSprite(collisionBlip, 455);
                        API.setBlipColor(collisionBlip, 73);
                        textLabel = API.createTextLabel(string.Format("Dealership: {0}", shopType.ToString()), collisionPosition, 10f, 0.8f, false);
                        shopMessage = "There appears to be a lot of Boats around.";
                        break;
                    case ShopType.OffRoad:
                        API.setBlipSprite(collisionBlip, 512);
                        API.setBlipColor(collisionBlip, 73);
                        textLabel = API.createTextLabel(string.Format("Dealership: {0}", shopType.ToString()), collisionPosition, 10f, 0.8f, false);
                        shopMessage = "There appears to be a lot of Offroad Vehicles around.";
                        break;
                    case ShopType.Vans:
                        API.setBlipSprite(collisionBlip, 67);
                        API.setBlipColor(collisionBlip, 73);
                        textLabel = API.createTextLabel(string.Format("Dealership: {0}", shopType.ToString()), collisionPosition, 10f, 0.8f, false);
                        shopMessage = "There appears to be a lot of Vans around.";
                        break;
                    case ShopType.Bicycles:
                        API.setBlipSprite(collisionBlip, 348);
                        API.setBlipColor(collisionBlip, 73);
                        textLabel = API.createTextLabel(string.Format("Dealership: {0}", shopType.ToString()), collisionPosition, 10f, 0.8f, false);
                        shopMessage = "There appears to be a lot of Bicycles around.";
                        break;
                    case ShopType.Clothing:
                        API.setBlipSprite(collisionBlip, 73);
                        textLabel = API.createTextLabel(shopType.ToString(), collisionPosition, 10f, 0.8f, false);
                        shopMessage = "I could get a new set of clothes here.";
                        break;
                    case ShopType.FishingSale:
                        API.setBlipSprite(collisionBlip, 431);
                        API.setBlipColor(collisionBlip, 42);
                        textLabel = API.createTextLabel("The cooks are looking for fresh fish.", collisionPosition, 10f, 0.8f, false);
                        shopMessage = "I could probably sell any fish I get here.";
                        break;
                    case ShopType.Fishing:
                        API.setBlipSprite(collisionBlip, 68);
                        API.setBlipColor(collisionBlip, 42);
                        shopMessage = "I could catch some fish here.";
                        break;
                    case ShopType.Modification:
                        API.setBlipSprite(collisionBlip, 446);
                        shopMessage = "I could get my vehicle modified here.";
                        break;
                    case ShopType.Repair:
                        API.setBlipSprite(collisionBlip, 402);
                        API.setBlipColor(collisionBlip, 11);
                        textLabel = API.createTextLabel(string.Format("Vehicle {0}s", shopType.ToString()), collisionPosition, 10f, 0.8f, false);
                        shopMessage = "This seems like a place where I could get my vehicle fixed.";
                        break;
                    case ShopType.FuelPump:
                        API.setBlipSprite(collisionBlip, 361);
                        API.setBlipColor(collisionBlip, 59);
                        textLabel = API.createTextLabel(string.Format("~b~Pump: ~b~{0}/5000 Units", shopUnits), collisionPosition, 10f, 0.8f, true);
                        shopMessage = "This seems like a place where I could get my vehicle some gas.";
                        break;
                    default:
                        textLabel = API.createTextLabel(string.Format("Dealership: {0}", shopType.ToString()), collisionPosition, 10f, 0.8f, false);
                        API.setBlipSprite(collisionBlip, 225);
                        API.setBlipColor(collisionBlip, 73);
                        shopMessage = string.Format("There appears to be a lot of {0} Vehicles around.", shopType.ToString());
                        break;
                }
                API.setBlipShortRange(collisionBlip, true);
            }
        }

        // OutsidePlayers
        public void addOutsidePlayer(Client player)
        {
            if (!outsidePlayers.Contains(player))
            {
                outsidePlayers.Add(player);
            }
        }

        public void removeOutsidePlayer(Client player)
        {
            if (outsidePlayers.Contains(player))
            {
                outsidePlayers.Remove(player);
            }
        }

        public List<Client> returnOutsidePlayers()
        {
            return outsidePlayers;
        }

        // InsidePlayers
        public void addInsidePlayers(Dictionary<Client, NetHandle> dictionary)
        {
            foreach (Client player in dictionary.Keys)
            {
                if (!insidePlayers.ContainsKey(player))
                {
                    insidePlayers.Add(player, dictionary[player]);
                }
            }
        }

        public void addInsidePlayer(Client player, NetHandle handle)
        {
            if (!insidePlayers.ContainsKey(player))
            {
                insidePlayers.Add(player, handle);
            }
        }

        public void removeInsidePlayers(Dictionary<Client, NetHandle> dictionary)
        {
            foreach (Client player in dictionary.Keys)
            {
                if (insidePlayers.ContainsKey(player))
                {
                    insidePlayers.Remove(player);
                }
            }
        }

        public void removeInsidePlayer(Client player)
        {
            if (insidePlayers.ContainsKey(player))
            {
                insidePlayers.Remove(player);
            }
        }

        public Dictionary<Client, NetHandle> returnInsidePlayers()
        {
            return insidePlayers;
        }

        // Shop Type
        public void setShopType(ShopType type)
        {
            shopType = type;
        }

        public ShopType returnShopType()
        {
            return shopType;
        }

        // Shop Objects
        public void setShopObjects(List<GTANetworkServer.Object> list)
        {
            foreach (GTANetworkServer.Object obj in list)
            {
                if (!shopObjects.Contains(obj))
                {
                    shopObjects.Add(obj);
                }
            }
        }

        public void removeShopObjects(List<GTANetworkServer.Object> list)
        {
            foreach (GTANetworkServer.Object obj in list)
            {
                if (shopObjects.Contains(obj))
                {
                    shopObjects.Remove(obj);
                }
            }
        }

        public List<GTANetworkServer.Object> returnShopObjects()
        {
            return shopObjects;
        }

        // CAMERAS
        public void setCameraPoint(Vector3 camera)
        {
            shopCameraPoint = camera;
        }

        public void setCameraCenterPoint(Vector3 camera)
        {
            shopCenterPoint = camera;
        }

        public Vector3 returnCameraPoint()
        {
            return shopCenterPoint;
        }

        public Vector3 returnCameraCenterPoint()
        {
            return shopCameraPoint;
        }

        // For Sale
        public void setForSale(bool value)
        {
            switch (value)
            {
                case true:
                    API.setBlipSprite(collisionBlip, 374);
                    forSale = true;
                    break;
                case false:
                    API.setBlipSprite(collisionBlip, 40);
                    forSale = false;
                    break;
            }
        }

        public bool returnForSale()
        {
            return forSale;
        }

        // Shop Price
        public void setShopPrice(int value)
        {
            shopPrice = value;
        }

        public int returnShopPrice()
        {
            return shopPrice;
        }

        // Shop Dimensions
        public void setShopDimension(int value)
        {
            shopDimension = value;
        }

        public int returnShopDimension()
        {
            return shopDimension;
        }

        public void Dispose()
        {
            if (textLabel != null)
            {
                 API.deleteEntity(textLabel);
            }
            collisionID = -1;
            collisionPosition = null;
            API.deleteEntity(collisionBlip);
            outsidePlayers.Clear();
            insidePlayers.Clear();
            shopType = ShopType.None;
            shopCenterPoint = null;
            shopCameraPoint = null;
            shopExit = null;
            if (shopExitCollision != null)
            {
                API.deleteColShape(shopExitCollision);
            }

            if (collisionShape != null)
            {
                API.deleteColShape(collisionShape);
            }

            if (API.doesEntityExist(selectableObject))
            {
                API.deleteEntity(selectableObject);
            }

            foreach(GTANetworkServer.Object obj in shopObjects)
            {
                API.deleteEntity(obj);
            }
            GC.SuppressFinalize(this);
        }

        // Units
        public void setShopUnits(int value)
        {
            shopUnits = value;

            if (shopUnits >= 5000)
            {
                shopUnits = 5000;
            }

            if (shopUnits <= 0)
            {
                shopUnits = 0;
            }

            if (shopType == ShopType.FuelPump)
            {
                textLabel.text = string.Format("~b~Pump: ~b~{0}/5000 Units", shopUnits);
            }
        }

        public void addShopUnits(int value)
        {
            shopUnits += value;

            if (shopUnits >= 5000)
            {
                shopUnits = 5000;
            }

            if (shopType == ShopType.FuelPump)
            {
                textLabel.text = string.Format("~b~Pump: ~b~{0}/5000 Units", shopUnits);
            }
        }

        public void removeShopUnits(int value)
        {
            if (shopUnits <= 0)
            {
                if (shopType == ShopType.FuelPump)
                {
                    textLabel.text = string.Format("~b~Pump: ~b~{0}/5000 Units", shopUnits);
                }

                shopUnits = 0;
                return;
            }

            shopUnits -= value;

            if (shopType == ShopType.FuelPump)
            {
                textLabel.text = string.Format("~b~Pump: ~b~{0}/5000 Units", shopUnits);
            }
        }

        public int returnShopUnits()
        {
            return shopUnits;
        }
    }
}
