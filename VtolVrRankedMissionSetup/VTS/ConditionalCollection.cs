using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public Conditional CreateCondition(Expression<Func<bool>> method)
        {
            IComponent root = Component.CreateComponents(method, out List<IComponent> localComps);

            for (int i = 0; i < localComps.Count; ++i)
            {
                localComps[i].Id = i;
            }

            Conditional conditional = new()
            {
                Id = ConditionalList.Count,
                Components = localComps.ToArray(),
                rootComponent = root,
            };
            ConditionalList.Add(conditional);

            return conditional;
        }
    }
}
