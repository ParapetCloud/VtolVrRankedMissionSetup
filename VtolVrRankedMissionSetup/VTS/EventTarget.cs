using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using VtolVrRankedMissionSetup.VT;
using VtolVrRankedMissionSetup.VT.Methods;

namespace VtolVrRankedMissionSetup.VTS
{
    public class EventTarget
    {
        public string TargetType { get; set; }
        public int TargetID { get; set; }
        public string EventName { get; set; }
        public string MethodName { get; set; }
        public int AltTargetIdx { get; set; }

        [VTInlineArray]
        public ParamInfo[] Params { get; set; }

        public EventTarget(string name, Expression<Action> method)
        {
            if (method.Body is not MethodCallExpression callExpression)
            {
                throw new ArgumentException($"{nameof(method)} must be a method call expression");
            }

            EventName = name;
            AltTargetIdx = -1;

            MethodName = callExpression.Method.Name;
            TargetType = callExpression.Method.DeclaringType!.Name;

            List<ParamInfo> parms = [];

            ParameterInfo[] callParams = callExpression.Method.GetParameters();
            for (int i = 0; i < callExpression.Arguments.Count; ++i)
            {
                ParameterInfo param = callParams[i];
                var arg = callExpression.Arguments[i];

                if (arg is not ConstantExpression cExpr)
                {
                    throw new InvalidOperationException("arguments must be constants");
                }

                IEnumerable<ParamAttrInfoAttribute> attrs = param.GetCustomAttributes<ParamAttrInfoAttribute>();

                parms.Add(new ParamInfo()
                {
                    Name = param.Name!,
                    Type = param.ParameterType.FullName!,
                    Value = cExpr.Value?.ToString() ?? "null",
                    Attrs = attrs.Select(a => new ParamAttrInfo() { Data = a.Data, Type = a.Type }).ToArray(),
                });
            }

            Params = parms.ToArray();

            _ = method.ReturnType;
        }
    }
}
