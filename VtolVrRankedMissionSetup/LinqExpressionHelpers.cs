using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace VtolVrRankedMissionSetup
{
    public static class LinqExpressionHelpers
    {
        public static object? GetValue(Expression expr, Dictionary<string, object>? values = null)
        {
            if (values != null && expr is ParameterExpression pe && values.ContainsKey(pe.Name!))
                return values![pe.Name!];
            else if (expr is ConstantExpression constantExpression)
                return constantExpression.Value!;
            else if (expr is MemberExpression me)
            {
                if (me.Expression is ConstantExpression cstExpr)
                {
                    return cstExpr.Value
                        !.GetType()
                        !.GetField(me.Member.Name)
                        !.GetValue(cstExpr.Value!);
                }
                else if (me.Expression is MemberExpression mmbr)
                {
                    object obj = GetValue(mmbr)!;

                    return obj.GetType()!.GetField(me.Member.Name)?.GetValue(obj) ??
                        obj.GetType()!.GetProperty(me.Member.Name)!.GetValue(obj);
                }
                else
                {
                    throw new NotSupportedException($"{expr} is not supported");
                }
            }
            else if (expr is BinaryExpression bin && bin.NodeType == ExpressionType.ArrayIndex)
            {
                Array obj = (Array)GetValue(bin.Left)!;
                long ind = (long)Convert.ChangeType(GetValue(bin.Right)!, typeof(long));

                return obj.GetValue(ind);
            }
            else
                throw new NotSupportedException($"{expr} is not supported");
        }
    }
}
