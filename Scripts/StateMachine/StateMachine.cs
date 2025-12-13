using System;
using System.Collections.Generic;

namespace YeKostenko.CoreKit.StateMachine
{
    public class StateMachine<TContext> : IDisposable
    {
        private readonly Dictionary<Type, IState> _states = new();
        private IState _currentState;
        private IState _previousState;

        public TContext Context { get; }

        public StateMachine(TContext context)
        {
            Context = context;
        }

        public void RegisterState<T>(T state) where T : IState
        {
            _states[typeof(T)] = state;
        }

        public void ChangeState<T>(object payload = null) where T : IState
        {
            var newState = _states[typeof(T)];
            newState.Prepare(payload);

            _previousState = _currentState;
            
            _currentState?.Exit();
            _currentState = newState;
            _currentState.Enter(payload);
            
            OnChangeState(_previousState, _currentState);
        }

        public T GetState<T>() where T : IState
        {
            return (T)_states[typeof(T)];
        }

        public void Dispose()
        {
            foreach (var state in _states.Values)
            {
                if (state is IDisposable disposableState)
                {
                    disposableState.Dispose();
                }
            }
            
            _states.Clear();
            _currentState = null;
        }

        protected virtual void OnChangeState(IState previousState, IState newState)
        {
            
        }
    }
}