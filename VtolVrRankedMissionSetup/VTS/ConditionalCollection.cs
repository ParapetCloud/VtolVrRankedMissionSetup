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

        public Conditional CreateCondition(params IComponent[] components)
        {
            for (int i = 0; i < components.Length; ++i)
            {
                components[i].Id = i;
            }

            Conditional conditional = new()
            {
                Id = ConditionalList.Count,
                Components = components,
                rootComponent = components[0],
            };
            ConditionalList.Add(conditional);

            return conditional;
        }
    }
}
