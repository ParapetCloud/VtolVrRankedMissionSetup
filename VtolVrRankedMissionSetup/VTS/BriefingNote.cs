using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VtolVrRankedMissionSetup.VT;

namespace VtolVrRankedMissionSetup.VTS
{
    [VTName("BRIEFING_NOTE")]
    public class BriefingNote
    {
        public string Text { get; set; }
        public string ImagePath { get; set; }
        public string AudioClipPath { get; set; }
    }
}
