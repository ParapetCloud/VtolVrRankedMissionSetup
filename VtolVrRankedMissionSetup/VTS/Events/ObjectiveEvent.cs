
namespace VtolVrRankedMissionSetup.VTS.Events
{
    public class ObjectiveEvent
    {
        public EventInfo EventInfo { get; set; }

        public ObjectiveEvent(string eventInfoName, EventTarget[]? eventTargets = null)
        {
            EventInfo = new EventInfo()
            {
                EventName = eventInfoName,
                EventTargets = eventTargets,
            };
        }
    }
}
