using VtolVrRankedMissionSetup.VT;

namespace VtolVrRankedMissionSetup.VTS.Components
{
    [VTName("methodParameters")]
    public class MethodParameter
    {
        public string Value { get; set; }

        public MethodParameter(string value)
        {
            Value = value;
        }
    }
}
