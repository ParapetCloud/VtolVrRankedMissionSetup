using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VtolVrRankedMissionSetup.VT;

namespace VtolVrRankedMissionSetup.VTS.Components
{
    [VTName("methodParameters")]
    public class MethodParameter
    {
        public string Value { get; set; }

        public MethodParameter(string value)
        {
            Value = value;
        }
    }
}
