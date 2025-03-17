
namespace VtolVrRankedMissionSetup.VT.Methods
{
    using global::System;
    using VtolVrRankedMissionSetup.VTS;

    public static class System
    {
        public static void DisplayMessage(
            [ParamAttrInfo("TextInputModes", "MultiLine")][ParamAttrInfo("System.Int32", "140")] string Text,
            [ParamAttrInfo("MinMax", "(1,9999)")] float Duration)
            => throw new NotSupportedException("You can't actually call this");

        public static void FireConditionalAction([ParamInfo(CustomTypeName = "ConditionalActionReference")][IdLink("")]ConditionalAction Action) => throw new NotSupportedException("You can't actually call this");
        public static void IncrementValue(
            [ParamInfo(CustomParameterName = "Global Value")][IdLink("")]GlobalValue GlobalValue,
            [ParamAttrInfo("UnitSpawnAttributeRange+RangeTypes", "Int")]
            [ParamAttrInfo("MinMax", "(0,99999)")] float Add)
            => throw new NotSupportedException("You can't actually call this");
    }
}
