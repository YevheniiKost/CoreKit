namespace YeKostenko.CoreKit.StateMachine
{
    public interface IState
    {
        void Prepare(object payload = null);
        void Enter(object payload = null);
        void Exit();
    }
}