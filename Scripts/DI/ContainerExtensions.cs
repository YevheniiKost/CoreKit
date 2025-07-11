using System;
using System.Reflection;

namespace YeKostenko.CoreKit.DI
{
    internal static class ContainerExtensions
    {
        public static void RegisterBinding<TInterface, TObject>(this Container container, Type implementationType,
            LifeTime lifeTime)
        {
            container.Bind<TInterface>();
            var method = container.GetType()
                .GetMethod(nameof(Container.RegisterBinding), BindingFlags.NonPublic | BindingFlags.Instance);
            if (method == null) throw new InvalidOperationException("RegisterBinding method not found.");

            method.MakeGenericMethod(typeof(TInterface), implementationType)
                .Invoke(container, new object[] { lifeTime });
        }
    }
}