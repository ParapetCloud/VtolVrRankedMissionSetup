using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VtolVrRankedMissionSetup.VTS;

namespace VtolVrRankedMissionSetup.Step
{
    public class SelectBaseStep
    {
        public BaseInfo BaseA { get; set; }
        public BaseInfo BaseB { get; set; }

        public void Start(CustomScenario scenario)
        {
            bool enterPressed = false;

            while (true)
            {
                Render(scenario);
                if (enterPressed)
                {
                    if (BaseA == null)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("You must select a base for team A");
                        Console.ResetColor();
                    }
                    else if (BaseB == null)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("You must select a base for team B");
                        Console.ResetColor();
                    }
                    else
                        break;
                }

                string typed = Console.ReadLine()!;

                if (string.IsNullOrWhiteSpace(typed))
                {
                    enterPressed = true;
                    continue;
                }
                else
                {
                    enterPressed = false;
                }

                BaseInfo? selected = null;

                if (int.TryParse(typed, out int id))
                {
                    selected = scenario.Bases.SingleOrDefault(b => b.Prefab.Id == id);
                }

                if (selected != null)
                {
                    Render(scenario, selected);

                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    --Console.CursorTop;
                    Console.CursorLeft = 0;
                    Console.WriteLine("Press A for team A, B for team B, or any other key to clear                                                                      ");
                    Console.ResetColor();

                    ConsoleKeyInfo key = Console.ReadKey();

                    if (key.Key == ConsoleKey.A)
                    {
                        BaseA = selected;

                        if (selected == BaseB)
                            BaseB = null;
                    }
                    else if (key.Key == ConsoleKey.B)
                    {
                        BaseB = selected;

                        if (selected == BaseA)
                            BaseA = null;
                    }
                    else if (selected == BaseA)
                        BaseA = null;
                    else if (selected == BaseB)
                        BaseB = null;
                }
            }
        }

        private void Render(CustomScenario scenario, BaseInfo? selected = null)
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Select the team bases");
            Console.WriteLine();

            foreach (BaseInfo baseInfo in scenario.Bases)
            {
                if (baseInfo == selected)
                {
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.White;
                }
                else if (baseInfo == BaseA)
                {
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.Blue;
                }
                else if (baseInfo == BaseB)
                {
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.Red;
                }
                else
                {
                    Console.ResetColor();
                }

                Console.WriteLine($"{baseInfo.Prefab.BaseName} ({baseInfo.Prefab.Id})");
            }

            Console.ResetColor();
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Type the ID of the base and hit enter to change the team, or just hit enter to continue");
            Console.ResetColor();
        }
    }
}
