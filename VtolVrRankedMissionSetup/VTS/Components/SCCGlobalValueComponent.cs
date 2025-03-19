using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using VtolVrRankedMissionSetup.VT;
using VtolVrRankedMissionSetup.VT.Methods;
using VtolVrRankedMissionSetup.VTS.Components;
using VtolVrRankedMissionSetup.VTS.UnitSpawners;

namespace VtolVrRankedMissionSetup.VTS
{
    [VTName("COMP")]
    public class SCCGlobalValueComponent : IComponent
    {
        [Id]
        public int Id { get; set; }

        public string Type { get; set; }

        public Vector3 UiPos { get; set; }

        [IdLink("gv")]
        public GlobalValue Value { get; set; }

        public string Comparison { get; set; }

        [VTName("c_value")]
        public string To { get; set; }

        public SCCGlobalValueComponent()
        {
            Type = "SCCGlobalValue";
        }

        public SCCGlobalValueComponent(GlobalValue value, BinaryExpression binaryExpression)
        {
            Type = "SCCGlobalValue";

            Comparison = binaryExpression.NodeType switch
            {
                ExpressionType.GreaterThan => "Greater_Than",
                ExpressionType.LessThan => "Less_Than",
                ExpressionType.Equal => "Equals",
                _ => throw new NotSupportedException($"{binaryExpression.NodeType} is not supported"),
            };

            Value = value;
            To = LinqExpressionHelpers.GetValue(binaryExpression.Right)!.ToString() ?? "0";
        }
    }
}
