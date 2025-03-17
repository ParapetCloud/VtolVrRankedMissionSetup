using System;
using System.Collections.Generic;
using VtolVrRankedMissionSetup.VTS;
using VtolVrRankedMissionSetup.VTS.UnitSpawners;

namespace VtolVrRankedMissionSetup.VT.Methods
{
    internal class SCCUnitList
    {
        public static int SCC_NumAlive(IEnumerable<IUnitSpawner> unitList) => throw new InvalidOperationException("You can't actually call this method");

        public static int SCC_NumNearWP(IEnumerable<IUnitSpawner> unitList, Waypoint waypoint, double radius) => throw new InvalidOperationException("You can't actually call this method");
    }
}
