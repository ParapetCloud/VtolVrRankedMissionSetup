using System;
using System.Linq;
using System.Linq.Expressions;

namespace VtolVrRankedMissionSetup
{
    public static class LinqExpressionHelpers
    {
        public static object? GetValue(Expression expr)
        {
            if (expr is ConstantExpression constantExpression)
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
