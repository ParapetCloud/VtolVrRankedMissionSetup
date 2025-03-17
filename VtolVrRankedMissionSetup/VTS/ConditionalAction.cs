using VtolVrRankedMissionSetup.VT;

namespace VtolVrRankedMissionSetup.VTS
{
    public class ConditionalAction
    {
        [Id]
        public int Id { get; set; }

        public string Name { get; set; }

        [VTName("BASE_BLOCK")]
        public Block BaseBlock { get; set; }
    }
}
