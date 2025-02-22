using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using VtolVrRankedMissionSetup.VT;

namespace VtolVrRankedMissionSetup.VTM
{
    public class StaticPrefab
    {
        public string Prefab { get; set; }

        [Id]
        public uint Id { get; set; }

        public Vector3 GlobalPos { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector2 Grid { get; set; }
        public Vector3 TSpacePose { get; set; }
        public string TerrainToLocalMatrix { get; set; }
        public string? BaseName { get; set; }
    }
}
