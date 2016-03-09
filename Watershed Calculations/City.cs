using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Watershed_Calculations
{
    class City
    {
        private List<Building> city;
        private List<Building> buildings;        
        private int citySize;       
        
        public City(int totalBuildSize)
        {
            city = new List<Building>();
            buildings = new List<Building>();
            citySize = totalBuildSize;
        }

        /// <summary>
        /// Adds a new building that can be constructed.
        /// </summary>
        /// <param name="b"></param>
        public void AddBuilding(Building b)
        {
            buildings.Add(b);
        }

        /// <summary>
        /// Returns a building with the equivalent name passed as a parameter. 
        /// Returns null if a building isn't found.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public Building GetBuilding(String s)
        {
            return buildings.Find(b => b.Name.ToLower().Equals(s.ToLower()));
        }

        /// <summary>
        /// Returns the total number of build points.
        /// </summary>
        /// <returns></returns>
        public int GetAllBuildPoints()
        {
            int result = 0;
            foreach (Building building in city)
            {
                result += building.BuildPoints;
            }
            return result;
        }        

        public override String ToString()
        {
            StringBuilder cityFormat = new StringBuilder();

            Queue<String> buildingsBuilt = new Queue<String>();

            foreach (Building b in city)
            {
                for (int i = 0; i < b.Hexes; i++)
                {
                    buildingsBuilt.Enqueue(b.Name);
                }
            }
            
            for (int i = buildingsBuilt.Count; i < 30; i++)
            {
                buildingsBuilt.Enqueue(String.Empty);
            }

            int index = 1;
            foreach (String s in buildingsBuilt)
            {
                cityFormat.AppendFormat("| {0} ", s);
                if (index % 5 == 0)
                {
                    index = 0;
                    cityFormat.AppendLine("|");
                }
                index++;
            }

            buildingsBuilt.Clear();
            buildingsBuilt = null;

            return cityFormat.ToString();
        }

        /// <summary>
        /// Returns the total number of build points per category.
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public int GetBuildPoints(Category category)
        {
            int result = 0;
            foreach (Building building in city.FindAll(c => c.Category == category))
            {
                result += building.BuildPoints;
            }
            return result;
        }

        /// <summary>
        /// Returns true if the specified building can be built. False, otherwise.
        /// Verifies that only 1 unique building can exist at any time and total hexes is less than the city size.
        /// </summary>
        /// <param name="building"></param>
        /// <param name="leftoverBP"></param>
        /// <param name="baseBP"></param>
        /// <returns></returns>
        public bool CanBuild(Building building, int minOtherBP, int baseBP)
        {
            int totalBuiltHexes = 0;
            foreach (Building b in city)
            {
                totalBuiltHexes += b.Hexes;
            }

            bool buildingAlreadyExists = city.Exists(b => b.Name.Equals(building.Name));
            bool hasMinBP = GetBuildPoints(building.Category) >= 2;
            
            if (building.IsUnique && (!buildingAlreadyExists && !hasMinBP) && totalBuiltHexes < citySize)
            {
                return false;
            }
            else
            {
                return minOtherBP >= baseBP - 1;
            }
        }

        public int GetMinOtherBuildPoints(Category c)
        {
            int leftoverBP = 0;
            switch (c)
            {
                case Category.Agriculture:
                    leftoverBP = new List<int> { GetBuildPoints(Category.Industry), GetBuildPoints(Category.ForestryManagement), GetBuildPoints(Category.Residential) }.Min();
                    break;
                case Category.ForestryManagement:
                    leftoverBP = new List<int> { GetBuildPoints(Category.Agriculture), GetBuildPoints(Category.Industry), GetBuildPoints(Category.Residential) }.Min();
                    break;
                case Category.Industry:
                    leftoverBP = new List<int> { GetBuildPoints(Category.Agriculture), GetBuildPoints(Category.ForestryManagement), GetBuildPoints(Category.Residential) }.Min();
                    break;
                case Category.Residential:
                    leftoverBP = new List<int> { GetBuildPoints(Category.Agriculture), GetBuildPoints(Category.Industry), GetBuildPoints(Category.ForestryManagement) }.Min();
                    break;
            }
            return leftoverBP;
        }

        public double GetSoilLoss()
        {
            double soilLoss = 0.0;
            double mitigation = 0.0;
            foreach (Building building in city)
            {
                if (building.IsMitigation)
                {
                    mitigation += building.SoilLoss;
                }
                else
                {
                    soilLoss += building.SoilLoss;
                }                
            }
            // mitigation should always be negative therefore we add?
            return soilLoss + (soilLoss * mitigation);
        }

        public double GetWaterUsage()
        {
            double waterUsage = 0.0;
            double mitigation = 0.0;
            foreach (Building building in city)
            {
                if (building.IsMitigation)
                {
                    mitigation += building.WaterUsage;
                }
                else
                {
                    waterUsage += building.WaterUsage;
                }                
            }
            return waterUsage + (waterUsage * mitigation);
        }

        public double GetImperviousSurface()
        {
            double imperviousSurface = 0.0;
            double mitigation = 0.0;
            foreach (Building building in city)
            {
                if (building.IsMitigation)
                {
                    mitigation += building.ImperviousSurface;
                }
                else
                {
                    imperviousSurface += building.ImperviousSurface;
                }                
            }
            return imperviousSurface + (imperviousSurface * mitigation);
        }

        public double GetPollution()
        {
            double pollution = 0.0;
            double mitigation = 0.0;
            foreach (Building building in city)
            {
                if (building.IsMitigation)
                {
                    mitigation += building.Pollution;
                }
                else
                {
                    pollution += building.Pollution;
                }                
            }
            return pollution + (pollution * mitigation);
        }

        public bool BuildBuilding(Building building)
        {
            bool built = false;
            if (building != null && CanBuild(building, GetMinOtherBuildPoints(building.Category), GetBuildPoints(building.Category)))
            {
                city.Add(building);
                built = true;
            }

            return built;
        }

        /// <summary>
        /// sf = is + wu
        /// wp = is + p + sl
        /// sd = is + sl + p
        /// wt = p + is
        /// </summary>
        /// <returns></returns>
        public double GetEcosystemHealth()
        {
            double streamFlow = GetImperviousSurface() + GetWaterUsage();
            double waterPollution = GetImperviousSurface() + GetPollution() + GetSoilLoss();
            double speciesDeath = GetImperviousSurface() + GetSoilLoss() + GetPollution();
            double waterTemp = GetPollution() + GetImperviousSurface();

            double ecoHealth = (streamFlow + waterPollution + speciesDeath + waterPollution) / 6;            

            return ecoHealth;            
        }
    }
}