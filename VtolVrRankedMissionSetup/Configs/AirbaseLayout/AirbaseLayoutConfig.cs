using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VtolVrRankedMissionSetup.Configs.AirbaseLayout
{
    public class AirbaseLayoutConfig
    {
        public AircraftConfig[]? F26 { get; set; }
        public AircraftConfig[]? F45 { get; set; }
        public AircraftConfig[]? F24 { get; set; }
        public AircraftConfig[]? T55 { get; set; }
        public AircraftConfig[]? F16 { get; set; }

        public WaypointConfig Waypoints { get; set; } = null!;
    }
}
