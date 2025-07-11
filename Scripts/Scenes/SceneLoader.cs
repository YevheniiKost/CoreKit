using System;
using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.SceneManagement;

using Logger = YeKostenko.CoreKit.Logging.Logger;

namespace YeKostenko.CoreKit.Scenes
{
    public class SceneLoader : ISceneLoader
    {
        private readonly HashSet<string> _loadedScenes = new();

        public async UniTask LoadSceneAsync(
            string sceneName,
            LoadSceneMode mode = LoadSceneMode.Single,
            bool setActive = true,
            Action<float>? onProgress = null
        )
        {
            if (IsSceneLoaded(sceneName)) return;

            try
            {
                var loadOp = SceneManager.LoadSceneAsync(sceneName, mode);
                loadOp.allowSceneActivation = false;

                while (!loadOp.isDone)
                {
                    float progress = Mathf.Clamp01(loadOp.progress / 0.9f);
                    onProgress?.Invoke(progress);

                    if (loadOp.progress >= 0.9f)
                    {
                        if (setActive)
                            loadOp.allowSceneActivation = true;
                    }

                    await UniTask.Yield(); 
                }

                if (mode == LoadSceneMode.Additive)
                    _loadedScenes.Add(sceneName);

                onProgress?.Invoke(1f); 
            }
            catch (Exception ex)
            {
                Logger.LogError($"[SceneLoader] Failed to load scene '{sceneName}': {ex}");
                onProgress?.Invoke(0f);
            }
        }


        public async UniTask UnloadSceneAsync(string sceneName)
        {
            if (!IsSceneLoaded(sceneName)) return;

            try
            {
                await SceneManager.UnloadSceneAsync(sceneName).ToUniTask();
                _loadedScenes.Remove(sceneName);
            }
            catch (Exception ex)
            {
                Logger.LogError($"[SceneLoader] Failed to unload scene '{sceneName}': {ex}");
            }
        }

        public bool IsSceneLoaded(string sceneName) =>
            _loadedScenes.Contains(sceneName) ||
            SceneManager.GetSceneByName(sceneName).isLoaded;
    }
}