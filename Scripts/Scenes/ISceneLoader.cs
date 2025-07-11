using System;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace YeKostenko.CoreKit.Scenes
{
    public interface ISceneLoader
    {
        UniTask LoadSceneAsync(
            string sceneName,
            LoadSceneMode mode = LoadSceneMode.Single,
            bool setActive = true,
            Action<float> onProgress = null
        );
        UniTask UnloadSceneAsync(string sceneName);
        bool IsSceneLoaded(string sceneName);
    }
}