﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using VtolVrRankedMissionSetup.VT;
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
        public double SpawnChance { get; set; } = 100;
        public Vector3 LastValidPlacement { get => GlobalPosition; }
        public string EditorPlacementMode { get; set; } = "Ground";
        public string SpawnFlags { get; set; } = string.Empty;

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
    }
}
