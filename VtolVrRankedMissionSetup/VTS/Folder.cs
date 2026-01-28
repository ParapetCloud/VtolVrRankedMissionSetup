using System;
using System.Collections.Generic;
using System.Text;
using VtolVrRankedMissionSetup.VT;

namespace VtolVrRankedMissionSetup.VTS
{
    [VTName("FOLDER")]
    public class Folder
    {
        [Id]
        public required string Name { get; set; }

        public int SortOrder { get; set; }

        public bool Expanded { get; set; }
    }
}
