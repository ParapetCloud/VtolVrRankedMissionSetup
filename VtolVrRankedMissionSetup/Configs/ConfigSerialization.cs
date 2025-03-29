using System.Text.Json.Serialization;
using VtolVrRankedMissionSetup.Configs.AirbaseLayout;
using VtolVrRankedMissionSetup.Configs.ScenarioMode;

namespace VtolVrRankedMissionSetup.Configs
{
    [JsonSourceGenerationOptions(
        WriteIndented = true,
        Converters =
        [
            typeof(Vector3JsonConverter),
        ],
        PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
        DictionaryKeyPolicy = JsonKnownNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true)]
    [JsonSerializable(typeof(AirbaseLayoutConfig))]
    [JsonSerializable(typeof(ScenarioModeConfig))]
    public partial class ConfigSerialization : JsonSerializerContext
    {
    }
}
