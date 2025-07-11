namespace YeKostenko.CoreKit.StateMachine.Example
{
    public abstract class ExampleApplicationState : BaseState<ExampleContext>
    {
        protected ExampleApplicationState(StateMachine<ExampleContext> stateMachine) : base(stateMachine)
        {
        }
    }
}