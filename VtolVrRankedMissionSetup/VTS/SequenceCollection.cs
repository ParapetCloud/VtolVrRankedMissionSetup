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
        public EventSequence[] Sequences => SequenceList.ToArray();

        [VTIgnore]
        private List<EventSequence> SequenceList { get; }

        public SequenceCollection()
        {
            SequenceList = [];
        }

        public EventSequence CreateSequence(string name, bool startsImmediately = true)
        {
            EventSequence sequence = new()
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
