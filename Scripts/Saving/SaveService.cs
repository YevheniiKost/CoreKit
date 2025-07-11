using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

namespace YeKostenko.CoreKit.Scripts.Saving
{
    public class SaveService
    {
        private readonly ISaveStorage _storage;
        private readonly ISaveSerializer _serializer;
        private readonly List<ISaveDataProvider> _modules;
        private readonly SaveMigrationService _migrationService;

        private const string SaveKey = "game_data";

        public SaveService(ISaveStorage storage, ISaveSerializer serializer,
            IEnumerable<ISaveDataProvider> modules, SaveMigrationService migrationService)
        {
            _storage = storage;
            _serializer = serializer;
            _modules = modules.ToList();
            _migrationService = migrationService;
        }
        
        public void RegisterModule(ISaveDataProvider module)
        {
            if (!_modules.Contains(module))
                _modules.Add(module);
        }

        public void UnregisterModule(ISaveDataProvider module)
        {
            _modules.Remove(module);
        }
        
        public async UniTask<T?> LoadModuleAsync<T>(ISaveDataProvider module)
        {
            if (!_storage.Exists(SaveKey)) return default;

            string json = await _storage.LoadAsync(SaveKey);
            var saveData = _serializer.Deserialize<GameSaveData>(json);
            var migrated = _migrationService.ApplyMigrations(saveData);

            if (migrated.Data.TryGetValue(module.Key, out var moduleJson))
            {
                var obj = _serializer.Deserialize(moduleJson, module.DataType);
                module.RestoreState(obj);
                return (T)obj;
            }

            return default;
        }

        public async UniTask SaveAsync()
        {
            var saveData = new GameSaveData();

            foreach (var module in _modules)
            {
                if (module is ISaveDirtyTracker tracker && !tracker.IsDirty)
                    continue;

                var obj = module.CaptureState();
                string json = _serializer.Serialize(obj);
                saveData.Data[module.Key] = json;
                saveData.Timestamps[module.Key] = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                if (module is ISaveDirtyTracker dt)
                    dt.ClearDirty();
            }

            string fullJson = _serializer.Serialize(saveData);
            await _storage.SaveAsync(SaveKey, fullJson);
        }

        public async UniTask LoadAsync()
        {
            if (!_storage.Exists(SaveKey)) return;

            string fullJson = await _storage.LoadAsync(SaveKey);
            var rawSave = _serializer.Deserialize<GameSaveData>(fullJson);
            var migratedSave = _migrationService.ApplyMigrations(rawSave);

            foreach (var module in _modules)
            {
                if (!migratedSave.Data.TryGetValue(module.Key, out var json)) continue;

                object obj = _serializer.Deserialize(json, module.DataType);
                module.RestoreState(obj);
            }
        }
    }
}