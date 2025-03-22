using System.Numerics;
using VtolVrRankedMissionSetup.VT;
using VtolVrRankedMissionSetup.VT.Methods;
using VtolVrRankedMissionSetup.VTS.UnitFields;

namespace VtolVrRankedMissionSetup.VTS.UnitSpawners
{
    public interface IUnitSpawner
    {
        public string UnitName { get; }
        public Vector3 GlobalPosition { get; }
        public int UnitInstanceID { get; }
        [Id]
        public string UnitID { get; }
        public Vector3 Rotation { get; }
        public Vector3 LastValidPlacement { get; }
        public string EditorPlacementMode { get; }

        public IUnitFields? UnitFields { get; }

        [EventTarget("Set Lives", "Unit")]
        public void SetLives([ParamAttrInfo("MinMax", "(0,100)")][ParamAttrInfo("UnitSpawnAttributeRange+RangeTypes", "Int")]float lives);
    }
}
