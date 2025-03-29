using System;
using VtolVrRankedMissionSetup.VT;
using VtolVrRankedMissionSetup.VTS.Events;

namespace VtolVrRankedMissionSetup.VTS
{
    [VTName("SEQUENCE")]
    public class EventSequence
    {
        [Id]
        public int Id { get; set; }
        public string SequenceName { get; set; } = "new sequence";
        public bool StartImmediately { get; set; }
        public bool WhileLoop { get; set; }

        [VTInlineArray()]
        public Event[] Events { get; set; } = [];

        [EventTarget("Begin", "Event_Sequences")]
        public void BeginEvent() => throw new NotSupportedException("You can't actually call this");

        [EventTarget("Stop", "Event_Sequences")]
        public void Stop() => throw new NotSupportedException("You can't actually call this");

        [EventTarget("Restart", "Event_Sequences")]
        public void Restart() => throw new NotSupportedException("You can't actually call this");
    }
}
