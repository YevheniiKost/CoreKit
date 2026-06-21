using System;
using System.IO;
using System.Threading;

using Cysharp.Threading.Tasks;

namespace YeKostenko.CoreKit.Scripts.Saving
{
    public class FileJsonStorage : ISaveStorage
    {
        private readonly string _directoryPath;
        private readonly SemaphoreSlim _writeLock = new SemaphoreSlim(1, 1);

        public FileJsonStorage(string directoryPath)
        {
            _directoryPath = directoryPath;
            Directory.CreateDirectory(_directoryPath);
        }

        public bool Exists(string key)
        {
            string path = GetPath(key);
            return File.Exists(path);
        }

        public async UniTask SaveAsync(string key, string json)
        {
            await _writeLock.WaitAsync();
            try
            {
                await UniTask.RunOnThreadPool(() => WriteAtomic(GetPath(key), json));
            }
            finally
            {
                _writeLock.Release();
            }
        }

        public async UniTask<string> LoadAsync(string key)
        {
            string path = GetPath(key);
            return await UniTask.RunOnThreadPool(() => ReadWithBackupFallback(path));
        }

        public string LoadSync(string key)
        {
            return ReadWithBackupFallback(GetPath(key));
        }

        public async UniTask DeleteAsync(string key)
        {
            await _writeLock.WaitAsync();
            try
            {
                await UniTask.RunOnThreadPool(() => DeleteFiles(key));
            }
            finally
            {
                _writeLock.Release();
            }
        }

        public async UniTask<long?> GetLastModifiedUtcAsync(string key)
        {
            string path = GetPath(key);
            return await UniTask.RunOnThreadPool(() => GetLastModifiedUtc(path));
        }

        private string GetPath(string key)
        {
            return Path.Combine(_directoryPath, key + ".json");
        }

        private static long? GetLastModifiedUtc(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }

            DateTime lastWrite = File.GetLastWriteTimeUtc(path);
            return new DateTimeOffset(lastWrite).ToUnixTimeSeconds();
        }

        private static void WriteAtomic(string path, string json)
        {
            string directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string tempPath = path + ".tmp";
            string backupPath = path + ".bak";

            File.WriteAllText(tempPath, json);

            if (File.Exists(path))
            {
                if (File.Exists(backupPath))
                {
                    File.Delete(backupPath);
                }

                File.Replace(tempPath, path, backupPath);
                return;
            }

            File.Move(tempPath, path);
        }

        private static string ReadWithBackupFallback(string path)
        {
            if (TryReadFile(path, out string content))
            {
                return content;
            }

            string backupPath = path + ".bak";
            if (TryReadFile(backupPath, out string backupContent))
            {
                return backupContent;
            }

            return null;
        }

        private static bool TryReadFile(string path, out string content)
        {
            content = null;
            if (!File.Exists(path))
            {
                return false;
            }

            content = File.ReadAllText(path);
            return true;
        }

        private void DeleteFiles(string key)
        {
            string path = GetPath(key);
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            string backupPath = path + ".bak";
            if (File.Exists(backupPath))
            {
                File.Delete(backupPath);
            }

            string tempPath = path + ".tmp";
            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }
        }
    }
}
