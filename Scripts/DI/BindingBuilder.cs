using System;

namespace YeKostenko.CoreKit.DI
{
    public class BindingBuilder<TInterface>
    {
        private readonly Container _container;
        private Type _implementationType;
        private LifeTime _lifeTime;

        public BindingBuilder(Container container)
        {
            this._container = container;
        }

        public BindingBuilder<TInterface> To<TImplementation>() where TImplementation : TInterface
        {
            _implementationType = typeof(TImplementation);
            return this;
        }

        public BindingBuilder<TInterface> ToFactory(Func<TInterface> factory)
        {
            _container.RegisterFactory(factory);
            return this;
        }

        public void AsSingleton()
        {
            RegisterBinding(LifeTime.Singleton);
        }

        public void AsTransient()
        {
            RegisterBinding(LifeTime.Transient);
        }

        public void ToInstance(TInterface instance)
        {
            _container.RegisterInstance(instance);
        }

        private void RegisterBinding(LifeTime lifeTime)
        {
            if (_implementationType != null)
                _container.RegisterBinding<TInterface, object>(_implementationType, lifeTime);
            else
                _container.RegisterBinding<TInterface, TInterface>(lifeTime);
        }
    }
}