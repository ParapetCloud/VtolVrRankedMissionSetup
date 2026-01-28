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
        [VTIgnore(VTIgnoreCondition.WhenWritingNull)]
        [VTName("ALLIED")]
        public UnitGroup? Allied { get; set; }

        [VTIgnore(VTIgnoreCondition.WhenWritingNull)]
        [VTName("ENEMY")]
        public UnitGroup? Enemy { get; set; }
    }
}
