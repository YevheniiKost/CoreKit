using UnityEngine;

using Logger = YeKostenko.CoreKit.Logging.Logger;

namespace YeKostenko.CoreKit.DI
{
    public class MonoBehDependencyInjector : IDependencyInjector
    {
        private readonly Container _container;
        
        public MonoBehDependencyInjector(Container container)
        {
            _container = container;
        }

        public void Inject(object target)
        {
            try
            {
                if (target is MonoBehaviour monoBehaviour)
                    _container.InjectInto(monoBehaviour);
                else
                    Logger.LogWarning(
                        $"Cannot inject dependencies into {target.GetType().Name}. It is not a MonoBehaviour.");
            }
            catch (System.Exception e)
            {
                Logger.LogError($"Dependency injection failed for {target.GetType().Name}: {e.Message}");
            }
        }

        public void InjectIntoHierarchy(object target)
        {
            try
            {
                if (target is MonoBehaviour gameObject)
                    _container.InjectIntoHierarchy(gameObject);
                else
                    Logger.LogWarning(
                        $"Cannot inject dependencies into hierarchy of {target.GetType().Name}. It is not a MonoBehaviour.");
            }
            catch (System.Exception e)
            {
                Logger.LogError($"Dependency injection into hierarchy failed for {target.GetType().Name}: {e.Message}");
            }
        }
    }
}