using System;
using VtolVrRankedMissionSetup.VT;
using VtolVrRankedMissionSetup.VT.Methods;

namespace VtolVrRankedMissionSetup.VTS
{
    public class ConditionalAction
    {
        [Id]
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        [VTName("BASE_BLOCK")]
        public required Block BaseBlock { get; set; }
    }

    public static class ConditionalActionExtension
    {

        [EventTarget("Fire Conditional Action", "System")]
        public static void FireConditionalAction([ParamInfo(CustomTypeName = "ConditionalActionReference")][IdLink("")] this ConditionalAction Action) => throw new NotSupportedException("You can't actually call this");
    }
}
