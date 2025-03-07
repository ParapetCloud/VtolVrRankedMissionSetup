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
        public Sequence[] Sequences => SequenceList.ToArray();

        [VTIgnore]
        private List<Sequence> SequenceList { get; }

        public SequenceCollection()
        {
            SequenceList = [];
        }

        public Sequence CreateSequence(string name, bool startsImmediately = true)
        {
            Sequence sequence = new()
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
