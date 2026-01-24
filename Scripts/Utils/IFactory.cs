namespace YevheniiKostenko.CoreKit.Utils
{
    public interface IFactory<out T>
    {
        T Create();
    }

    public interface IFactory<out TResult, in TParam>
    {
        TResult Create(TParam param);
    }
}