using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using YeKostenko.CoreKit.Logging;

namespace YeKostenko.CoreKit.DI
{
    public class Container : IDisposable
    {
        private class BindingInfo
        {
            public Type ImplementationType;
            public LifeTime LifeTime;
            public object SingletonInstance;
            public object FixedInstance;
        }

        private readonly Dictionary<Type, BindingInfo> _bindings = new();
        private readonly Dictionary<Type, Func<object>> _factories = new();
        private readonly Container _parent;

        private bool _disposed;

        public Container(Container parent = null, string label = null) {
            this._parent = parent;
            this.Label = label ?? $"Container_{GetHashCode()}";

            ContainerRegistry.Register(this);
        }

        public string Label { get; private set; }

        public BindingBuilder<TInterface> Bind<TInterface>()
        {
            return new BindingBuilder<TInterface>(this);
        }

        public T Resolve<T>() => (T)Resolve(typeof(T));

        public object Resolve(Type type)
        {
            if (type == typeof(Container)) return this;

            if (TryResolveLocal(type, out var result))
                return result;

            if (_parent != null)
                return _parent.Resolve(type);

            throw new InvalidOperationException($"No binding registered for type {type.FullName}");
        }

        private bool TryResolveLocal(Type type, out object result)
        {
            result = null;

            if (!_bindings.TryGetValue(type, out BindingInfo binding))
            {
                return false;
            }

            if (_factories.TryGetValue(type, out Func<object> factory))
            {
                result = factory();
                return true;
            }

            if (binding.FixedInstance != null)
            {
                result = binding.FixedInstance;
                return true;
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Lazy<>))
            {
                Type actualType = type.GetGenericArguments()[0];
                result = Activator.CreateInstance(type, new Func<object>(() => Resolve(actualType)));
                return true;
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Func<>))
            {
                Type actualType = type.GetGenericArguments()[0];
                var method = typeof(Container)
                    .GetMethod(nameof(MakeFunc), BindingFlags.NonPublic | BindingFlags.Instance);
                if (method == null)
                    throw new InvalidOperationException("MakeFunc method not found.");
                method = method.MakeGenericMethod(actualType);
                result = method.Invoke(this, null);
                return true;
            }

            if (binding.LifeTime == LifeTime.Singleton)
            {
                if (binding.SingletonInstance == null)
                {
                    binding.SingletonInstance = CreateInstance(binding.ImplementationType);
                }

                result = binding.SingletonInstance;
                return true;
            }

            result = CreateInstance(binding.ImplementationType);
            return true;
        }

        private object CreateInstance(Type type)
        {
            ConstructorInfo ctor = type.GetConstructors()
                .OrderByDescending(c => c.GetParameters().Length)
                .FirstOrDefault();

            if (ctor == null) throw new InvalidOperationException($"No public constructor for {type.Name}");

            object[] parameters;
            try
            {
                parameters = ctor.GetParameters()
                    .Select(p => Resolve(p.ParameterType))
                    .ToArray();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed to resolve constructor parameters for {type.FullName}: {ex.Message}");
                throw;
            }

            try
            {
                return ctor.Invoke(parameters);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed to create instance of {type.FullName}: {ex.Message}");
                throw;
            }
        }

        private Func<T> MakeFunc<T>() => Resolve<T>;

        internal void RegisterBinding<TInterface, TImplementation>(LifeTime lifeTime)
        {
            _bindings[typeof(TInterface)] = new BindingInfo
            {
                ImplementationType = typeof(TImplementation),
                LifeTime = lifeTime
            };
        }

        internal void RegisterBinding<TConcrete>(LifeTime lifeTime)
        {
            _bindings[typeof(TConcrete)] = new BindingInfo
            {
                ImplementationType = typeof(TConcrete),
                LifeTime = lifeTime
            };
        }

        public void RegisterInstance<T>(T instance)
        {
            _bindings[typeof(T)] = new BindingInfo
            {
                FixedInstance = instance
            };
        }

        public void RegisterFactory<T>(Func<T> factory)
        {
            _factories[typeof(T)] = () => factory();
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;

            foreach (BindingInfo kvp in _bindings.Values) {
                try
                {
                    (kvp.SingletonInstance as IDisposable)?.Dispose();
                    (kvp.FixedInstance as IDisposable)?.Dispose();
                }
                catch (Exception ex)
                {
                    Logger.LogWarning($"Exception during disposal: {ex.Message}");
                }
            }

            _bindings.Clear();
            ContainerRegistry.Unregister(this);
        }
    }
}