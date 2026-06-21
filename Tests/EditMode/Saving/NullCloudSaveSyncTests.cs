using System.Collections.Generic;

using NUnit.Framework;

using YeKostenko.CoreKit.Scripts.Saving;

namespace YeKostenko.CoreKit.Tests.EditMode.Saving
{
    public sealed class NullCloudSaveSyncTests
    {
        [Test]
        public void IsAvailable_ReturnsFalse()
        {
            NullCloudSaveSync sync = new NullCloudSaveSync();

            Assert.IsFalse(sync.IsAvailable);
        }

        [Test]
        public void UploadAsync_ReturnsNotAvailable()
        {
            NullCloudSaveSync sync = new NullCloudSaveSync();

            CloudSyncResult result = sync.UploadAsync(SaveSlotId.Slot0).GetAwaiter().GetResult();

            Assert.AreEqual(CloudSyncResultStatus.NotAvailable, result.Status);
        }

        [Test]
        public void ResolveOnStartupAsync_ReturnsEmptyList()
        {
            NullCloudSaveSync sync = new NullCloudSaveSync();

            IReadOnlyList<CloudConflictResolution> resolutions =
                sync.ResolveOnStartupAsync().GetAwaiter().GetResult();

            Assert.IsNotNull(resolutions);
            Assert.AreEqual(0, resolutions.Count);
        }
    }
}
