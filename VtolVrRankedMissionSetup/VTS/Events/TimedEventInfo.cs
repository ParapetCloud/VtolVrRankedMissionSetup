using System;
using VtolVrRankedMissionSetup.VT;

namespace VtolVrRankedMissionSetup.VTS.Events
{
    public class TimedEventInfo
    {
        public string EventName { get; set; } = string.Empty;
        public TimeSpan Time { get; set; }

        [VTInlineArray]
        public EventTarget[] Targets { get; set; } = [];
    }
}
