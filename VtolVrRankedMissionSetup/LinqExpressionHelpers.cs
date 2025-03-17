using System;
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
                    return GetValue(mmbr);
                else
                {
                    throw new NotSupportedException($"{expr} is not supported");
                }
            }
            else
                throw new NotSupportedException($"{expr} is not supported");
        }
    }
}
