using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using VtolVrRankedMissionSetup.VT;

namespace VtolVrRankedMissionSetup.VTS.Components
{
    [VTName("COMP")]
    internal class CompositeComponent : IComponent
    {
        [Id]
        public int Id { get; set; }

        public string Type { get; set; }

        public Vector3 UiPos { get; set; }

        [IdLink("factors")]
        public IComponent[] Children { get; set; }

        public CompositeComponent(string type, params IEnumerable<IComponent> children)
        {
            Type = $"SCC{type}";

            List<IComponent> components = [];

            foreach (IComponent child in children)
            {
                if (child is CompositeComponent composit && composit.Type == Type)
                {
                    foreach (IComponent c2 in composit.Children)
                        components.Add(c2);
                }
                else
                    components.Add(child);
            }

            Children = components.ToArray();
        }
    }
}
