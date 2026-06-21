using System.Collections.Generic;

using Cysharp.Threading.Tasks;

namespace YeKostenko.CoreKit.Scripts.Saving
{
    public sealed class NullCloudSaveSync : ICloudSaveSync
    {
        public bool IsAvailable => false;

        public UniTask<CloudSyncResult> UploadAsync(SaveSlotId slotId)
        {
            return UniTask.FromResult(CloudSyncResult.NotAvailable());
        }

        public UniTask<CloudSyncResult> DownloadAsync(SaveSlotId slotId)
        {
            return UniTask.FromResult(CloudSyncResult.NotAvailable());
        }

        public UniTask<IReadOnlyList<CloudConflictResolution>> ResolveOnStartupAsync()
        {
            List<CloudConflictResolution> empty = new List<CloudConflictResolution>();
            return UniTask.FromResult<IReadOnlyList<CloudConflictResolution>>(empty);
        }
    }
}
