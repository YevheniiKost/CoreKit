namespace YeKostenko.CoreKit.StateMachine.Example
{
    public class MainMenuState : ExampleApplicationState
    {
        public MainMenuState(ExampleStateMachine stateMachine) : base(stateMachine)
        {
        }

        public override void Prepare(object payload = null)
        {
        }

        public override void Enter(object payload = null)
        {
            Logging.Logger.Log("Entering main menu...");
            // Here you can add logic to display the main menu
            // For example, you can change the context or trigger some actions
        }

        public override void Exit()
        {
            Logging.Logger.Log("Exiting main menu...");
        }
    }
}