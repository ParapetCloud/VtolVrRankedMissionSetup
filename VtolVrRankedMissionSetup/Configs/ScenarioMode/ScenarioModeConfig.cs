using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VtolVrRankedMissionSetup.Configs.ScenarioMode
{
    public class ScenarioModeConfig
    {
        public string ScenarioCreationService { get; set; }
        public string PrimaryDefaultLayout { get; set; }
        public string? SecondaryDefaultLayout { get; set; }
        public Dictionary<string, string> DefaultEquipment { get; set; }
        public Dictionary<string, string>? ForcedEquipment { get; set; }
    }
}
