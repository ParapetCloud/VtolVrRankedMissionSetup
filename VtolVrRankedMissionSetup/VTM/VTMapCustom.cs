using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VtolVrRankedMissionSetup.VTM
{
    public class VTMapCustom
    {
        public string MapID { get; set; } = string.Empty;
        public string MapName { get; set; } = string.Empty;
        public string MapDescription { get; set; } = string.Empty;
        public string MapType { get; set; } = string.Empty;
        public string EdgeMode { get; set; } = string.Empty;
        public string CoastSide { get; set; } = string.Empty;
        public string Biome { get; set; } = string.Empty;
        public string Seed { get; set; } = string.Empty;
        public double MapSize { get; set; }

        public StaticPrefab[] StaticPrefabs { get; set; } = [];
    }
}
