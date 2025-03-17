using System;
using VtolVrRankedMissionSetup.VT;
using VtolVrRankedMissionSetup.VTS.Events;

namespace VtolVrRankedMissionSetup.VTS
{
    [VTName("SEQUENCE")]
    public class Event_Sequences
    {
        [Id]
        public int Id { get; set; }
        public string SequenceName { get; set; }
        public bool StartImmediately { get; set; }
        public bool WhileLoop { get; set; }

        [VTInlineArray()]
        public Event[] Events { get; set; }

        public void Begin() => throw new NotSupportedException("You can't actually call this");
        public void Stop() => throw new NotSupportedException("You can't actually call this");
        public void Restart() => throw new NotSupportedException("You can't actually call this");
    }
}
