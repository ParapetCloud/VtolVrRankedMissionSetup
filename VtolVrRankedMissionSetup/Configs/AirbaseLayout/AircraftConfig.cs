using System.Numerics;

namespace VtolVrRankedMissionSetup.Configs.AirbaseLayout
{
    public class AircraftConfig
    {
        public AltSpawnConfig[] Spawns { get; set; } = null!;
        public Vector3 Location { get; set; }
        public Vector3 Rotation { get; set; }
    }
}
