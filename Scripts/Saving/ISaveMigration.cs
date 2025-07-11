namespace YeKostenko.CoreKit.Scripts.Saving
{
    public interface ISaveMigration
    {
        string FromVersion { get; }
        string ToVersion { get; }
        GameSaveData Migrate(GameSaveData oldData);
    }
}