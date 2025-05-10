using System;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;
using VtolVrRankedMissionSetup.VT;
using VtolVrRankedMissionSetup.VTS.Components;
using VtolVrRankedMissionSetup.VTS.UnitSpawners;

namespace VtolVrRankedMissionSetup.VTS
{
    [VTName("COMP")]
    public class SCCUnitComponent : IComponent
    {
        [Id]
        public int Id { get; set; }

        public string Type { get; set; }

        public Vector3 UiPos { get; set; }

        [IdLink("unit")]
        public IUnitSpawner Unit { get; set; }

        public string MethodName { get; set; }
        public bool IsNot { get; set; }

        [VTInlineArray]
        public MethodParameter[] MethodParameters { get; set; }

        public SCCUnitComponent(MethodCallExpression mce)
        {
            Type = "SCCUnit";

            MethodName = mce.Method.Name;

            Unit = (IUnitSpawner)LinqExpressionHelpers.GetValue(mce.Object!)!;

            MethodInfo? translateMethod = typeof(SCCUnitComponent).GetMethod(MethodName, mce.Method.GetParameters().Select(pi => pi.ParameterType).ToArray());

            if (translateMethod == null)
                throw new InvalidOperationException($"Could not find parameter translation method {MethodName}");

            MethodParameters = ((string[])translateMethod.Invoke(null, mce.Arguments.Select(arg => LinqExpressionHelpers.GetValue(arg)).ToArray())!)
                .Select(s => new MethodParameter(s))
                .ToArray();
        }

        public static string[] SCC_NearWaypoint(Waypoint waypoint, double distance) => [
            waypoint.Id.ToString(),
            distance.ToString(),
        ];

        public static string[] SCC_NearWaypoint(IUnitSpawner unit, double distance) => [
            $"unit:{unit.UnitInstanceID}",
            distance.ToString(),
        ];

        public static string[] SCC_IsUsingAltNumber(int altIndex) => [(altIndex + 1).ToString()];
    }
}
