using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VtolVrRankedMissionSetup.VT;

namespace VtolVrRankedMissionSetup.VTS
{
    public class EventTarget
    {
        public string TargetType { get; set; }
        public int TargetID { get; set; }
        public string EventName { get; set; }
        public string MethodName { get; set; }
        public int AltTargetIdx { get; set; }

        [VTInlineArray]
        public ParamInfo[] Params { get; set; }
    }
}
