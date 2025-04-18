﻿using System;
using System.Linq.Expressions;
using System.Numerics;
using VtolVrRankedMissionSetup.VT;
using VtolVrRankedMissionSetup.VT.Methods;
using VtolVrRankedMissionSetup.VTS.Components;

namespace VtolVrRankedMissionSetup.VTS
{
    [VTName("COMP")]
    public class SCCUnitGroupComponent : IComponent
    {
        [Id]
        public int Id { get; set; }

        public string Type { get; set; }

        public Vector3 UiPos { get; set; }

        public string UnitGroup { get; set; }

        public string MethodName { get; set; }
        public bool IsNot { get; set; }

        [VTInlineArray]
        public MethodParameter[] MethodParameters { get; set; }

        public SCCUnitGroupComponent(BinaryExpression binaryExpression)
        {
            Type = "SCCUnitGroup";

            MethodCallExpression mce = (MethodCallExpression)binaryExpression.Left;

            MethodName = mce.Method.Name!;

            if (MethodName == nameof(SCCUnitGroup.NumAirborne))
            {
                UnitGroup = (string)LinqExpressionHelpers.GetValue(mce.Arguments[0])!;

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
            else
            {
                throw new NotSupportedException($"{binaryExpression} is not supported");
            }
        }
    }
}
