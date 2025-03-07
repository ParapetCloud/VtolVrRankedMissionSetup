using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using VtolVrRankedMissionSetup.VT;
using VtolVrRankedMissionSetup.VTS.Components;
using VtolVrRankedMissionSetup.VTS.UnitSpawners;

namespace VtolVrRankedMissionSetup.VTS
{
    [VTName("COMP")]
    public class SCCUnitListComponent : IComponent
    {
        [Id]
        public int Id { get; set; }

        public string Type { get; set; }

        public Vector3 UiPos { get; set; }

        [IdLink("unitList")]
        public MultiplayerSpawn[] UnitList { get; set; }

        public string MethodName { get; set; }
        public bool IsNot { get; set; }

        [VTInlineArray]
        public MethodParameter[] MethodParameters { get; set; }

        public SCCUnitListComponent()
        {
            Type = "SCCUnitList";
        }
    }
}
