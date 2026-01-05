using System;
using System.Collections.Generic;
using System.Text;
using VtolVrRankedMissionSetup.VT;

namespace VtolVrRankedMissionSetup.VTS
{

    [VTName("PRESET")]
    public class WeatherPreset
    {
        [Id]
        public int Id { get; set; }
        public required string Data { get; set; }
    }
}
