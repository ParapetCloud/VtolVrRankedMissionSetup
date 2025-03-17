using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VtolVrRankedMissionSetup.VT;

namespace VtolVrRankedMissionSetup.VTS
{
    [VTName("gv")]
    public class GlobalValue
    {
        public string Data => $"{Id};{Name};{Description};{InitialValue};";

        [VTIgnore]
        [Id]
        public int Id { get; set; }

        [VTIgnore]
        public string Name { get; set; } = string.Empty;

        [VTIgnore]
        public string Description { get; set; } = string.Empty;

        [VTIgnore]
        public double InitialValue { get; set; }
    }
}
