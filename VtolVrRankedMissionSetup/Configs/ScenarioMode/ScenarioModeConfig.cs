using System.Collections.Generic;
using VtolVrRankedMissionSetup.VT;

namespace VtolVrRankedMissionSetup.Configs.ScenarioMode
{
    public class ScenarioModeConfig
    {
        public string ScenarioCreationService { get; set; } = string.Empty;
        public string[] DefaultLayouts { get; set; } = [];
        public string? OtherLayout { get; set; }
        public Dictionary<AircraftType, string> DefaultEquipment { get; set; } = [];
        public Dictionary<AircraftType, string>? ForcedEquipment { get; set; }
        public string[]? WeatherPresets { get; set; }
    }
}
