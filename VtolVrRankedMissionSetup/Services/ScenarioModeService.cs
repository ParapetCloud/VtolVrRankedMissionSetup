using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VtolVrRankedMissionSetup.Attributes;
using VtolVrRankedMissionSetup.Configs;
using VtolVrRankedMissionSetup.Configs.AirbaseLayout;
using VtolVrRankedMissionSetup.Configs.ScenarioMode;
using Windows.Storage;

namespace VtolVrRankedMissionSetup.Services
{
    [Service(ServiceLifetime.Singleton)]
    public class ScenarioModeService
    {
        public ScenarioModeConfig ActiveMode { get; set; }
        public Dictionary<string, ScenarioModeConfig> Configs { get; set; } = [];

        public ScenarioModeService()
        {
            LoadConfig();

            // Look at this hard coding!
            ActiveMode = Configs["HS"];
        }

        private void LoadConfig()
        {
            Configs.Clear();

            DirectoryInfo directoryInfo = new("Configs/ScenarioMode");
            FileInfo[] files = directoryInfo.GetFiles("*.json");

            foreach (FileInfo file in files)
            {
                string name = file.Name[0..(file.Name.Length - 5)];
                try
                {
                    // This really should have some logging...

                    ScenarioModeConfig? config = JsonSerializer.Deserialize<ScenarioModeConfig>(File.ReadAllText(file.FullName), ConfigSerialization.SerializerOptions)!;

                    if (config == null)
                        continue;

                    Configs.Add(name, config);
                }
                catch (JsonException) { }
            }
        }
    }
}
