using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VtolVrRankedMissionSetup.VT;

namespace VtolVrRankedMissionSetup.VTS
{
    [VTName("SEQUENCE")]
    public class Sequence
    {
        [Id]
        public int Id { get; set; }
        public string SequenceName { get; set; }
        public bool StartImmediately { get; set; }
        public bool WhileLoop { get; set; }

        [VTInlineArray()]
        public Event[] Events { get; set; }
    }
}
