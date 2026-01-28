using Microsoft.UI.Composition;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;
using VtolVrRankedMissionSetup.VT;
using VtolVrRankedMissionSetup.VT.Methods;
using VtolVrRankedMissionSetup.VTS.Components;
using VtolVrRankedMissionSetup.VTS.UnitSpawners;

namespace VtolVrRankedMissionSetup.VTS
{
    public interface IComponent
    {
        [Id]
        public int Id { get; set; }
        public string Type { get; set; }
        public Vector3 UiPos { get; set; }
    }

    public static class Component
    {
        public static IComponent CreateComponents(Expression<Func<bool>> method, out List<IComponent> components)
        {
            components = [];

            IComponent root = CreateComponents(method.Body);

            PopulateComponentList(root, components);

            return root;
        }

        private static void PopulateComponentList(IComponent comp, List<IComponent> components)
        {
            components.Add(comp);

            if (comp is CompositeComponent composite)
            {
                foreach (IComponent child in composite.Children)
                    PopulateComponentList(child, components);
            }
        }

        private static IComponent CreateComponents(Expression exp, Dictionary<string, object>? values = null)
        {
            return exp switch
            {
                BinaryExpression binaryExpression => CreateBinaryComponent(binaryExpression),
                MethodCallExpression mce => CreateMethodCall(mce),
                UnaryExpression unaryExpression => CreateUnaryComponent(unaryExpression, values),
                _ => throw new NotSupportedException($"{exp} is not supported"),
            };
        }

        private static IComponent CreateBinaryComponent(BinaryExpression binaryExpression)
        {
            IComponent comp = binaryExpression.NodeType switch
            {
                ExpressionType.AndAlso => new CompositeComponent("And", CreateComponents(binaryExpression.Left), CreateComponents(binaryExpression.Right)),
                ExpressionType.And => new CompositeComponent("And", CreateComponents(binaryExpression.Left), CreateComponents(binaryExpression.Right)),
                ExpressionType.OrElse => new CompositeComponent("Or", CreateComponents(binaryExpression.Left), CreateComponents(binaryExpression.Right)),
                ExpressionType.Or => new CompositeComponent("Or", CreateComponents(binaryExpression.Left), CreateComponents(binaryExpression.Right)),
                ExpressionType.GreaterThan => CreateComparisonComponent(binaryExpression),
                ExpressionType.GreaterThanOrEqual => CreateComparisonComponent(binaryExpression),
                ExpressionType.LessThan => CreateComparisonComponent(binaryExpression),
                ExpressionType.LessThanOrEqual => CreateComparisonComponent(binaryExpression),
                ExpressionType.Equal => CreateComparisonComponent(binaryExpression),
                ExpressionType.NotEqual => CreateComparisonComponent(binaryExpression),
                _ => throw new NotSupportedException($"{binaryExpression} is not supported"),
            };

            return comp;
        }

        private static IComponent CreateUnaryComponent(UnaryExpression unaryExpression, Dictionary<string, object>? values = null)
        {
            if (unaryExpression.NodeType == ExpressionType.Not && unaryExpression.Operand is MethodCallExpression mce)
            {
                Type methodContainer = mce.Method.DeclaringType!;

                if (methodContainer == typeof(SCCUnitList))
                {
                    return new SCCUnitListComponent(mce, values)
                    {
                        IsNot = true,
                    };
                }

                throw new NotSupportedException($"{methodContainer} is not supported");
            }

            throw new NotSupportedException($"{unaryExpression} is not supported");
        }

        private static IComponent CreateComparisonComponent(BinaryExpression binaryExpression)
        {
            switch (binaryExpression.Left)
            {
                case MethodCallExpression mce:
                    {
                        Type methodContainer = mce.Method.DeclaringType!;

                        if (methodContainer == typeof(SCCUnitList))
                            return new SCCUnitListComponent(binaryExpression);
                        if (methodContainer == typeof(SCCUnitGroup))
                            return new SCCUnitGroupComponent(binaryExpression);
                    }
                    break;
                case MemberExpression memberExpression:
                    {
                        object? value = LinqExpressionHelpers.GetValue(memberExpression);
                        if (value is GlobalValue gv)
                            return new SCCGlobalValueComponent(gv, binaryExpression);
                    }
                    break;
            }

            throw new NotSupportedException($"{binaryExpression} is not supported");
        }

        private static IComponent CreateMethodCall(MethodCallExpression mce)
        {
            Type methodContainer = mce.Method.DeclaringType!;

            if (methodContainer == typeof(SCCUnitList))
                return new SCCUnitListComponent(mce);
            else if (methodContainer.IsAssignableTo(typeof(IUnitSpawner)))
                return new SCCUnitComponent(mce);
            else if (methodContainer.IsAssignableTo(typeof(Enumerable)))
            {
                if (mce.Method.Name == "AnyTrue")
                {
                    IEnumerable<object> list = (IEnumerable<object>)LinqExpressionHelpers.GetValue(mce.Arguments[0])!;

                    Expression lambda = mce.Arguments[1];

                    return CreateComposite(list, lambda, "Or");
                }
                if (mce.Method.Name == "All")
                {
                    IEnumerable<object> list = (IEnumerable<object>)LinqExpressionHelpers.GetValue(mce.Arguments[0])!;

                    Expression lambda = mce.Arguments[1];

                    return CreateComposite(list, lambda, "And");
                }
            }

            throw new NotSupportedException($"{mce} is not supported");
        }

        private static IComponent CreateComposite(IEnumerable<object> objects, Expression lambda, string type)
        {
            Expression body = (Expression)lambda.GetType().GetProperty("Body")!.GetValue(lambda)!;

            ParameterExpression pe = ((IEnumerable<ParameterExpression>)lambda.GetType().GetProperty("Parameters")!.GetValue(lambda)!).First();

            IComponent[] comps = objects.Select(o => {
                Dictionary<string, object> values = new()
                        {
                            { pe.Name ?? "__unused__", o },
                        };

                return CreateComponents(body, values);
            }).ToArray();

            if (comps.Length == 1)
            {
                return comps[0];
            }
            else if (comps.Length == 0)
            {
                throw new Exception("Any/All require at least 1 value");
            }

            return new CompositeComponent(type, comps);
        }
    }
}
