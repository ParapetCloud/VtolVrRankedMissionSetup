using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using VtolVrRankedMissionSetup.VT;

namespace VtolVrRankedMissionSetup.VTS
{
    [VTName("WAYPOINT")]
    public class Waypoint
    {
        [Id]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Vector3 GlobalPoint { get; set; }
    }
}
