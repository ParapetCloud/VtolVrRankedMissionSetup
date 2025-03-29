using System.Numerics;
using VtolVrRankedMissionSetup.VT;

namespace VtolVrRankedMissionSetup.VTS
{
    [VTName("CONDITIONAL")]
    public class Conditional
    {
        [Id]
        public int Id { get; set; }
        public Vector3 OutputNodePos { get; set; }
        [IdLink("root")]
        public required IComponent rootComponent { get; set; }

        [VTInlineArray]
        public IComponent[] Components { get; set; } = [];
    }
}
