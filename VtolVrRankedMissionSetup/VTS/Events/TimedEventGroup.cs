using System;
using VtolVrRankedMissionSetup.VT;

namespace VtolVrRankedMissionSetup.VTS.Events
{
    public class TimedEventGroup
    {
        public string GroupName { get; set; }
        [Id]
        public int GroupID { get; set; }
        public bool BeginImmediately { get; set; }
        public TimeSpan InitialDelay { get; set; }

        [VTInlineArray]
        public TimedEventInfo[] EventInfos { get; set; }

        [TargetType("Timed_Events")]
        public void Begin() => throw new NotSupportedException("You can't actually call this");

        [TargetType("Timed_Events")]
        public void End() => throw new NotSupportedException("You can't actually call this");
    }
}
