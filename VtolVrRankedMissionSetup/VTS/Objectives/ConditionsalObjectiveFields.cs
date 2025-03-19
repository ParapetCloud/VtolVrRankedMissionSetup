using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VtolVrRankedMissionSetup.VT;

namespace VtolVrRankedMissionSetup.VTS.Objectives
{
    public class ConditionalObjectiveFields : IObjectiveFields
    {
        [IdLink("successConditional")]
        public Conditional? SuccessConditional { get; set; }
        [IdLink("failConditional")]
        public Conditional? FailConditional { get; set; }
    }
}
