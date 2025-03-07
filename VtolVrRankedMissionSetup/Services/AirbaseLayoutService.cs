using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using VtolVrRankedMissionSetup.Configs;
using VtolVrRankedMissionSetup.Configs.AirbaseLayout;

namespace VtolVrRankedMissionSetup.Services
{
    [Service(ServiceLifetime.Singleton)]
    public class AirbaseLayoutService
    {
        private Dictionary<string, AirbaseLayoutConfig> configs = [];

        public AirbaseLayoutConfig GetConfig(string layout, string prefab)
        {
            string airbasePath = $"{layout}/{prefab}";

            if (!configs.TryGetValue(airbasePath, out AirbaseLayoutConfig? config))
            {
                config = JsonSerializer.Deserialize<AirbaseLayoutConfig>(File.ReadAllText($"Configs/AirbaseLayout/{airbasePath}.json"), ConfigSerialization.SerializerOptions)!;
                configs.Add(airbasePath, config);
            }

            return config;
        }
    }
}
