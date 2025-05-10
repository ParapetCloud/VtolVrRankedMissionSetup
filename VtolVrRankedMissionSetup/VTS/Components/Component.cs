using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Numerics;
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

        private static IComponent CreateComponents(Expression exp)
        {
            return exp switch
            {
                BinaryExpression binaryExpression => CreateBinaryComponent(binaryExpression),
                MethodCallExpression mce => CreateMethodCall(mce),
                UnaryExpression unaryExpression => CreateUnaryComponent(unaryExpression),
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

        private static IComponent CreateUnaryComponent(UnaryExpression unaryExpression)
        {
            if (unaryExpression.NodeType == ExpressionType.Not && unaryExpression.Operand is MethodCallExpression mce)
            {
                Type methodContainer = mce.Method.DeclaringType!;

                if (methodContainer == typeof(SCCUnitList))
                {
                    return new SCCUnitListComponent(mce)
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

            throw new NotSupportedException($"{mce} is not supported");
        }
    }
}
