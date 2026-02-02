using System;
using VtolVrRankedMissionSetup.VTS;

namespace VtolVrRankedMissionSetup.VT.Methods
{
    public static class GameSystem
    {
        [EventTarget("Display Message", "System", TargetId = 1)]
        public static void DisplayMessage(
            [ParamAttrInfo("TextInputModes", "MultiLine")][ParamAttrInfo("System.Int32", "140")] string Text,
            [ParamAttrInfo("MinMax", "(1,9999)")] float Duration)
            => throw new NotSupportedException("You can't actually call this");
    }
}
