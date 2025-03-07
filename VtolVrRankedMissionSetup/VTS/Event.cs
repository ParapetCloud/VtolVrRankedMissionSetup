using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VtolVrRankedMissionSetup.VT;

namespace VtolVrRankedMissionSetup.VTS
{
    [VTName("EVENT")]
    public class Event
    {
        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        [IdLink("conditional")]
        public Conditional? Conditional { get; set; }
        public int Delay { get; set; }
        public string NodeName { get; set; }
        public EventInfo EventInfo { get; set; }

        public Event(string name, int delay = 0, Conditional? entryCondition = null, EventTarget[]? eventTargets = null)
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
