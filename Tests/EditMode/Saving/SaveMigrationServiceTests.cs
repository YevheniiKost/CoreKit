using System.Collections.Generic;

using NUnit.Framework;

using YeKostenko.CoreKit.Scripts.Saving;

namespace YeKostenko.CoreKit.Tests.EditMode.Saving
{
    public sealed class SaveMigrationServiceTests
    {
        [Test]
        public void ApplyMigrations_RunsChainUntilNoMatch()
        {
            TestSaveMigration firstStep = new TestSaveMigration();
            SaveMigrationService service = new SaveMigrationService(new List<ISaveMigration> { firstStep });

            GameSaveData data = new GameSaveData();
            data.Version = "1.0.0";

            GameSaveData migrated = service.ApplyMigrations(data);

            Assert.IsTrue(firstStep.WasApplied);
            Assert.AreEqual("2.0.0", migrated.Version);
            Assert.IsTrue(migrated.Data.ContainsKey("migration_marker"));
        }

        [Test]
        public void ApplyMigrations_WhenAlreadyCurrent_DoesNothing()
        {
            TestSaveMigration migration = new TestSaveMigration();
            SaveMigrationService service = new SaveMigrationService(new List<ISaveMigration> { migration });

            GameSaveData data = new GameSaveData();
            data.Version = "2.0.0";

            GameSaveData migrated = service.ApplyMigrations(data);

            Assert.IsFalse(migration.WasApplied);
            Assert.AreEqual("2.0.0", migrated.Version);
        }
    }
}
