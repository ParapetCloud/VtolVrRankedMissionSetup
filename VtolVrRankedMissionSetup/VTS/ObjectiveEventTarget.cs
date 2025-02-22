using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VtolVrRankedMissionSetup.VT;

namespace VtolVrRankedMissionSetup.VTS
{
    public class ObjectiveEventTarget
    {
        public string TargetType { get; set; } = "System";
        public int TargetID { get; set; } = 1;
        public string EventName { get; set; } = "Display Message";
        public string MethodName { get; set; } = "DisplayMessage";

        [VTInlineArray]
        public ParamInfo[] ParamInfos { get; set; }

        public class ParamInfo
        {
            public string Type { get; set; }
            public string Value { get; set; }
            public string Name { get; set; }

            [VTInlineArray]
            public ParamAttrInfo[] ParamAttrInfos { get; set; }
        }

        public class ParamAttrInfo
        {
            public string Type { get; set; }
            public string Data { get; set; }
        }
    }
}
