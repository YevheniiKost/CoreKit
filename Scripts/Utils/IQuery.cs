namespace YevheniiKostenko.CoreKit.Utils
{
    public interface IQuery<out TResult>
    {
        TResult Execute();
    }
    
    public interface IQuery<out TResult, in T>
    {
        TResult Execute(T parameter);
    }
}