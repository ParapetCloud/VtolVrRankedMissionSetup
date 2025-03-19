using System;
using VtolVrRankedMissionSetup.VTS;

namespace VtolVrRankedMissionSetup.VT.Methods
{
    public static class GameSystem
    {
        [EventTarget("Fire Conditional Action", "System")]
        public static void FireConditionalAction([ParamInfo(CustomTypeName = "ConditionalActionReference")][IdLink("")]ConditionalAction Action) => throw new NotSupportedException("You can't actually call this");

        [EventTarget("Display Message", "System", TargetId = 1)]
        public static void DisplayMessage(
            [ParamAttrInfo("TextInputModes", "MultiLine")][ParamAttrInfo("System.Int32", "140")] string Text,
            [ParamAttrInfo("MinMax", "(1,9999)")] float Duration)
            => throw new NotSupportedException("You can't actually call this");

        [EventTarget("Increment Value", "System", TargetId = 2)]
        public static void IncrementValue(
            [ParamInfo(CustomParameterName = "Global Value")][IdLink("")]GlobalValue GlobalValue,
            [ParamAttrInfo("UnitSpawnAttributeRange+RangeTypes", "Int")]
            [ParamAttrInfo("MinMax", "(0,99999)")] float Add)
            => throw new NotSupportedException("You can't actually call this");

        [EventTarget("Reset Value", "System", TargetId = 2)]
        public static void ResetValue(
            [ParamInfo(CustomParameterName = "Global Value")][IdLink("")] GlobalValue GlobalValue)
            => throw new NotSupportedException("You can't actually call this");

        [EventTarget("Set Value", "System", TargetId = 2)]
        public static void SetValue(
            [ParamInfo(CustomParameterName = "Global Value")][IdLink("")] GlobalValue GlobalValue,
            [ParamAttrInfo("UnitSpawnAttributeRange+RangeTypes", "Int")]
            [ParamAttrInfo("MinMax", "(-99999,99999)")] float Set_to)
            => throw new NotSupportedException("You can't actually call this");
    }
}
