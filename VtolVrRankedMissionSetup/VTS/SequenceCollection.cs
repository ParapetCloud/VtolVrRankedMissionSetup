using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VtolVrRankedMissionSetup.VT;

namespace VtolVrRankedMissionSetup.VTS
{
    public class SequenceCollection
    {
        [VTInlineArray]
        public Event_Sequences[] Sequences => SequenceList.ToArray();

        [VTIgnore]
        private List<Event_Sequences> SequenceList { get; }

        public SequenceCollection()
        {
            SequenceList = [];
        }

        public Event_Sequences CreateSequence(string name, bool startsImmediately = true)
        {
            Event_Sequences sequence = new()
            {
                Id = SequenceList.Count,
                SequenceName = name,
                StartImmediately = startsImmediately,
            };
            SequenceList.Add(sequence);

            return sequence;
        }
    }
}
