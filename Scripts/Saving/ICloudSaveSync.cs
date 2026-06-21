using System;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

namespace YeKostenko.CoreKit.Scripts.Saving
{
    public interface ICloudSaveSync
    {
        bool IsAvailable { get; }

        UniTask<CloudSyncResult> UploadAsync(SaveSlotId slotId);

        UniTask<CloudSyncResult> DownloadAsync(SaveSlotId slotId);

        UniTask<IReadOnlyList<CloudConflictResolution>> ResolveOnStartupAsync();
    }
}
