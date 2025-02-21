using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VtolVrRankedMissionSetup.VT;

namespace VtolVrRankedMissionSetup.VTS
{
    public class UnitGroups
    {
        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        [VTName("ALLIED")]
        public UnitGroup? Allied { get; set; }

        [VTIgnore(Condition = VTIgnoreCondition.WhenWritingNull)]
        [VTName("ENEMY")]
        public UnitGroup? Enemy { get; set; }
    }
}
