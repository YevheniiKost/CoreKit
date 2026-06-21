using System;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using Newtonsoft.Json;

using YeKostenko.CoreKit.Scripts.Saving;

namespace YeKostenko.CoreKit.Tests.EditMode.Saving
{
    [JsonObject(MemberSerialization.Fields)]
    internal sealed class TestModuleData
    {
        public int Value;
        public string Label = string.Empty;
    }

    internal sealed class FakeSaveProvider : ISaveDataProviderWithDirty
    {
        private bool _isDirty = true;

        public FakeSaveProvider(string key, TestModuleData state)
        {
            Key = key;
            State = state;
        }

        public string Key { get; }

        public TestModuleData State { get; }

        public Type DataType => typeof(TestModuleData);

        public bool IsDirty => _isDirty;

        public object CaptureState()
        {
            TestModuleData copy = new TestModuleData();
            copy.Value = State.Value;
            copy.Label = State.Label;
            return copy;
        }

        public void RestoreState(object state)
        {
            if (state is not TestModuleData data)
            {
                return;
            }

            State.Value = data.Value;
            State.Label = data.Label;
            _isDirty = false;
        }

        public void MarkDirty()
        {
            _isDirty = true;
        }

        public void ClearDirty()
        {
            _isDirty = false;
        }
    }

    internal sealed class FakeMetadataProvider : ISaveDataProvider
    {
        public SaveGameMetadata Metadata = new SaveGameMetadata();

        public string Key => SaveMetadataProviderKeys.Metadata;

        public Type DataType => typeof(SaveGameMetadata);

        public object CaptureState()
        {
            SaveGameMetadata copy = new SaveGameMetadata();
            copy.LocationName = Metadata.LocationName;
            copy.PlayTimeSeconds = Metadata.PlayTimeSeconds;
            return copy;
        }

        public void RestoreState(object state)
        {
            if (state is not SaveGameMetadata metadata)
            {
                return;
            }

            Metadata.LocationName = metadata.LocationName;
            Metadata.PlayTimeSeconds = metadata.PlayTimeSeconds;
        }
    }

    internal sealed class TrackingCloudSaveSync : ICloudSaveSync
    {
        public bool IsAvailable { get; set; } = true;

        public int UploadCount { get; private set; }

        public List<SaveSlotId> UploadedSlots { get; } = new List<SaveSlotId>();

        public UniTask<CloudSyncResult> UploadAsync(SaveSlotId slotId)
        {
            UploadCount++;
            UploadedSlots.Add(slotId);
            return UniTask.FromResult(CloudSyncResult.Success());
        }

        public UniTask<CloudSyncResult> DownloadAsync(SaveSlotId slotId)
        {
            return UniTask.FromResult(CloudSyncResult.Success());
        }

        public UniTask<IReadOnlyList<CloudConflictResolution>> ResolveOnStartupAsync()
        {
            List<CloudConflictResolution> empty = new List<CloudConflictResolution>();
            return UniTask.FromResult<IReadOnlyList<CloudConflictResolution>>(empty);
        }
    }

    internal sealed class TestSaveMigration : ISaveMigration
    {
        public string FromVersion => "1.0.0";

        public string ToVersion => "2.0.0";

        public bool WasApplied { get; private set; }

        public GameSaveData Migrate(GameSaveData oldData)
        {
            WasApplied = true;
            oldData.Data["migration_marker"] = "\"applied\"";
            return oldData;
        }
    }

    internal static class SaveTestFactory
    {
        public const string CurrentVersion = "2.0.0";

        public static SaveService CreateService(
            ISaveStorage storage,
            IEnumerable<ISaveDataProvider> providers,
            ICloudSaveSync cloudSync = null,
            IEnumerable<ISaveMigration> migrations = null)
        {
            SaveMigrationService migrationService = new SaveMigrationService(migrations ?? Array.Empty<ISaveMigration>());
            ICloudSaveSync sync = cloudSync ?? new NullCloudSaveSync();

            return new SaveService(
                storage,
                new NewtonsoftJsonSerializer(),
                providers,
                migrationService,
                sync,
                CurrentVersion);
        }
    }
}
