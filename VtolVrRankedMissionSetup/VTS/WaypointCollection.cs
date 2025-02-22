using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VtolVrRankedMissionSetup.VT;

namespace VtolVrRankedMissionSetup.VTS
{
    public class WaypointCollection
    {
        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        [IdLink("bullseyeID")]
        public Waypoint? Bullseye { get; set; }

        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        [IdLink("bullseyeID_B")]
        public Waypoint? BullseyeB { get; set; }

        [VTInlineArray]
        public Waypoint[] Waypoints { get; set; }
    }
}
