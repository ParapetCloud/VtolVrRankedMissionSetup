using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VtolVrRankedMissionSetup.VT;
using VtolVrRankedMissionSetup.VTS.Events;

namespace VtolVrRankedMissionSetup.VTS.Objectives
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
        public ObjectiveEvent StartEvent { get; set; } = new("Start Event");
        [VTName("failEvent")]
        public ObjectiveEvent FailEvent { get; set; } = new("Failed Event");
        [VTName("completeEvent")]
        public ObjectiveEvent CompleteEvent { get; set; } = new("Completed Event");
        [VTName("fields")]
        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        public IObjectiveFields? Fields { get; set; }

        [EventTarget("Begin Objective", "Objective")]
        public void BeginObjective() => throw new NotSupportedException("You can't actually call this");
        [EventTarget("Complete Objective", "Objective")]
        public void CompleteObjective() => throw new NotSupportedException("You can't actually call this");
        [EventTarget("Fail Objective", "Objective")]
        public void FailObjective() => throw new NotSupportedException("You can't actually call this");
        [EventTarget("Cancel Objective", "Objective")]
        public void CancelObjective() => throw new NotSupportedException("You can't actually call this");
        [EventTarget("Reset Objective", "Objective")]
        public void ResetObjective() => throw new NotSupportedException("You can't actually call this");
    }
}
