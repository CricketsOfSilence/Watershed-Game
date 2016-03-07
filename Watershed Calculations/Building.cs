using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Watershed_Calculations
{
    public class Building
    {
        private Category category;
        public Category Category { get { return category; } }        
        private int hexes;
        public int Hexes { get { return hexes; } }
        private int buildPoints;
        public int BuildPoints { get { return buildPoints; } }
        private double imperviousSurface;
        public double ImperviousSurface { get { return imperviousSurface; } }
        private double pollution;
        public double Pollution { get { return pollution; } }
        private double soilLoss;
        public double SoilLoss { get { return soilLoss; } }
        private double waterUsage;
        public double WaterUsage { get { return waterUsage; } }
        private String name;
        public String Name { get { return name; } }
        private bool isUnique;
        public bool IsUnique { get { return isUnique; } }
        private bool isMitigation;
        public bool IsMitigation { get { return isMitigation; } }

        public Building (Category cat, int nHex, int nBP, double nIS, double nP, double nWU, double nSL, String bName, bool unique)
        {
            category = cat;
            hexes = nHex;
            buildPoints = nBP;
            imperviousSurface = nIS;
            pollution = nP;
            waterUsage = nWU;
            soilLoss = nSL;
            name = bName;
            isUnique = unique;
            isMitigation = nIS < 0 || nP < 0 || nWU < 0 || nSL < 0;
        }

        public override string ToString()
        {
            return name;
        }
    }    

    public enum Category
    {
        Residential = 0,
        Agriculture,
        Industry,
        ForestryManagement,
        Empty
    };

    public static class CategoryExtensions
    {
        public static Category ConvertToCategory(String s)
        {
            Category result = Category.Empty;
            switch (s)
            {
                case "Residential":
                    result = Category.Residential;
                    break;
                case "Agriculture":
                    result = Category.Agriculture;
                    break;
                case "Industry":
                    result = Category.Industry;
                    break;
                case "Forestry Management":
                    result = Category.ForestryManagement;
                    break;
                default:
                    throw new ArgumentException();
            }
            return result;
        }
    }
}
