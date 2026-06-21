using System;
using System.Collections.Generic;
using System.Threading;

using Cysharp.Threading.Tasks;

namespace YeKostenko.CoreKit.Scripts.Saving
{
    public sealed class SaveService : ISaveService
    {
        private readonly ISaveStorage _storage;
        private readonly ISaveSerializer _serializer;
        private readonly List<ISaveDataProvider> _modules;
        private readonly SaveMigrationService _migrationService;
        private readonly ICloudSaveSync _cloudSync;
        private readonly string _currentSaveVersion;
        private readonly SemaphoreSlim _operationLock = new SemaphoreSlim(1, 1);

        public event EventHandler<SaveSlotId> Saving;
        public event EventHandler<SaveSlotId> Saved;
        public event EventHandler<SaveSlotId> Loading;
        public event EventHandler<SaveSlotId> Loaded;
        public event EventHandler<SaveLoadFailedEventArgs> LoadFailed;

        public SaveService(
            ISaveStorage storage,
            ISaveSerializer serializer,
            IEnumerable<ISaveDataProvider> modules,
            SaveMigrationService migrationService,
            ICloudSaveSync cloudSync,
            string currentSaveVersion)
        {
            _storage = storage;
            _serializer = serializer;
            _modules = new List<ISaveDataProvider>(modules);
            _migrationService = migrationService;
            _cloudSync = cloudSync;
            _currentSaveVersion = currentSaveVersion;
        }

        public void RegisterProvider(ISaveDataProvider provider)
        {
            if (provider == null)
            {
                return;
            }

            if (!ContainsProvider(provider))
            {
                _modules.Add(provider);
            }
        }

        public void UnregisterProvider(ISaveDataProvider provider)
        {
            if (provider == null)
            {
                return;
            }

            _modules.Remove(provider);
        }

        public async UniTask<T?> LoadModuleAsync<T>(ISaveDataProvider module, SaveSlotId slotId)
        {
            string storageKey = SaveSlotKey.ToStorageKey(slotId);
            if (!_storage.Exists(storageKey))
            {
                return default;
            }

            GameSaveData migrated = await TryLoadAndMigrateEnvelopeAsync(storageKey);
            if (migrated == null)
            {
                return default;
            }

            if (!migrated.Data.TryGetValue(module.Key, out string moduleJson))
            {
                return default;
            }

            try
            {
                object obj = _serializer.Deserialize(moduleJson, module.DataType);
                if (obj == null)
                {
                    return default;
                }

                module.RestoreState(obj);
                return (T)obj;
            }
            catch (Exception ex)
            {
                LoadFailed?.Invoke(this, new SaveLoadFailedEventArgs(
                    slotId, LoadResultStatus.SerializationError, ex.Message));
                return default;
            }
        }

        public async UniTask<SaveResult> SaveAsync(SaveSlotId slotId, bool saveAllModules = false)
        {
            await _operationLock.WaitAsync();
            try
            {
                Saving?.Invoke(this, slotId);

                string storageKey = SaveSlotKey.ToStorageKey(slotId);
                GameSaveData saveData = await TryLoadEnvelopeAsync(storageKey);
                if (saveData == null)
                {
                    saveData = new GameSaveData();
                }

                long savedAtUtc = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                bool anyWritten = false;

                for (int i = 0; i < _modules.Count; i++)
                {
                    ISaveDataProvider module = _modules[i];
                    if (!saveAllModules && module is ISaveDirtyTracker dirtyTracker && !dirtyTracker.IsDirty)
                    {
                        continue;
                    }

                    try
                    {
                        object capturedState = module.CaptureState();
                        string moduleJson = _serializer.Serialize(capturedState);
                        saveData.Data[module.Key] = moduleJson;
                        saveData.Timestamps[module.Key] = savedAtUtc;
                        anyWritten = true;

                        if (module.Key == SaveMetadataProviderKeys.Metadata
                            && capturedState is SaveGameMetadata gameMetadata)
                        {
                            saveData.Metadata = CloneMetadata(gameMetadata);
                        }

                        if (module is ISaveDirtyTracker tracker)
                        {
                            tracker.ClearDirty();
                        }
                    }
                    catch (Exception ex)
                    {
                        return new SaveResult(SaveResultStatus.SerializationError, ex.Message);
                    }
                }

                if (!anyWritten)
                {
                    return SaveResult.NothingToSave();
                }

                saveData.Version = _currentSaveVersion;
                saveData.SavedAtUtc = savedAtUtc;

                string fullJson = _serializer.Serialize(saveData);

                try
                {
                    await _storage.SaveAsync(storageKey, fullJson);
                }
                catch (Exception ex)
                {
                    return SaveResult.IoError(ex.Message);
                }

                if (_cloudSync.IsAvailable)
                {
                    await _cloudSync.UploadAsync(slotId);
                }

                Saved?.Invoke(this, slotId);
                return SaveResult.Success();
            }
            finally
            {
                _operationLock.Release();
            }
        }

        public async UniTask<LoadResult> LoadAsync(SaveSlotId slotId)
        {
            await _operationLock.WaitAsync();
            try
            {
                Loading?.Invoke(this, slotId);

                string storageKey = SaveSlotKey.ToStorageKey(slotId);
                if (!_storage.Exists(storageKey))
                {
                    LoadFailed?.Invoke(this, new SaveLoadFailedEventArgs(
                        slotId, LoadResultStatus.NotFound, "Save file not found."));
                    return LoadResult.NotFound();
                }

                GameSaveData migrated = await TryLoadAndMigrateEnvelopeAsync(storageKey);
                if (migrated == null)
                {
                    LoadFailed?.Invoke(this, new SaveLoadFailedEventArgs(
                        slotId, LoadResultStatus.Corrupted, "Save file is corrupted."));
                    return LoadResult.Corrupted("Save file is corrupted.");
                }

                try
                {
                    for (int i = 0; i < _modules.Count; i++)
                    {
                        ISaveDataProvider module = _modules[i];
                        if (!migrated.Data.TryGetValue(module.Key, out string moduleJson))
                        {
                            continue;
                        }

                        object obj = _serializer.Deserialize(moduleJson, module.DataType);
                        if (obj == null)
                        {
                            continue;
                        }

                        module.RestoreState(obj);
                    }
                }
                catch (Exception ex)
                {
                    LoadFailed?.Invoke(this, new SaveLoadFailedEventArgs(
                        slotId, LoadResultStatus.SerializationError, ex.Message));
                    return LoadResult.SerializationError(ex.Message);
                }

                Loaded?.Invoke(this, slotId);
                return LoadResult.Success();
            }
            finally
            {
                _operationLock.Release();
            }
        }

        public async UniTask<SaveResult> DeleteAsync(SaveSlotId slotId)
        {
            string storageKey = SaveSlotKey.ToStorageKey(slotId);

            try
            {
                await _storage.DeleteAsync(storageKey);
                return SaveResult.Success();
            }
            catch (Exception ex)
            {
                return SaveResult.IoError(ex.Message);
            }
        }

        public bool HasSave(SaveSlotId slotId)
        {
            string storageKey = SaveSlotKey.ToStorageKey(slotId);
            return _storage.Exists(storageKey);
        }

        public SaveSlotMetadata GetSlotMetadata(SaveSlotId slotId)
        {
            string storageKey = SaveSlotKey.ToStorageKey(slotId);
            if (!_storage.Exists(storageKey))
            {
                return CreateEmptyMetadata(slotId);
            }

            try
            {
                string json = _storage.LoadSync(storageKey);
                return ParseSlotMetadata(slotId, json);
            }
            catch (Exception)
            {
                return CreateEmptyMetadata(slotId);
            }
        }

        public async UniTask<SaveSlotMetadata> GetSlotMetadataAsync(SaveSlotId slotId)
        {
            string storageKey = SaveSlotKey.ToStorageKey(slotId);
            if (!_storage.Exists(storageKey))
            {
                return CreateEmptyMetadata(slotId);
            }

            try
            {
                string json = await _storage.LoadAsync(storageKey);
                return ParseSlotMetadata(slotId, json);
            }
            catch (Exception)
            {
                return CreateEmptyMetadata(slotId);
            }
        }

        private SaveSlotMetadata ParseSlotMetadata(SaveSlotId slotId, string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return CreateEmptyMetadata(slotId);
            }

            GameSaveData saveData = _serializer.Deserialize<GameSaveData>(json);
            if (saveData == null)
            {
                return CreateEmptyMetadata(slotId);
            }

            return BuildMetadata(slotId, saveData);
        }

        public IReadOnlyList<SaveSlotMetadata> GetAllSlotMetadata()
        {
            List<SaveSlotMetadata> metadataList = new List<SaveSlotMetadata>(SaveSlotKey.ManualSlotCount + 1);

            for (int i = 0; i < SaveSlotKey.ManualSlotCount; i++)
            {
                SaveSlotId slotId = SaveSlotKey.FromManualSlotIndex(i);
                metadataList.Add(GetSlotMetadata(slotId));
            }

            metadataList.Add(GetSlotMetadata(SaveSlotId.AutoSave));
            return metadataList;
        }

        public async UniTask ResolveCloudOnStartupAsync()
        {
            if (!_cloudSync.IsAvailable)
            {
                return;
            }

            await _cloudSync.ResolveOnStartupAsync();
        }

        private async UniTask<GameSaveData> TryLoadEnvelopeAsync(string storageKey)
        {
            if (!_storage.Exists(storageKey))
            {
                return null;
            }

            try
            {
                string json = await _storage.LoadAsync(storageKey);
                if (string.IsNullOrEmpty(json))
                {
                    return null;
                }

                return _serializer.Deserialize<GameSaveData>(json);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private async UniTask<GameSaveData> TryLoadAndMigrateEnvelopeAsync(string storageKey)
        {
            GameSaveData rawSave = await TryLoadEnvelopeAsync(storageKey);
            if (rawSave == null)
            {
                return null;
            }

            return _migrationService.ApplyMigrations(rawSave);
        }

        private SaveSlotMetadata BuildMetadata(SaveSlotId slotId, GameSaveData saveData)
        {
            SaveSlotMetadata metadata = new SaveSlotMetadata();
            metadata.SlotId = slotId;
            metadata.SavedAtUtc = saveData.SavedAtUtc;
            metadata.Version = saveData.Version ?? string.Empty;

            bool hasModuleData = saveData.Data != null && saveData.Data.Count > 0;
            metadata.IsEmpty = saveData.SavedAtUtc <= 0 && !hasModuleData;

            SaveGameMetadata gameMetadata = ResolveGameMetadata(saveData);
            metadata.LocationName = gameMetadata.LocationName ?? string.Empty;
            metadata.PlayTimeSeconds = gameMetadata.PlayTimeSeconds;

            return metadata;
        }

        private SaveGameMetadata ResolveGameMetadata(GameSaveData saveData)
        {
            if (saveData.Metadata != null && !string.IsNullOrEmpty(saveData.Metadata.LocationName))
            {
                return saveData.Metadata;
            }

            if (saveData.Data != null
                && saveData.Data.TryGetValue(SaveMetadataProviderKeys.Metadata, out string metadataJson)
                && !string.IsNullOrEmpty(metadataJson))
            {
                SaveGameMetadata fromModule = _serializer.Deserialize<SaveGameMetadata>(metadataJson);
                if (fromModule != null)
                {
                    return fromModule;
                }
            }

            if (saveData.Metadata != null)
            {
                return saveData.Metadata;
            }

            return new SaveGameMetadata();
        }

        private static SaveGameMetadata CloneMetadata(SaveGameMetadata source)
        {
            SaveGameMetadata copy = new SaveGameMetadata();
            copy.LocationName = source.LocationName ?? string.Empty;
            copy.PlayTimeSeconds = source.PlayTimeSeconds;
            return copy;
        }

        private static SaveSlotMetadata CreateEmptyMetadata(SaveSlotId slotId)
        {
            SaveSlotMetadata metadata = new SaveSlotMetadata();
            metadata.SlotId = slotId;
            metadata.IsEmpty = true;
            return metadata;
        }

        private bool ContainsProvider(ISaveDataProvider provider)
        {
            for (int i = 0; i < _modules.Count; i++)
            {
                if (_modules[i] == provider)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
