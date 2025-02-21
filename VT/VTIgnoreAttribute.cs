using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VtolVrRankedMissionSetup.VT
{
    public enum VTIgnoreCondition
    {
        Always,
        WhenWritingDefault,
        WhenWritingNull,
    }

    public class VTIgnoreAttribute : Attribute
    {
        public VTIgnoreCondition Condition { get; set; }
    }
}
