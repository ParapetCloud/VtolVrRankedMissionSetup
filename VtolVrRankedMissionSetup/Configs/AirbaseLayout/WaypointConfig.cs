using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace VtolVrRankedMissionSetup.Configs.AirbaseLayout
{
    public class WaypointConfig
    {
        public Vector3 Rtb { get; set; }
        public Vector3 Protection { get; set; }

        public Vector3[] Perimeter { get; set; }
    }
}
