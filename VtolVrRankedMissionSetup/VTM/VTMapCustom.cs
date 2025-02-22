using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VtolVrRankedMissionSetup.VTM
{
    public class VTMapCustom
    {
        public string MapID { get; set; }
        public string MapName { get; set; }
        public string MapDescription { get; set; }
        public string MapType { get; set; }
        public string EdgeMode { get; set; }
        public string CoastSide { get; set; }
        public string Biome { get; set; }
        public string Seed { get; set; }
        public double MapSize { get; set; }

        public StaticPrefab[] StaticPrefabs { get; set; }
    }
}
