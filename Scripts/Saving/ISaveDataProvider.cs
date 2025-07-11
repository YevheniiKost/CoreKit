using System;

namespace YeKostenko.CoreKit.Scripts.Saving
{
    public interface ISaveDataProvider
    {
        string Key { get; }
        object CaptureState();
        void RestoreState(object state);
        Type DataType { get; }
    }
}