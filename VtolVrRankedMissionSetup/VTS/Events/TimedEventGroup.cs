using System;
using VtolVrRankedMissionSetup.VT;

namespace VtolVrRankedMissionSetup.VTS.Events
{
    public class TimedEventGroup
    {
        public string GroupName { get; set; } = "New Timed Event Group";
        [Id]
        public int GroupID { get; set; }
        public bool BeginImmediately { get; set; }
        public TimeSpan InitialDelay { get; set; }

        [VTInlineArray]
        public TimedEventInfo[] EventInfos { get; set; } = [];

        [EventTarget("Begin", "Timed_Events")]
        public void Begin() => throw new NotSupportedException("You can't actually call this");

        [EventTarget("End", "Timed_Events")]
        public void EndEvent() => throw new NotSupportedException("You can't actually call this");
    }
}
