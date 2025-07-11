namespace YeKostenko.CoreKit.Scripts.Saving
{
    public interface ISaveDirtyTracker
    {
        bool IsDirty { get; }
        void MarkDirty();
        void ClearDirty();
    }
}