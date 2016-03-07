using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Watershed_Calculations
{
    public class Board
    {
        private City city;
        private int totalBuildPoints;
        private int totalEcosystemHealth;  
        private Building[] buildingsThisTurn;
        private int turn;

        public Board (int tbp, int teh, int tbs)
        {
            city = new City(tbs);
            totalBuildPoints = tbp;
            totalEcosystemHealth = teh;

            buildingsThisTurn = new Building[4];
            turn = 0;
        }

        public void StartGame()
        {
            turn = 0;
            buildingsThisTurn = new Building[4];
        }

        public bool LoadBuildings(IEnumerable<String> buildingValues)
        {           
            try
            {
                foreach (String s in buildingValues.Skip(1))
                {
                    String[] parts = s.Split(',');
                    String name = parts[0];
                    Category cat = CategoryExtensions.ConvertToCategory(parts[1]);
                    int hex = Int32.Parse(parts[2]);
                    int bp = Int32.Parse(parts[3]);
                    double iS = Double.Parse(parts[4]);
                    double pol = Double.Parse(parts[5]);
                    double sl = Double.Parse(parts[6]);
                    double wu = Double.Parse(parts[7]);
                    bool unique = bool.Parse(parts[8]);

                    city.AddBuilding(new Building(cat, hex, bp, iS, pol, wu, sl, name, unique));
                }
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }

        public bool HasWon()
        {
            return city.GetAllBuildPoints() >= totalBuildPoints;
        }
        
        public bool HasLost()
        {
            return city.GetEcosystemHealth() >= totalEcosystemHealth;
        }

        public void NextTurn()
        {
            foreach (Building b in buildingsThisTurn)
            {
                city.BuildBuilding(b);
            }

            buildingsThisTurn = new Building[4];
            turn++;
        }

        public String GetCityDetails()
        {
            StringBuilder details = new StringBuilder();            

            details.AppendLine(city.ToString());
            details.AppendLine();
            details.AppendFormat("Total Build Points: {0}\n", city.GetAllBuildPoints());
            details.AppendFormat("Ecosystem Health: {0}\n", city.GetEcosystemHealth());
            details.AppendFormat("Remaining Buildings this turn: {0}", RemainingBuildingsThisTurn());

            return details.ToString();
        }

        private int RemainingBuildingsThisTurn()
        {
            int buildingsBuilt = 0;
            for (int i = 0; i < buildingsThisTurn.Count(); i++)
            {
                if (buildingsThisTurn[i] != null)
                {
                    buildingsBuilt++;
                }
            }
            return 4 - buildingsBuilt;
        }

        /// <summary>
        /// Doesn't actually build the buildings. Just verifies that the build can be built,
        /// and then adds it to the queue to be added at the end of the turn.
        /// </summary>
        /// <param name="sBuilding"></param>
        /// <returns></returns>
        public bool BuildBuilding(String sBuilding)
        {
            Building building = city.GetBuilding(sBuilding);
            bool built = false;

            if (building != null 
                && CheckCategoryBuilt((int)building.Category, building) 
                && city.CanBuild(building,
                                 city.GetMinOtherBuildPoints(building.Category),
                                 city.GetBuildPoints(building.Category)))
            {
                SetCategoryBuilt((int)building.Category, building);
                built = true;
            }

            return built;
        }

        private bool CheckCategoryBuilt(int index, Building b)
        {
            bool result = false;
            if (buildingsThisTurn[index] == null)
            {                
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }

        private void SetCategoryBuilt(int index, Building b)
        {
            if (buildingsThisTurn[index] == null)
            {
                buildingsThisTurn[index] = b;
            }
        }
    }
}
