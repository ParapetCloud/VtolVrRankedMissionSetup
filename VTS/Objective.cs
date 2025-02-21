using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VtolVrRankedMissionSetup.VT;

namespace VtolVrRankedMissionSetup.VTS
{
    public class Objective
    {
        public string ObjectiveName { get; set; }
        public string? ObjectiveInfo { get; set; }
        [Id]
        public int ObjectiveID { get; set; }
        public int OrderID { get; set; }
        public bool Required { get; set; }
        public double CompletionReward { get; set; }
        [IdLink("waypoint")]
        public Waypoint? Waypoint { get; set; }
        public bool AutoSetWaypoint { get; set; }
        public ObjectiveStartMode StartMode { get; set; }
        public ObjectiveType ObjectiveType { get; set; }
        [VTName("startEvent")]
        public ObjectiveEvent StartEvent { get; set; } = new() { EventInfo = new() { EventName = "Start Event" } };
        [VTName("failEvent")]
        public ObjectiveEvent FailEvent { get; set; } = new() { EventInfo = new() { EventName = "Failed Event" } };
        [VTName("completeEvent")]
        public ObjectiveEvent CompleteEvent { get; set; } = new() { EventInfo = new() { EventName = "Completed Event" } };
        [VTName("fields")]
        public ObjectiveFields Fields { get; set; } = new();
    }
}
