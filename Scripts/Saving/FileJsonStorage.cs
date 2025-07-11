using System.IO;
using Cysharp.Threading.Tasks;

namespace YeKostenko.CoreKit.Scripts.Saving
{
    public class FileJsonStorage : ISaveStorage
    {
        private readonly string _path;

        public FileJsonStorage(string directoryPath)
        {
            _path = Path.Combine(directoryPath, "save.json");
        }

        public bool Exists(string key) => File.Exists(_path);

        public UniTask SaveAsync(string key, string json)
        {
            File.WriteAllText(_path, json);
            return UniTask.CompletedTask;
        }

        public UniTask<string?> LoadAsync(string key)
        {
            return UniTask.FromResult<string?>(File.Exists(_path) ? File.ReadAllText(_path) : null);
        }

        public UniTask DeleteAsync(string key)
        {
            if (File.Exists(_path)) File.Delete(_path);
            return UniTask.CompletedTask;
        }
    }
}