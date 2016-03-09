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

        public String DisplayCityDetails()
        {
            StringBuilder details = new StringBuilder();            

            details.AppendLine(city.ToString());            
            details.AppendFormat("\n\nArgiculture Build Points: {0}\n", city.GetBuildPoints(Category.Agriculture));
            details.AppendFormat("Residential Build Points: {0}\n", city.GetBuildPoints(Category.Residential));
            details.AppendFormat("Industry Build Points: {0}\n", city.GetBuildPoints(Category.Industry));
            details.AppendFormat("Forestry Management Build Points: {0}\n\n", city.GetBuildPoints(Category.ForestryManagement));

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

        public String DisplayEcoDetails()
        {
            StringBuilder details = new StringBuilder();

            details.AppendFormat("Impervious Surface: {0}\nWater Usage: {1}\nPollution: {2}\nSoil Loss: {3}\n\n", city.GetImperviousSurface(), city.GetWaterUsage(), city.GetPollution(), city.GetSoilLoss());

            double streamFlow = city.GetImperviousSurface() + city.GetWaterUsage();
            double waterPollution = city.GetImperviousSurface() + city.GetPollution() + city.GetSoilLoss();
            double speciesDeath = city.GetImperviousSurface() + city.GetSoilLoss() + city.GetPollution();
            double waterTemp = city.GetPollution() + city.GetImperviousSurface();

            details.AppendFormat("Stream Flow: {0}\nWater Pollution: {1}\nSpecies Death: {2}\nWater Temperature: {3}", streamFlow, waterPollution, speciesDeath, waterTemp);

            return details.ToString();
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
