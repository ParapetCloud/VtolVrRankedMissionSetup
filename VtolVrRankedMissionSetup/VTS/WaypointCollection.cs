using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using VtolVrRankedMissionSetup.VT;

namespace VtolVrRankedMissionSetup.VTS
{
    public class WaypointCollection
    {
        [VTIgnore(VTIgnoreCondition.WhenWritingNull)]
        [IdLink("bullseyeID")]
        public Waypoint? Bullseye { get; set; }

        [VTIgnore(VTIgnoreCondition.WhenWritingNull)]
        [IdLink("bullseyeID_B")]
        public Waypoint? BullseyeB { get; set; }

        [VTInlineArray]
        public Waypoint[] Waypoints => WaypointList.ToArray();

        [VTIgnore]
        private List<Waypoint> WaypointList { get; }

        public WaypointCollection()
        {
            WaypointList = [];
        }

        public Waypoint CreateWaypoint(string name, Vector3 globalPoint)
        {
            Waypoint wp = new()
            {
                Id = WaypointList.Count,
                Name = name,
                GlobalPoint = globalPoint,
            };
            WaypointList.Add(wp);

            return wp;
        }
    }
}
