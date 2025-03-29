using System.Text.Json.Serialization;
using VtolVrRankedMissionSetup.Configs.AirbaseLayout;
using VtolVrRankedMissionSetup.Configs.ScenarioMode;

namespace VtolVrRankedMissionSetup.Configs
{
    [JsonSourceGenerationOptions(WriteIndented = true, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
    [JsonSerializable(typeof(AirbaseLayoutConfig))]
    [JsonSerializable(typeof(ScenarioModeConfig))]
    public partial class ConfigSerialization : JsonSerializerContext
    {
    }
}
