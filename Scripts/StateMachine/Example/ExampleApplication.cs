namespace YeKostenko.CoreKit.StateMachine.Example
{
    public class ExampleApplication
    {
        public void Start()
        {
            ExampleContext context = new ExampleContext { Name = "Example", Value = 42 };
            ExampleStateMachine stateMachine = new ExampleStateMachine(context);
            stateMachine.RegisterState(new BootState(stateMachine));
            stateMachine.RegisterState(new MainMenuState(stateMachine));
            stateMachine.ChangeState<BootState>();
        }
    }
}