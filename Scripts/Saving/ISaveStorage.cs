using Cysharp.Threading.Tasks;

namespace YeKostenko.CoreKit.Scripts.Saving
{
    public interface ISaveStorage
    {
        UniTask SaveAsync(string key, string json);
        UniTask<string> LoadAsync(string key);
        UniTask DeleteAsync(string key);
        bool Exists(string key);
    }
}