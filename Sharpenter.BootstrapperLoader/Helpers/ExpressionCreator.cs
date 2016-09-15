using System;
using System.Linq.Expressions;

namespace Sharpenter.BootstrapperLoader.Helpers
{
    internal class ExpressionCreator : IAmInstanceCreator
    {
        public object Create(Type type)
        {
            var defaultConstructor = type.GetConstructor(new Type[0]);
            if(defaultConstructor == null) throw new ArgumentException($"Default constructor not found in type {type.FullName}");

            var constructorExpression = Expression.New(defaultConstructor);
            var compiledLambda = Expression.Lambda<Func<object>>(constructorExpression).Compile();            

            return compiledLambda();
        }
    }
}
