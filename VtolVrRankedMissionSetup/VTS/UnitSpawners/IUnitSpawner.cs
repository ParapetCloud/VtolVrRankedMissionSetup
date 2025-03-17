using System.Numerics;
using VtolVrRankedMissionSetup.VTS.UnitFields;

namespace VtolVrRankedMissionSetup.VTS.UnitSpawners
{
    public interface IUnitSpawner
    {
        public string UnitName { get; }
        public Vector3 GlobalPosition { get; }
        public int UnitInstanceID { get; }
        public string UnitID { get; }
        public Vector3 Rotation { get; }
        public Vector3 LastValidPlacement { get; }
        public string EditorPlacementMode { get; }

        public IUnitFields? UnitFields { get; }
    }
}
