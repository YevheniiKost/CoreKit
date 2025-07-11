using System.Collections.Generic;
using System.Linq;

namespace YeKostenko.CoreKit.Scripts.Saving
{
    public class SaveMigrationService
    {
        private readonly List<ISaveMigration> _migrations;

        public SaveMigrationService(IEnumerable<ISaveMigration> migrations)
        {
            _migrations = migrations.OrderBy(m => m.FromVersion).ToList();
        }

        public GameSaveData ApplyMigrations(GameSaveData original)
        {
            string currentVersion = original.Version;

            while (true)
            {
                var migration = _migrations.FirstOrDefault(m => m.FromVersion == currentVersion);
                if (migration == null)
                    break;

                original = migration.Migrate(original);
                currentVersion = migration.ToVersion;
                original.Version = currentVersion;
            }

            return original;
        }
    }
}