using GTANetworkServer;
using GTANetworkShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stuykserver.Classes
{
    public class ShopInformationHandling : Script, IDisposable
    {
        public ShopInformationHandling()
        {
            shopEmployees = new Dictionary<string, EmployeeRank>();
            shopEmployeePayrates = new Dictionary<EmployeeRank, int>();
            outsidePlayers = new List<Client>();
            insidePlayers = new Dictionary<Client, NetHandle>();
            shopObjects = new List<GTANetworkServer.Object>();
            shopKeys = new List<Client>();
        }

        // SHOP OWNERSHIP PROPERTIES
        public enum EmployeeRank
        {
            Employee,
            AssistantManager,
            Manager,
            Owner
        }

        string shopOwner; // Used to set the Owner of the shop.
        Dictionary<string, EmployeeRank> shopEmployees; // Shop Employees with Ranks.
        Dictionary<EmployeeRank, int> shopEmployeePayrates; // Employee Ranks + PayRates.
        List<Client> shopKeys; // Exactly what it is.
        int shopBalance; // Amount of cash the shop has.

        // SHOP PROPERTIES
        public enum ShopType
        {
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
            House,
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
            Sale,
            apa_v_mp_h_01_a,
            apa_v_mp_h_01_c,
            apa_v_mp_h_01_b,
            apa_v_mp_h_02_a,
            apa_v_mp_h_02_c,
            apa_v_mp_h_02_b,
            apa_v_mp_h_03_a,
            apa_v_mp_h_03_c,
            apa_v_mp_h_03_b,
            apa_v_mp_h_04_a,
            apa_v_mp_h_04_b,
            apa_v_mp_h_04_c,
            apa_v_mp_h_05_a,
            apa_v_mp_h_05_c,
            apa_v_mp_h_05_b,
            apa_v_mp_h_06_a,
            apa_v_mp_h_06_b,
            apa_v_mp_h_06_c,
            apa_v_mp_h_07_a,
            apa_v_mp_h_07_c,
            apa_v_mp_h_07_b,
            apa_v_mp_h_08_a,
            apa_v_mp_h_08_b,
            apa_v_mp_h_08_c
        }

        ColShape collisionShape; // Used for the main point.
        int collisionID; // Pulled from the Database.
        Vector3 collisionPosition; // Position for the collision, point of Exit / Entrance.
        Blip collisionBlip; // Blip attached to the Collision.
        List<Client> outsidePlayers; // Players inside of the collision.
        Dictionary<Client, NetHandle> insidePlayers; // Players inside of an interior or shop.
        ShopType shopType; // Determines what icons go with the shop.

        //Optional Properties
        Vector3 shopCenterPoint; // Used to determine where the player is positioned.
        Vector3 shopCameraPoint; // Used to determine where the camera faces the player from.
        Vector3 shopExit; // Used as the exit point. (Vehicle Shops use this.)
        ColShape shopExitCollision;
        List<GTANetworkServer.Object> shopObjects; // Used as a container for any shop objects.
        bool forSale; // Mostly used for HOUSING.
        int shopPrice; // Mostly used for HOUSING.
        int shopDimension; // Mostly used for HOUSING.

        // SHOP OWNER
        public void setShopOwner(string value)
        {
            shopOwner = value;
        }

        public string returnShopOwner()
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
            if (value == true)
            {
                API.setBlipSprite(collisionBlip, 374);
                forSale = true;
            }
            else
            {
                API.setBlipSprite(collisionBlip, 40);
                forSale = false;
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
            GC.SuppressFinalize(this);
        }
    }
}
