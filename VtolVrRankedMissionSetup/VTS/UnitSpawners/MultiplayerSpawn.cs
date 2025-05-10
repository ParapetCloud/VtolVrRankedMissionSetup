using System;
using System.Numerics;
using VtolVrRankedMissionSetup.VT;
using VtolVrRankedMissionSetup.VT.Methods;
using VtolVrRankedMissionSetup.VTS.UnitFields;

namespace VtolVrRankedMissionSetup.VTS.UnitSpawners
{
    [VTName("UnitSpawner")]
    public class MultiplayerSpawn : IUnitSpawner
    {
        public string UnitName { get; set; }
        public Vector3 GlobalPosition { get; set; }
        [Id]
        public int UnitInstanceID { get; set; }
        public string UnitID { get; } = "MultiplayerSpawn";
        public Vector3 Rotation { get; set; }
        public Vector3 LastValidPlacement { get => GlobalPosition; }
        public string EditorPlacementMode { get; set; } = "Ground";
        public bool OnCarrier { get; set; }
        public bool MpSelectEnabled { get; set; } = true;

        public IUnitFields? UnitFields { get => MultiplayerSpawnFields; }

        [VTInlineArray]
        public AltSpawn[] AltSpawns { get; set; } = [];

        [VTIgnore]
        public MultiplayerSpawnFields MultiplayerSpawnFields { get; }

        public MultiplayerSpawn(Team team, string? unitName = null)
        {
            UnitName = unitName ?? "MP Spawn";
            MultiplayerSpawnFields = new MultiplayerSpawnFields();

            if (team == Team.Enemy)
                UnitID += "Enemy";
        }

        [EventTarget("Set Lives", "Unit", AltTargetIdx = -2)]
        public void SetLives([ParamAttrInfo("MinMax", "(0,100)")][ParamAttrInfo("UnitSpawnAttributeRange+RangeTypes", "Int")] float Lives) => throw new InvalidOperationException("You can't actually call this method");

        [EventTarget("Destroy Vehicle", "Unit", AltTargetIdx = -2)]
        public void DestroyVehicle() => throw new InvalidOperationException("You can't actually call this method");

        public bool SCC_NearWaypoint(Waypoint waypoint, double distance) => throw new InvalidOperationException("You can't actually call this method");
        public bool SCC_NearWaypoint(IUnitSpawner unit, double distance) => throw new InvalidOperationException("You can't actually call this method");
        public bool SCC_IsUsingAltNumber(int altIndex) => throw new InvalidOperationException("You can't actually call this method");
    }
}
