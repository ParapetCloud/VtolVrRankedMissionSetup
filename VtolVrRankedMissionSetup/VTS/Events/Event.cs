using System;
using VtolVrRankedMissionSetup.VT;

namespace VtolVrRankedMissionSetup.VTS.Events
{
    [VTName("EVENT")]
    public class Event
    {
        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        [IdLink("conditional")]
        public Conditional? Conditional { get; set; }
        public TimeSpan Delay { get; set; }
        public string NodeName { get; set; }
        public EventInfo EventInfo { get; set; }

        public Event(string name, TimeSpan delay, Conditional? entryCondition = null, EventTarget[]? eventTargets = null)
        {
            NodeName = name;
            Delay = delay;
            Conditional = entryCondition;

            EventInfo = new EventInfo()
            {
                EventTargets = eventTargets,
            };
        }
    }
}
