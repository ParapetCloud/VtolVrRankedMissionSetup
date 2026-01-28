using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using VtolVrRankedMissionSetup.VT;
using VtolVrRankedMissionSetup.VT.Methods;
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
        public IUnitSpawner[] UnitList { get; set; }

        public string MethodName { get; set; }
        public bool IsNot { get; set; }

        [VTInlineArray]
        public MethodParameter[] MethodParameters { get; set; }

        public SCCUnitListComponent(BinaryExpression binaryExpression, Dictionary<string, object>? values = null)
        {
            Type = "SCCUnitList";

            MethodCallExpression mce = (MethodCallExpression)binaryExpression.Left;

            MethodName = mce.Method.Name!;

            if (MethodName == "SCC_NumAlive")
            {
                UnitList = ((IEnumerable<IUnitSpawner>)LinqExpressionHelpers.GetValue(mce.Arguments[0], values)!).ToArray();

                MethodParameters = [
                    binaryExpression.NodeType switch
                    {
                        ExpressionType.GreaterThan => new MethodParameter("Greater_Than"),
                        ExpressionType.LessThan => new MethodParameter("Less_Than"),
                        ExpressionType.Equal => new MethodParameter("Equals"),
                        _ => throw new NotSupportedException($"{binaryExpression.NodeType} is not supported"),
                    },
                    new MethodParameter(((ConstantExpression)binaryExpression.Right).Value!.ToString()!)
                ];
            }
            else if (MethodName == nameof(SCCUnitList.SCC_NumNearWP))
            {
                UnitList = ((IEnumerable<IUnitSpawner>)LinqExpressionHelpers.GetValue(mce.Arguments[0])!).ToArray();

                MethodParameters = [
                    new MethodParameter(((Waypoint)LinqExpressionHelpers.GetValue(mce.Arguments[1])!).Id.ToString()),
                    new MethodParameter(LinqExpressionHelpers.GetValue(mce.Arguments[2])!.ToString()!),
                    binaryExpression.NodeType switch
                    {
                        ExpressionType.GreaterThan => new MethodParameter("Greater_Than"),
                        ExpressionType.LessThan => new MethodParameter("Less_Than"),
                        ExpressionType.Equal => new MethodParameter("Equals"),
                        _ => throw new NotSupportedException($"{binaryExpression.NodeType} is not supported"),
                    },
                    new MethodParameter(((ConstantExpression)binaryExpression.Right).Value!.ToString()!)
                ];
            }
            else if (MethodName == nameof(SCCUnitList.SCC_AnyNearWaypoint))
            {
                UnitList = ((IEnumerable<IUnitSpawner>)LinqExpressionHelpers.GetValue(mce.Arguments[0])!).ToArray();

                MethodParameters = [
                    new MethodParameter(((Waypoint)LinqExpressionHelpers.GetValue(mce.Arguments[1])!).Id.ToString()),
                    new MethodParameter(LinqExpressionHelpers.GetValue(mce.Arguments[2])!.ToString()!),
                ];
            }
            else
            {
                throw new NotSupportedException($"{binaryExpression} is not supported");
            }
        }

        public SCCUnitListComponent(MethodCallExpression mce, Dictionary<string, object>? values = null)
        {
            Type = "SCCUnitList";

            MethodName = mce.Method.Name!;

            if (MethodName == nameof(SCCUnitList.SCC_AnyNearWaypoint))
            {
                UnitList = ((IEnumerable<IUnitSpawner>)LinqExpressionHelpers.GetValue(mce.Arguments[0], values)!).ToArray();

                MethodParameters = [
                    new MethodParameter(((Waypoint)LinqExpressionHelpers.GetValue(mce.Arguments[1], values)!).Id.ToString()),
                    new MethodParameter(LinqExpressionHelpers.GetValue(mce.Arguments[2], values)!.ToString()!),
                ];
            }
            else
            {
                throw new NotSupportedException($"{mce} is not supported");
            }
        }
    }
}
