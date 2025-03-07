using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VtolVrRankedMissionSetup.VT;

namespace VtolVrRankedMissionSetup.VTS
{
    public class ConditionalCollection
    {
        [VTInlineArray]
        public Conditional[] Conditionals => ConditionalList.ToArray();

        [VTIgnore]
        private List<Conditional> ConditionalList { get; }

        public ConditionalCollection()
        {
            ConditionalList = [];
        }

        public Conditional CreateCondition()
        {
            Conditional conditional = new()
            {
                Id = ConditionalList.Count
            };
            ConditionalList.Add(conditional);

            return conditional;
        }
    }
}
