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

        public Sequence CreateSequence()
        {
            Sequence sequence = new()
            {
                Id = SequenceList.Count
            };
            SequenceList.Add(sequence);

            return sequence;
        }
    }
}
