using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        public IUnitSpawner[] UnitList { get; set; }

        public string MethodName { get; set; }
        public bool IsNot { get; set; }

        [VTInlineArray]
        public MethodParameter[] MethodParameters { get; set; }

        public SCCUnitListComponent()
        {
            Type = "SCCUnitList";
        }

        public SCCUnitListComponent(BinaryExpression binaryExpression)
        {
            Type = "SCCUnitList";

            MethodCallExpression mce = (MethodCallExpression)binaryExpression.Left;

            MethodName = mce.Method.Name!;

            if (MethodName == "SCC_NumAlive")
            {
                if (mce.Arguments[0] is ConstantExpression constantExpression)
                    UnitList = (IUnitSpawner[])constantExpression.Value!;
                else if (mce.Arguments[0] is MemberExpression me)
                    UnitList = ((IEnumerable<IUnitSpawner>)((ConstantExpression)me.Expression!).Value
                        !.GetType()
                        !.GetField(me.Member.Name)
                        !.GetValue(((ConstantExpression)me.Expression!).Value!)!).ToArray();
                else
                    throw new NotSupportedException($"{binaryExpression} is not supported");

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
