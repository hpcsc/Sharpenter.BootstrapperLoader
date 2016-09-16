using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Sharpenter.BootstrapperLoader.Helpers
{
    internal class ExpressionCreator : IAmInstanceCreator
    {
        public virtual object Create(Type type)
        {
            var constructorInfo = FindDefaultConstructorInfo(type);
            
            var constructorExpression = Expression.New(constructorInfo);
            
            var compiledLambda = Expression.Lambda<Func<object>>(constructorExpression).Compile();            

            return compiledLambda();
        }

        protected static ConstructorInfo FindDefaultConstructorInfo(Type type)
        {
            var defaultConstructor = type.GetConstructor(new Type[0]);
            if (defaultConstructor == null) throw new ArgumentException($"Constructor with given signature or default constructor not found in type {type.FullName}");

            return defaultConstructor;
        }
    }

    internal class ExpressionCreator<TArg> : ExpressionCreator
    {
        private readonly TArg _parameter;

        public ExpressionCreator(TArg parameter)
        {
            _parameter = parameter;
        }

        public override object Create(Type type)
        {
            var parameterType = typeof(TArg);
            var constructorInfo = FindConstructorInfo(type);

            var parameterExpressions = Expression.Parameter(parameterType);
            var constructorExpression = Expression.New(constructorInfo, parameterExpressions);

            var compiledLambda = Expression.Lambda<Func<TArg, object>>(constructorExpression, parameterExpressions).Compile();

            return compiledLambda(_parameter);
        }

        private static ConstructorInfo FindConstructorInfo(Type type)
        {
            var constructorInfo = type.GetConstructor(new[] { typeof(TArg) });
            return constructorInfo ?? FindDefaultConstructorInfo(type);
        }
    }
}
