using System.Text.Json.Serialization;
using VtolVrRankedMissionSetup.Configs.AirbaseLayout;
using VtolVrRankedMissionSetup.Configs.ScenarioMode;
using VtolVrRankedMissionSetup.VT;

namespace VtolVrRankedMissionSetup.Configs
{
    [JsonSourceGenerationOptions(
        WriteIndented = true,
        Converters =
        [
            typeof(Vector3JsonConverter),
            typeof(AltSpawnJsonConverter),
            typeof(JsonStringEnumConverter<AircraftType>)
        ],
        PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
        DictionaryKeyPolicy = JsonKnownNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true)]
    [JsonSerializable(typeof(AirbaseLayoutConfig))]
    [JsonSerializable(typeof(ScenarioModeConfig))]
    [JsonSerializable(typeof(AircraftType))]
    public partial class ConfigSerialization : JsonSerializerContext
    {
    }
}
