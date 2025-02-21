using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using VtolVrRankedMissionSetup.VT;
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
        public double SpawnChance { get; }
        public Vector3 LastValidPlacement { get; }
        public string EditorPlacementMode { get; }
        public string SpawnFlags { get; }

        public IUnitFields? UnitFields { get; }
    }
}
