namespace YeKostenko.CoreKit.StateMachine
{
    public abstract class BaseState<TContext> : IState
    {
        protected StateMachine<TContext> StateMachine { get; private set; }
        protected TContext Context => StateMachine.Context;

        protected BaseState(StateMachine<TContext> stateMachine)
        {
            StateMachine = stateMachine;
        }

        public abstract void Prepare(object payload = null);
        public abstract void Enter(object payload = null);
        public abstract void Exit();
    }
}