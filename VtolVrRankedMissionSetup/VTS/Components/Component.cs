using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using VtolVrRankedMissionSetup.VT;

namespace VtolVrRankedMissionSetup.VTS
{
    public interface IComponent
    {
        [Id]
        public int Id { get; set; }
        public string Type { get; set; }
        public Vector3 UiPos { get; set; }
    }
}
