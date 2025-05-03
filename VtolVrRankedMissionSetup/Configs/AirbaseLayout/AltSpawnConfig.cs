using System.Numerics;
using VtolVrRankedMissionSetup.VT;

namespace VtolVrRankedMissionSetup.Configs.AirbaseLayout
{
    public class AltSpawnConfig
    {
        public AircraftType Type { get; set; }
        public int? Slots { get; set; }
        public double? Rotation { get; set; }
        public Vector3? AltPosition { get; set; }
    }
}
