using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VtolVrRankedMissionSetup.VT;

namespace VtolVrRankedMissionSetup.Configs.ScenarioMode
{
    public class ScenarioModeConfig
    {
        public string ScenarioCreationService { get; set; } = string.Empty;
        public string PrimaryDefaultLayout { get; set; } = string.Empty;
        public string? SecondaryDefaultLayout { get; set; }
        public Dictionary<AircraftType, string> DefaultEquipment { get; set; } = [];
        public Dictionary<AircraftType, string>? ForcedEquipment { get; set; }
        public string[]? WeatherPresets { get; set; }
    }
}
