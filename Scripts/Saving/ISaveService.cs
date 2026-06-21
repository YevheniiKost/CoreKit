using System;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

namespace YeKostenko.CoreKit.Scripts.Saving
{
    public class SaveLoadFailedEventArgs : EventArgs
    {
        public SaveSlotId SlotId { get; }
        public LoadResultStatus Status { get; }
        public string Message { get; }

        public SaveLoadFailedEventArgs(SaveSlotId slotId, LoadResultStatus status, string message)
        {
            SlotId = slotId;
            Status = status;
            Message = message ?? string.Empty;
        }
    }

    public interface ISaveService
    {
        event EventHandler<SaveSlotId> Saving;
        event EventHandler<SaveSlotId> Saved;
        event EventHandler<SaveSlotId> Loading;
        event EventHandler<SaveSlotId> Loaded;
        event EventHandler<SaveLoadFailedEventArgs> LoadFailed;

        void RegisterProvider(ISaveDataProvider provider);

        void UnregisterProvider(ISaveDataProvider provider);

        UniTask<SaveResult> SaveAsync(SaveSlotId slotId, bool saveAllModules = false);

        UniTask<LoadResult> LoadAsync(SaveSlotId slotId);

        UniTask<SaveResult> DeleteAsync(SaveSlotId slotId);

        bool HasSave(SaveSlotId slotId);

        SaveSlotMetadata GetSlotMetadata(SaveSlotId slotId);

        UniTask<SaveSlotMetadata> GetSlotMetadataAsync(SaveSlotId slotId);

        IReadOnlyList<SaveSlotMetadata> GetAllSlotMetadata();

        UniTask ResolveCloudOnStartupAsync();

        UniTask<T?> LoadModuleAsync<T>(ISaveDataProvider module, SaveSlotId slotId);
    }
}
