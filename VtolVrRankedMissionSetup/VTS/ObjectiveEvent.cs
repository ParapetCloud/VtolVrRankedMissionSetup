using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VtolVrRankedMissionSetup.VTS
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
