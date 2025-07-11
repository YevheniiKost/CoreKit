namespace YeKostenko.CoreKit.StateMachine.Example
{
    public class BootState : ExampleApplicationState
    {
        public BootState(ExampleStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Prepare(object payload = null)
        {
        }

        public override void Enter(object payload = null)
        {
            Logging.Logger.Log("Booting application...");
            StateMachine.ChangeState<ExampleApplicationState>();
        }

        public override void Exit()
        {
            Logging.Logger.Log("Boot completed, transitioning to main menu...");
        }
    }
}