using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VtolVrRankedMissionSetup.VT;

namespace VtolVrRankedMissionSetup.VTS
{
    public class ParamInfo
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public string Name { get; set; }

        [VTInlineArray]
        public ParamAttrInfo[] Attrs { get; set; }
    }
}
