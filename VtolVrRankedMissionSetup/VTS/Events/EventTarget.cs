﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using VtolVrRankedMissionSetup.VT;
using VtolVrRankedMissionSetup.VT.Methods;

namespace VtolVrRankedMissionSetup.VTS.Events
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

        public EventTarget(Expression<Action> method)
        {
            if (method.Body is not MethodCallExpression callExpression)
            {
                throw new ArgumentException($"{nameof(method)} must be a method call expression");
            }

            MethodName = callExpression.Method.Name;

            EventTargetAttribute ttAttr = callExpression.Method.GetCustomAttribute<EventTargetAttribute>() ?? throw new NullReferenceException("Event Target Attribute not set");

            EventName = ttAttr.EventName;
            TargetType = ttAttr.TargetTypeName;
            TargetID = ttAttr.TargetId;
            AltTargetIdx = ttAttr.AltTargetIdx;


            if (!callExpression.Method.IsStatic)
            {
                PropertyInfo? idProperty = callExpression.Method.DeclaringType!.GetProperties().SingleOrDefault(p => p.GetCustomAttribute<IdAttribute>() != null);

                if (idProperty != null)
                {
                    object self = LinqExpressionHelpers.GetValue(callExpression.Object!)!;
                    TargetID = (int)idProperty.GetValue(self)!;
                }
            }

            List<ParamInfo> parms = [];

            ParameterInfo[] callParams = callExpression.Method.GetParameters();
            for (int i = 0; i < callExpression.Arguments.Count; ++i)
            {
                ParameterInfo param = callParams[i];
                var arg = callExpression.Arguments[i];

                object? val = LinqExpressionHelpers.GetValue(arg);
                if (param.Name == "targetID")
                {
                    TargetID = (int)val!;
                    continue;
                }

                IdLinkAttribute? idLinkAttr = param.GetCustomAttribute<IdLinkAttribute>();
                if (idLinkAttr != null)
                {
                    PropertyInfo idProp = val!.GetType().GetProperties().Single(p => p.GetCustomAttribute<IdAttribute>() != null);
                    val = idProp.GetValue(val);
                }

                IEnumerable<ParamAttrInfoAttribute> attrs = param.GetCustomAttributes<ParamAttrInfoAttribute>();
                ParamInfoAttribute? infoAttribute = param.GetCustomAttribute<ParamInfoAttribute>();

                string typeName = 
                    infoAttribute?.CustomTypeName ??
                    ((infoAttribute?.UseFullName ?? true) ? param.ParameterType.FullName! : param.ParameterType.Name);

                string parameterName = infoAttribute?.CustomParameterName ?? param.Name!;

                parms.Add(new ParamInfo()
                {
                    Name = parameterName,
                    Type = typeName,
                    Value = val?.ToString() ?? "null",
                    Attrs = attrs.Select(a => new ParamAttrInfo() { Data = a.Data, Type = a.Type }).ToArray(),
                });
            }

            Params = parms.ToArray();

            _ = method.ReturnType;
        }
    }
}
