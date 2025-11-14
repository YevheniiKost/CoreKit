using System;
using System.Linq;
using System.Reflection;

namespace YeKostenko.CoreKit.DI
{
    internal static class ContainerExtensions
    {
        public static void RegisterBinding<TInterface, TObject>(this Container container, Type implementationType,
            LifeTime lifeTime)
        {
            container.Bind<TInterface>();
            var method = container
                .GetType()
                .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(m => m.Name == nameof(Container.RegisterBinding))
                .Where(m => m.IsGenericMethodDefinition)
                .SingleOrDefault(m => m.GetGenericArguments().Length == 2);
            
            if (method == null) throw new InvalidOperationException("RegisterBinding method not found.");

            method.MakeGenericMethod(typeof(TInterface), implementationType)
                .Invoke(container, new object[] { lifeTime });
        }
    }
}