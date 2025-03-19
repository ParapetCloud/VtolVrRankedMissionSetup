using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VtolVrRankedMissionSetup.VT;

namespace VtolVrRankedMissionSetup.VTS.Objectives
{
    public class GlobalValueObjectiveFields : IObjectiveFields
    {
        [IdLink("currentValue")]
        public GlobalValue? CurrentValue { get; set; }
        [IdLink("targetValue")]
        public GlobalValue? TargetValue { get; set; }
    }
}
