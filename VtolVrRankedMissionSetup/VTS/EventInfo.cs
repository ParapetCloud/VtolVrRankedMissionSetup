using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VtolVrRankedMissionSetup.VT;

namespace VtolVrRankedMissionSetup.VTS
{
    public class EventInfo
    {
        public string EventName { get; set; }

        [VTInlineArray]
        public EventTarget[] EventTargets { get; set; }
    }
}
