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
            if(target is MonoBehaviour monoBehaviour)
                _container.InjectInto(monoBehaviour);
            else
                Logger.LogWarning($"Cannot inject dependencies into {target.GetType().Name}. It is not a MonoBehaviour.");
        }

        public void InjectIntoHierarchy(object target)
        {
            if(target is MonoBehaviour gameObject)
                _container.InjectIntoHierarchy(gameObject);
            else
                Logger.LogWarning($"Cannot inject dependencies into hierarchy of {target.GetType().Name}. It is not a MonoBehaviour.");
        }
    }
}