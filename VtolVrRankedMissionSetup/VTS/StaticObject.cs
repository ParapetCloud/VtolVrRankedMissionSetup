using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

using VtolVrRankedMissionSetup.VT;

namespace VtolVrRankedMissionSetup.VTS
{
    public class StaticObject
    {
        public string PrefabID { get; set; }
        [Id]
        public int Id { get; set; }
        public Vector3 GlobalPos { get; set; }
        public Vector3 Rotation { get; set; }
    }
}
