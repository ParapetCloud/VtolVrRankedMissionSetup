using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VtolVrRankedMissionSetup.VT;
using VtolVrRankedMissionSetup.VTS.Events;

namespace VtolVrRankedMissionSetup.VTS
{
    [VTName("ELSE_IF")]
    public class Block
    {
        public string BlockName { get; set; } = string.Empty;

        [Id]
        public int BlockId { get; set; }

        [VTName("CONDITIONAL")]
        public required Conditional Conditional { get; set; }

        [VTName("ACTIONS")]
        public required EventInfo Actions { get; set; }

        [VTInlineArray]
        public Block[]? ElseIfs { get; set; }

        [VTName("ELSE_ACTIONS")]
        [VTIgnore(VTIgnoreCondition.WhenWritingNull)]
        public EventInfo? ElseActions { get; set; }
    }
}
