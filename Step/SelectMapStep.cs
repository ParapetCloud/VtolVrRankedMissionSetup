using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VtolVrRankedMissionSetup.VT;
using VtolVrRankedMissionSetup.VTM;
using VtolVrRankedMissionSetup.VTS;

namespace VtolVrRankedMissionSetup.Step
{
    public class SelectMapStep
    {
        public VTMapCustom Map { get; set; }

        public void Start()
        {
            DirectoryInfo info = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Steam\\steamapps\\common\\VTOL VR\\CustomMaps"));

            string searchString = string.Empty;
            int highlighted = 0;

            while (true)
            {
                DirectoryInfo[] maps = info.GetDirectories($"*{searchString}*");
                Render(maps, highlighted);

                Console.Write(searchString);

                ConsoleKeyInfo typed = Console.ReadKey()!;

                if (typed.Key == ConsoleKey.UpArrow)
                {
                    if (highlighted > 0)
                        --highlighted;
                }
                else if (typed.Key == ConsoleKey.DownArrow)
                {
                    if (highlighted < maps.Length - 1)
                        ++highlighted;
                }
                else if (typed.Key == ConsoleKey.Backspace)
                {
                    if (searchString.Length > 0)
                        searchString = searchString[0..(searchString.Length - 1)];
                }
                else if (typed.Key == ConsoleKey.Enter)
                {
                    DirectoryInfo highlightedDirectory = maps[highlighted];

                    Map = VTSerializer.DeserializeFromFile<VTMapCustom>(Path.Combine(highlightedDirectory.FullName, $"{highlightedDirectory.Name}.vtm"));
                    break;
                }
                else if (typed.Key == ConsoleKey.Escape)
                {
                    // no op
                }
                else if (typed.KeyChar != 0)
                    searchString += typed.KeyChar;
            }
        }

        private void Render(DirectoryInfo[] maps, int highlight)
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Select the map to use");
            Console.WriteLine();


            Console.ResetColor();

            for (int i = 0; i < maps.Length; ++i)
            {
                if (i == highlight)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                else
                    Console.ResetColor();

                Console.WriteLine(maps[i].Name);
            }

            Console.ResetColor();
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Press the up and down arrows to select");
            Console.ResetColor();
        }
    }
}
