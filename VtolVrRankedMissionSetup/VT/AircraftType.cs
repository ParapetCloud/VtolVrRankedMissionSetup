using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace VtolVrRankedMissionSetup.VT
{
    public enum AircraftType
    {
        [JsonStringEnumMemberName("F/A-26B")]
        F26,
        [JsonStringEnumMemberName("F-45A")]
        F45,
        [JsonStringEnumMemberName("EF-24G")]
        F24,
        [JsonStringEnumMemberName("T-55")]
        T55,
        [JsonStringEnumMemberName("AV-42C")]
        AV42,
        [JsonStringEnumMemberName("AH-94")]
        AH94,
        [JsonStringEnumMemberName("F-16")]
        F16,
    }
}
