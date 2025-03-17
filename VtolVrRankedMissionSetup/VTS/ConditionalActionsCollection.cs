using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using VtolVrRankedMissionSetup.VT;
using VtolVrRankedMissionSetup.VTS.Events;

namespace VtolVrRankedMissionSetup.VTS
{
    public class ConditionalActionsCollection
    {
        [VTInlineArray]
        public ConditionalAction[] Conditionals => ConditionalList.ToArray();

        [VTIgnore]
        private List<ConditionalAction> ConditionalList { get; }

        private int BlockCount = 0;

        public ConditionalActionsCollection()
        {
            ConditionalList = [];
        }

        public ConditionalAction CreateConditionalAction(string name, Expression<Func<bool>> condition, EventTarget[] actions, Expression<Func<bool>>[] elseIfConditions, EventTarget[][] elseIfActions, EventTarget[] elseActions)
        {
            IComponent root = Component.CreateComponents(condition, out List<IComponent> localComps);

            if (elseIfActions.Length != elseIfConditions.Length)
            {
                throw new InvalidOperationException("ElseIf Actions/Conditions must be the same length");
            }

            for (int i = 0; i < localComps.Count; ++i)
            {
                localComps[i].Id = i;
            }

            ConditionalAction conditional = new()
            {
                Name = name,
                Id = ConditionalList.Count,
                BaseBlock = new Block()
                {
                    BlockId = ++BlockCount,
                    Conditional = new Conditional()
                    {
                        Id = 0,
                        Components = localComps.ToArray(),
                        rootComponent = root,
                    },
                    Actions = new EventInfo(actions),
                    ElseActions = new EventInfo(elseActions),
                },
            };
            ConditionalList.Add(conditional);

            List<Block> elseIfs = [];
            for (int i = 0; i < elseIfActions.Length; ++i)
            {
                IComponent elseIfRoot = Component.CreateComponents(elseIfConditions[i], out List<IComponent> elseIfComps);

                elseIfs.Add(new Block()
                {
                    BlockId = ++BlockCount,
                    Conditional = new Conditional()
                    {
                        Id = 0,
                        Components = elseIfComps.ToArray(),
                        rootComponent = elseIfRoot,
                    },
                    Actions = new EventInfo(elseIfActions[i]),
                });
            }

            conditional.BaseBlock.ElseIfs = elseIfs.ToArray();

            return conditional;
        }
    }
}
