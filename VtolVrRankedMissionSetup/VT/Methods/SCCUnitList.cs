using System;
using System.Collections.Generic;
using VtolVrRankedMissionSetup.VTS;
using VtolVrRankedMissionSetup.VTS.UnitSpawners;

namespace VtolVrRankedMissionSetup.VT.Methods
{
    /// <summary>
    /// Creates a <see cref="SCCUnitListComponent"/>
    /// </summary>
    internal static class SCCUnitList
    {
        public static int NumAlive(this IEnumerable<IUnitSpawner> unitList) => throw new InvalidOperationException("You can't actually call this method");

        public static int NumNearWP(this IEnumerable<IUnitSpawner> unitList, Waypoint waypoint, double radius) => throw new InvalidOperationException("You can't actually call this method");

        public static bool AnyNearWaypoint(this IEnumerable<IUnitSpawner> unitList, Waypoint waypoint, double radius) => throw new InvalidOperationException("You can't actually call this method");

        public static bool AnyGetsDamagedBy(this IEnumerable<IUnitSpawner> unitList, IEnumerable<IUnitSpawner> damagers) => throw new InvalidOperationException("You can't actually call this method");
    }
}
