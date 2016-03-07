using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watershed_Calculations;

namespace Watershed_Game
{
    class Program
    {
        private static Board gameBoard;
        private static String buildingFile;      

        static void Main(string[] args)
        {
            gameBoard = new Board(25, 100, 30);
            buildingFile = String.Empty;

            while (!gameBoard.HasWon() && !gameBoard.HasLost())
            {
                Console.Out.Write("> ");
                String[] line = Console.ReadLine().Split(' ');

                switch (line[0])
                {
                    case "l":
                    case "load":

                        String path = String.Join(" ", line.Skip(1));
                        //path = path.Replace("\"\"", "");

                        if (loadFile(path))
                        {
                            Console.Out.WriteLine("File loaded");
                            buildingFile = path;
                        }
                        else
                        {
                            Console.Out.WriteLine("File failed to be loaded.");
                        }
                        break;
                    case "s":
                    case "start":
                        gameBoard.StartGame();
                        Console.Out.WriteLine("Game started");
                        break;
                    case "b":
                    case "build":                        
                        Console.Out.WriteLine(String.Format("Building built: {0}", gameBoard.BuildBuilding(String.Join(" ", line.Skip(1)))));
                        break;
                    case "n":
                    case "next":                        
                        gameBoard.NextTurn();
                        Console.Out.WriteLine(gameBoard.GetCityDetails());
                        break;
                    case "r":
                    case "reset":
                        gameBoard.StartGame();
                        break;
                    case "p":
                    case "points":
                        Console.Out.WriteLine(gameBoard.GetCityDetails());
                        break;
                    case "h":
                    case "help":
                        StringBuilder h = new StringBuilder();
                        h.AppendLine("Commands:");
                        h.AppendFormat("\t load <file path> | l <file path>\n");
                        h.AppendFormat("\t start | s\n");
                        h.AppendFormat("\t build <building name> | b <building name>\n");
                        h.AppendFormat("\t next | n\n");
                        h.AppendFormat("\t reset | r\n");
                        h.AppendFormat("\t points | p\n");
                        h.AppendFormat("\t help | h\n");
                        Console.Out.WriteLine(h.ToString());
                        break;
                    default:
                        Console.Out.WriteLine("Unrecognized command");
                        break;                                            
                }
            }

            if (gameBoard.HasLost())
            {
                Console.Out.WriteLine("You lost.");                
            }
            else
            {
                Console.Out.WriteLine("You win!");
            }

            Console.Out.WriteLine("Press any key to continue.");
            Console.ReadLine();            
        }

        private static bool loadFile(String filePath)
        {
            return gameBoard.LoadBuildings(File.ReadAllLines(filePath));
        }
    }
}
