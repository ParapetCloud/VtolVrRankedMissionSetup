using VtolVrRankedMissionSetup.VT;

namespace VtolVrRankedMissionSetup.VTS.Events
{
    public class EventInfo
    {
        public string EventName { get; set; }

        [VTInlineArray]
        public EventTarget[]? EventTargets { get; set; }

        public EventInfo()
        {
            EventName = string.Empty;
        }

        public EventInfo(EventTarget[]? eventTargets)
        {
            EventName = string.Empty;
            EventTargets = eventTargets;
        }
    }
}
