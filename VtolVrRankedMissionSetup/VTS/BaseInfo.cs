using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VtolVrRankedMissionSetup.VT;
using VtolVrRankedMissionSetup.VTM;

namespace VtolVrRankedMissionSetup.VTS
{
    public class BaseInfo
    {
        [IdLink("id")]
        public StaticPrefab Prefab { get; set; }

        public string OverrideBaseName { get; set; } = string.Empty;
        public Team BaseTeam { get; set; } = Team.Allied;
        [VTName("CUSTOM_DATA")]
        public Empty? CustomData { get; set; }

        public BaseInfo(StaticPrefab prefab)
        {
            Prefab = prefab;
        }
    }
}
