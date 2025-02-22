using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VtolVrRankedMissionSetup.VT;

namespace VtolVrRankedMissionSetup.VTS.UnitFields
{
    public class MultiplayerSpawnFields : IUnitFields
    {
        public string Vehicle {get; set; } = "F/A-26B";
		public string SlotLabel {get; set; } = string.Empty;
		public string UnitGroup {get; set; } = string.Empty;
		public StartMode StartMode { get; set; } = StartMode.FlightReady;
		public string Equipment { get; set; } = string.Empty;
		public double InitialSpeed {get; set; } = 0;
		public bool RtbIsSpawn {get; set; } = false;
		public bool LimitedLives {get; set; } = false;
		public uint LifeCount {get; set; } = 1;
		public bool B_eqAssignmentMode {get; set; } = false;
		public double CostToSpawn {get; set; } = 0;
		public string LiveryRef {get; set; } = "0;";
		public bool ReceiveFriendlyDamage {get; set; } = true;

        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingDefault)]
        public int Slots { get; set; }
    }
}
