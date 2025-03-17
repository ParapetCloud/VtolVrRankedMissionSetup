using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VtolVrRankedMissionSetup.VT
{
    public class TargetTypeAttribute : Attribute
    {
        public string TargetTypeName { get; set; }

        public TargetTypeAttribute(string typeName)
        {
            TargetTypeName = typeName;
        }
    }
}
