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
        public bool mpSelectEnabled { get; set; } = true;

        public IUnitFields? UnitFields { get => MultiplayerSpawnFields; }

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
        public void SetLives([ParamAttrInfo("MinMax", "(0,100)")][ParamAttrInfo("UnitSpawnAttributeRange+RangeTypes", "Int")] float Lives) => throw new NotSupportedException("You can't actually call this");
    }
}
