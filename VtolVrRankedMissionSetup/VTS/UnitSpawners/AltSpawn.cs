using System.Numerics;
using VtolVrRankedMissionSetup.VT;
using VtolVrRankedMissionSetup.VTS.UnitFields;

namespace VtolVrRankedMissionSetup.VTS.UnitSpawners
{
    [VTName("altSpawn")]
    public class AltSpawn
    {
        [VTName("globalPos")]
        public Vector3 GlobalPosition { get; set; }
        public Vector3 Rotation { get; set; }
        public int Weight { get; set; } = 100;
        [VTName("placementMode")]
        public string EditorPlacementMode { get; set; } = "Ground";
        public bool OnCarrier { get; set; }
        public bool MpSelectEnabled { get; set; } = true;

        [VTName("unitFields")]
        public IUnitFields? UnitFields => MultiplayerSpawnFields;

        [VTIgnore]
        public MultiplayerSpawnFields MultiplayerSpawnFields { get; }

        public AltSpawn()
        {
            MultiplayerSpawnFields = new MultiplayerSpawnFields();
        }
    }
}
