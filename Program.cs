using VtolVrRankedMissionSetup.Step;
using VtolVrRankedMissionSetup.VT;
using VtolVrRankedMissionSetup.VTM;
using VtolVrRankedMissionSetup.VTS;

namespace VtolVrRankedMissionSetup
{
    internal class Program
    {
        static void Main(string[] args)
        {
            SelectMapStep selectMap = new();
            selectMap.Start();

            if (selectMap.Map.StaticPrefabs == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Map '{selectMap.Map.MapID}' does not have any bases");
                Console.ResetColor();

                return;
            }

            if (selectMap.Map.StaticPrefabs.Count(prefab => prefab.Prefab.StartsWith("airbase")) < 2)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Map '{selectMap.Map.MapID}' does not have enough bases");
                Console.ResetColor();

                return;
            }

            CustomScenario scenario = new(selectMap.Map)
            {
                ScenarioName = $"BVR {selectMap.Map.MapID}",
                ScenarioID = $"BVR {selectMap.Map.MapID}",
                CampaignOrderIdx = 0,
            };

            SelectBaseStep selectBase = new();
            selectBase.Start(scenario);

            PlaceEverythingStep placeEverything = new();

            placeEverything.Start(scenario, selectBase.BaseA, selectBase.BaseB);

            VTSerializer.SerializeToFile(scenario, $"C:\\Program Files (x86)\\Steam\\steamapps\\common\\VTOL VR\\CustomScenarios\\Campaigns\\Headless Server\\BVR {selectMap.Map.MapID}\\BVR {selectMap.Map.MapID}.vts");
        }

    }
}
