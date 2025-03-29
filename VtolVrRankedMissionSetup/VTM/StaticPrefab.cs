using System.Numerics;
using VtolVrRankedMissionSetup.VT;

namespace VtolVrRankedMissionSetup.VTM
{
    public class StaticPrefab
    {
        public string Prefab { get; set; } = string.Empty;

        [Id]
        public uint Id { get; set; }

        public Vector3 GlobalPos { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector2 Grid { get; set; }
        public Vector3 TSpacePose { get; set; }
        public string TerrainToLocalMatrix { get; set; } = string.Empty;
        public string? BaseName { get; set; }
    }
}
