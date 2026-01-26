using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Nkraft.CrossUtility.Extensions;

public static class ExpressionExtension
{
    extension<T>(Expression<T> expression)
    {
        public string GetMemberName()
        {
            return expression.Body switch
            {
                MemberExpression m => m.Member.Name,
                UnaryExpression { Operand: MemberExpression m } => m.Member.Name,
                _ => throw new NotSupportedException("Expression body is not supported.")
            };
        }

        public PropertyInfo GetPropertyInfo()
        {
            if (expression.Body is not MemberExpression body)
                throw new InvalidOperationException($"Expression must be a {nameof(MemberExpression)}.");

            return body.Member is PropertyInfo propInfo
                ? propInfo
                : throw new InvalidOperationException($"Expression must be a property access.");
        }
    }
}
