using System;
using System.Collections;
using System.IO;

using Cysharp.Threading.Tasks;

using NUnit.Framework;

using UnityEngine.TestTools;

using YeKostenko.CoreKit.Scripts.Saving;

namespace YeKostenko.CoreKit.Tests.EditMode.Saving
{
    public sealed class FileJsonStorageTests
    {
        private string _testDirectory;

        [SetUp]
        public void SetUp()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), "GreedyGoldSaveTests", Guid.NewGuid().ToString());
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
        public IEnumerator SaveAsync_CreatesFileNamedByKey()
        {
            return UniTask.ToCoroutine(async () =>
            {
                FileJsonStorage storage = new FileJsonStorage(_testDirectory);
                await storage.SaveAsync("slot_0", "{\"test\":true}");

                string expectedPath = Path.Combine(_testDirectory, "slot_0.json");
                Assert.IsTrue(File.Exists(expectedPath));
            });
        }

        [Test]
        public void Exists_ReturnsFalseWhenFileMissing()
        {
            FileJsonStorage storage = new FileJsonStorage(_testDirectory);

            Assert.IsFalse(storage.Exists("missing"));
        }

        [UnityTest]
        public IEnumerator LoadAsync_ReturnsWrittenContent()
        {
            return UniTask.ToCoroutine(async () =>
            {
                FileJsonStorage storage = new FileJsonStorage(_testDirectory);
                const string payload = "{\"value\":42}";

                await storage.SaveAsync("slot_1", payload);
                string loaded = await storage.LoadAsync("slot_1");

                Assert.AreEqual(payload, loaded);
            });
        }

        [UnityTest]
        public IEnumerator DeleteAsync_RemovesMainAndSidecarFiles()
        {
            return UniTask.ToCoroutine(async () =>
            {
                FileJsonStorage storage = new FileJsonStorage(_testDirectory);
                await storage.SaveAsync("slot_0", "{\"a\":1}");
                await storage.SaveAsync("slot_0", "{\"a\":2}");
                await storage.DeleteAsync("slot_0");

                Assert.IsFalse(storage.Exists("slot_0"));
                Assert.IsFalse(File.Exists(Path.Combine(_testDirectory, "slot_0.json.bak")));
            });
        }

        [UnityTest]
        public IEnumerator SaveAsync_Overwrite_CreatesBackupFile()
        {
            return UniTask.ToCoroutine(async () =>
            {
                FileJsonStorage storage = new FileJsonStorage(_testDirectory);
                await storage.SaveAsync("slot_0", "{\"version\":1}");
                await storage.SaveAsync("slot_0", "{\"version\":2}");

                string backupPath = Path.Combine(_testDirectory, "slot_0.json.bak");
                Assert.IsTrue(File.Exists(backupPath));
                Assert.AreEqual("{\"version\":1}", File.ReadAllText(backupPath));
            });
        }

        [UnityTest]
        public IEnumerator LoadAsync_FallsBackToBackupWhenMainMissing()
        {
            return UniTask.ToCoroutine(async () =>
            {
                FileJsonStorage storage = new FileJsonStorage(_testDirectory);
                await storage.SaveAsync("slot_0", "{\"version\":1}");
                await storage.SaveAsync("slot_0", "{\"version\":2}");

                string mainPath = Path.Combine(_testDirectory, "slot_0.json");
                File.Delete(mainPath);

                string loaded = await storage.LoadAsync("slot_0");

                Assert.AreEqual("{\"version\":1}", loaded);
            });
        }

        [UnityTest]
        public IEnumerator LoadSync_ReturnsWrittenContent()
        {
            return UniTask.ToCoroutine(async () =>
            {
                FileJsonStorage storage = new FileJsonStorage(_testDirectory);
                const string payload = "{\"value\":99}";

                await storage.SaveAsync("slot_2", payload);
                string loaded = storage.LoadSync("slot_2");

                Assert.AreEqual(payload, loaded);
            });
        }

        [UnityTest]
        public IEnumerator GetLastModifiedUtcAsync_ReturnsValueForExistingFile()
        {
            return UniTask.ToCoroutine(async () =>
            {
                FileJsonStorage storage = new FileJsonStorage(_testDirectory);
                await storage.SaveAsync("slot_0", "{}");

                long? timestamp = await storage.GetLastModifiedUtcAsync("slot_0");

                Assert.IsTrue(timestamp.HasValue);
                Assert.Greater(timestamp.Value, 0L);
            });
        }

        [UnityTest]
        public IEnumerator GetLastModifiedUtcAsync_ReturnsNullForMissingFile()
        {
            return UniTask.ToCoroutine(async () =>
            {
                FileJsonStorage storage = new FileJsonStorage(_testDirectory);

                long? timestamp = await storage.GetLastModifiedUtcAsync("missing");

                Assert.IsFalse(timestamp.HasValue);
            });
        }
    }
}
