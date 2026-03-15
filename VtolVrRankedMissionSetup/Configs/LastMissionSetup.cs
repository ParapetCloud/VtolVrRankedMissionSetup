using System;
using System.Collections.Generic;
using System.Text;
using VtolVrRankedMissionSetup.VTS;

namespace VtolVrRankedMissionSetup.Configs
{
    public class SerializableBaseSetting
    {
        public Team Team { get; set; }
        public int ListOrder { get; set; }
        public required string Layout { get; set; }
    }

    public class BaseMissionSettings
    {
        public required SerializableBaseSetting[] BaseConfigs { get; set; }
    }

    public class LastMissionSetup
    {
        public BaseMissionSettings? FiveFiveOne { get; set; }

        public BaseMissionSettings? HS { get; set; }
    }
}
