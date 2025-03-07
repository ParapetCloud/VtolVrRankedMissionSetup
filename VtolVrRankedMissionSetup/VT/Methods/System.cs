using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VtolVrRankedMissionSetup.VT.Methods
{
    public static class System
    {
        public static void DisplayMessage(
            [ParamAttrInfo("TextInputModes", "MultiLine")][ParamAttrInfo("System.Int32", "140")] string Text,
            [ParamAttrInfo("MinMax", "(1,9999)")] float Duration)
        { }
    }
}
