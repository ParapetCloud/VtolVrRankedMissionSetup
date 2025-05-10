using System;
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
        [Id]
        public int UnitInstanceID { get; }
        public string UnitID { get; }
        public Vector3 Rotation { get; }
        public Vector3 LastValidPlacement { get; }
        public string EditorPlacementMode { get; }

        public IUnitFields? UnitFields { get; }

        /////////////////////////
        // Event Targets
        /////////////////////////
        
        [EventTarget("Set Lives", "Unit", AltTargetIdx = -2)]
        public void SetLives([ParamAttrInfo("MinMax", "(0,100)")][ParamAttrInfo("UnitSpawnAttributeRange+RangeTypes", "Int")]float lives);

        [EventTarget("Destroy Vehicle", "Unit", AltTargetIdx = -2)]
        public void DestroyVehicle();

        /////////////////////////
        // Conditionals
        /////////////////////////

        public bool SCC_NearWaypoint(Waypoint waypoint, double distance);
        public bool SCC_NearWaypoint(IUnitSpawner unit, double distance);
        public bool SCC_IsUsingAltNumber(int altIndex);
    }
}
