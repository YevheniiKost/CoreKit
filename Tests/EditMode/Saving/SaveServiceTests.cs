using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Cysharp.Threading.Tasks;

using NUnit.Framework;

using UnityEngine.TestTools;

using YeKostenko.CoreKit.Scripts.Saving;

namespace YeKostenko.CoreKit.Tests.EditMode.Saving
{
    public sealed class SaveServiceTests
    {
        private string _testDirectory;
        private FileJsonStorage _storage;

        [SetUp]
        public void SetUp()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), "GreedyGoldSaveServiceTests", Guid.NewGuid().ToString());
            _storage = new FileJsonStorage(_testDirectory);
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, recursive: true);
            }
        }

        [UnityTest]
        public IEnumerator SaveAsync_NewSlot_WritesSuccessAndSetsVersion()
        {
            return UniTask.ToCoroutine(async () =>
            {
                TestModuleData state = new TestModuleData { Value = 10, Label = "alpha" };
                FakeSaveProvider provider = new FakeSaveProvider("module_a", state);
                SaveService service = SaveTestFactory.CreateService(_storage, new[] { provider });

                SaveResult result = await service.SaveAsync(SaveSlotId.Slot0, saveAllModules: true);

                Assert.IsTrue(result.IsSuccess);
                Assert.IsTrue(service.HasSave(SaveSlotId.Slot0));

                SaveSlotMetadata metadata = await service.GetSlotMetadataAsync(SaveSlotId.Slot0);
                Assert.IsFalse(metadata.IsEmpty);
                Assert.AreEqual(SaveTestFactory.CurrentVersion, metadata.Version);
            });
        }

        [UnityTest]
        public IEnumerator LoadAsync_ExistingSave_RestoresProviderState()
        {
            return UniTask.ToCoroutine(async () =>
            {
                TestModuleData state = new TestModuleData { Value = 42, Label = "loaded" };
                FakeSaveProvider provider = new FakeSaveProvider("module_a", state);
                SaveService service = SaveTestFactory.CreateService(_storage, new[] { provider });

                await service.SaveAsync(SaveSlotId.Slot1, saveAllModules: true);

                state.Value = 0;
                state.Label = string.Empty;

                LoadResult loadResult = await service.LoadAsync(SaveSlotId.Slot1);

                Assert.IsTrue(loadResult.IsSuccess);
                Assert.AreEqual(42, state.Value);
                Assert.AreEqual("loaded", state.Label);
            });
        }

        [UnityTest]
        public IEnumerator LoadAsync_MissingFile_ReturnsNotFound()
        {
            return UniTask.ToCoroutine(async () =>
            {
                TestModuleData state = new TestModuleData();
                FakeSaveProvider provider = new FakeSaveProvider("module_a", state);
                SaveService service = SaveTestFactory.CreateService(_storage, new[] { provider });

                LoadResult result = await service.LoadAsync(SaveSlotId.Slot2);

                Assert.AreEqual(LoadResultStatus.NotFound, result.Status);
            });
        }

        [UnityTest]
        public IEnumerator SaveAsync_DirtyPartialSave_PreservesCleanModuleData()
        {
            return UniTask.ToCoroutine(async () =>
            {
                TestModuleData stateA = new TestModuleData { Value = 100 };
                TestModuleData stateB = new TestModuleData { Value = 200 };
                FakeSaveProvider providerA = new FakeSaveProvider("module_a", stateA);
                FakeSaveProvider providerB = new FakeSaveProvider("module_b", stateB);

                SaveService service = SaveTestFactory.CreateService(
                    _storage,
                    new ISaveDataProvider[] { providerA, providerB });

                await service.SaveAsync(SaveSlotId.Slot0, saveAllModules: true);

                providerA.ClearDirty();
                stateB.Value = 250;
                providerB.MarkDirty();

                SaveResult partialResult = await service.SaveAsync(SaveSlotId.Slot0, saveAllModules: false);
                Assert.IsTrue(partialResult.IsSuccess);

                stateA.Value = 0;
                stateB.Value = 0;

                await service.LoadAsync(SaveSlotId.Slot0);

                Assert.AreEqual(100, stateA.Value);
                Assert.AreEqual(250, stateB.Value);
            });
        }

        [UnityTest]
        public IEnumerator SaveAsync_AllModulesClean_ReturnsNothingToSave()
        {
            return UniTask.ToCoroutine(async () =>
            {
                TestModuleData state = new TestModuleData { Value = 5 };
                FakeSaveProvider provider = new FakeSaveProvider("module_a", state);
                provider.ClearDirty();

                SaveService service = SaveTestFactory.CreateService(_storage, new[] { provider });

                SaveResult result = await service.SaveAsync(SaveSlotId.Slot0, saveAllModules: false);

                Assert.AreEqual(SaveResultStatus.NothingToSave, result.Status);
                Assert.IsFalse(service.HasSave(SaveSlotId.Slot0));
            });
        }

        [UnityTest]
        public IEnumerator SaveAsync_SaveAllModules_WritesEvenWhenClean()
        {
            return UniTask.ToCoroutine(async () =>
            {
                TestModuleData state = new TestModuleData { Value = 7 };
                FakeSaveProvider provider = new FakeSaveProvider("module_a", state);
                provider.ClearDirty();

                SaveService service = SaveTestFactory.CreateService(_storage, new[] { provider });

                SaveResult result = await service.SaveAsync(SaveSlotId.Slot0, saveAllModules: true);

                Assert.IsTrue(result.IsSuccess);
                Assert.IsTrue(service.HasSave(SaveSlotId.Slot0));
            });
        }

        [UnityTest]
        public IEnumerator DeleteAsync_RemovesSaveFile()
        {
            return UniTask.ToCoroutine(async () =>
            {
                TestModuleData state = new TestModuleData { Value = 1 };
                FakeSaveProvider provider = new FakeSaveProvider("module_a", state);
                SaveService service = SaveTestFactory.CreateService(_storage, new[] { provider });

                await service.SaveAsync(SaveSlotId.Slot3, saveAllModules: true);
                Assert.IsTrue(service.HasSave(SaveSlotId.Slot3));

                SaveResult deleteResult = await service.DeleteAsync(SaveSlotId.Slot3);

                Assert.IsTrue(deleteResult.IsSuccess);
                Assert.IsFalse(service.HasSave(SaveSlotId.Slot3));
            });
        }

        [UnityTest]
        public IEnumerator GetSlotMetadata_ReturnsEnvelopeMetadata()
        {
            return UniTask.ToCoroutine(async () =>
            {
                TestModuleData state = new TestModuleData { Value = 1 };
                FakeSaveProvider provider = new FakeSaveProvider("module_a", state);
                FakeMetadataProvider metadataProvider = new FakeMetadataProvider();
                metadataProvider.Metadata.LocationName = "Cave";
                metadataProvider.Metadata.PlayTimeSeconds = 90f;

                SaveService service = SaveTestFactory.CreateService(
                    _storage,
                    new ISaveDataProvider[] { provider, metadataProvider });

                await service.SaveAsync(SaveSlotId.Slot0, saveAllModules: true);

                SaveSlotMetadata metadata = await service.GetSlotMetadataAsync(SaveSlotId.Slot0);

                Assert.AreEqual("Cave", metadata.LocationName);
                Assert.AreEqual(90f, metadata.PlayTimeSeconds, 0.001f);
                Assert.Greater(metadata.SavedAtUtc, 0L);
            });
        }

        [Test]
        public void GetAllSlotMetadata_ReturnsManualSlotsAndAutoSave()
        {
            TestModuleData state = new TestModuleData { Value = 1 };
            FakeSaveProvider provider = new FakeSaveProvider("module_a", state);
            SaveService service = SaveTestFactory.CreateService(_storage, new[] { provider });

            IReadOnlyList<SaveSlotMetadata> allMetadata = service.GetAllSlotMetadata();

            Assert.AreEqual(SaveSlotKey.ManualSlotCount + 1, allMetadata.Count);
            Assert.AreEqual(SaveSlotId.AutoSave, allMetadata[allMetadata.Count - 1].SlotId);
        }

        [UnityTest]
        public IEnumerator LoadAsync_AppliesMigrationsBeforeRestore()
        {
            return UniTask.ToCoroutine(async () =>
            {
                TestSaveMigration migration = new TestSaveMigration();
                TestModuleData state = new TestModuleData { Value = 1 };
                FakeSaveProvider provider = new FakeSaveProvider("module_a", state);

                GameSaveData legacyEnvelope = new GameSaveData();
                legacyEnvelope.Version = "1.0.0";
                legacyEnvelope.SavedAtUtc = 100;
                legacyEnvelope.Data["module_a"] = "{\"Value\":1,\"Label\":\"\"}";

                NewtonsoftJsonSerializer serializer = new NewtonsoftJsonSerializer();
                await _storage.SaveAsync("slot_0", serializer.Serialize(legacyEnvelope));

                SaveService loadService = SaveTestFactory.CreateService(
                    _storage,
                    new ISaveDataProvider[] { provider },
                    migrations: new ISaveMigration[] { migration });

                LoadResult result = await loadService.LoadAsync(SaveSlotId.Slot0);

                Assert.IsTrue(result.IsSuccess);
                Assert.IsTrue(migration.WasApplied);
            });
        }

        [UnityTest]
        public IEnumerator SaveAsync_WhenCloudAvailable_UploadsAfterLocalSave()
        {
            return UniTask.ToCoroutine(async () =>
            {
                TrackingCloudSaveSync cloudSync = new TrackingCloudSaveSync();
                TestModuleData state = new TestModuleData { Value = 3 };
                FakeSaveProvider provider = new FakeSaveProvider("module_a", state);

                SaveService service = SaveTestFactory.CreateService(
                    _storage,
                    new[] { provider },
                    cloudSync);

                await service.SaveAsync(SaveSlotId.AutoSave, saveAllModules: true);

                Assert.AreEqual(1, cloudSync.UploadCount);
                Assert.Contains(SaveSlotId.AutoSave, cloudSync.UploadedSlots);
            });
        }

        [UnityTest]
        public IEnumerator ResolveCloudOnStartupAsync_WithNullCloud_DoesNotThrow()
        {
            return UniTask.ToCoroutine(async () =>
            {
                TestModuleData state = new TestModuleData();
                FakeSaveProvider provider = new FakeSaveProvider("module_a", state);
                SaveService service = SaveTestFactory.CreateService(_storage, new[] { provider });

                await service.ResolveCloudOnStartupAsync();
            });
        }

        [UnityTest]
        public IEnumerator ManualAndAutoSave_UseSeparateFiles()
        {
            return UniTask.ToCoroutine(async () =>
            {
                TestModuleData manualState = new TestModuleData { Value = 1 };
                TestModuleData autoState = new TestModuleData { Value = 2 };
                FakeSaveProvider manualProvider = new FakeSaveProvider("module_a", manualState);
                FakeSaveProvider autoProvider = new FakeSaveProvider("module_b", autoState);

                SaveService manualService = SaveTestFactory.CreateService(_storage, new[] { manualProvider });
                SaveService autoService = SaveTestFactory.CreateService(_storage, new[] { autoProvider });

                await manualService.SaveAsync(SaveSlotId.Slot0, saveAllModules: true);
                await autoService.SaveAsync(SaveSlotId.AutoSave, saveAllModules: true);

                Assert.IsTrue(File.Exists(Path.Combine(_testDirectory, "slot_0.json")));
                Assert.IsTrue(File.Exists(Path.Combine(_testDirectory, "autosave.json")));
            });
        }
    }
}
