namespace YevheniiKostenko.CoreKit.Time
{
    public interface ITimeProvider
    {
        void RegisterTimeListener(ITimeListener listener);
        void ClearTimeListener(ITimeListener listener);
        
        void SetTimeScale(float timeScale);
    }
}