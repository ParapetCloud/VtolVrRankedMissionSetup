using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VtolVrRankedMissionSetup.VT;

namespace VtolVrRankedMissionSetup.VTS
{
    public class ObjectiveEventInfo
    {
        public string EventName { get; set; }

        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        public ObjectiveEventTarget EventTarget { get; set; }
    }
}
