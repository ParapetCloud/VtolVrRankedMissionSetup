using VtolVrRankedMissionSetup.VT;

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
}
