#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class SceneLauncher
{
    private const string SessionKeyTargetScene = "SceneLauncher_TargetScene";
    private const string SessionKeyPreviousScene = "SceneLauncher_PreviousScene";

    static SceneLauncher()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    [MenuItem("Tools/CoreKit/Launch Scene/Set Target Scene...")]
    public static void SelectTargetScene()
    {
        string path = EditorUtility.OpenFilePanel("Select Scene to Launch", "Assets", "unity");
        if (!string.IsNullOrEmpty(path))
        {
            if (path.StartsWith(Application.dataPath))
            {
                string relativePath = "Assets" + path.Substring(Application.dataPath.Length);
                SessionState.SetString(SessionKeyTargetScene, relativePath);
                Debug.Log($"[SceneLauncher] Target scene set to: {relativePath}");
            }
            else
            {
                Debug.LogWarning("Scene must be inside Assets folder.");
            }
        }
    }

    [MenuItem("Tools/CoreKit/Launch Scene/Launch Target Scene %#P")] // Ctrl+Shift+P
    public static void LaunchTargetScene()
    {
        string targetScene = SessionState.GetString(SessionKeyTargetScene, "");
        if (string.IsNullOrEmpty(targetScene))
        {
            Debug.LogWarning("Target scene is not set. Use Tools → Core Kit → Launch Scene → Set Target Scene...");
            return;
        }

        string currentScene = SceneManager.GetActiveScene().path;
        SessionState.SetString(SessionKeyPreviousScene, currentScene);

        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(targetScene);
            EditorApplication.isPlaying = true;
        }
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.EnteredEditMode)
        {
            string previousScene = SessionState.GetString(SessionKeyPreviousScene, "");
            if (!string.IsNullOrEmpty(previousScene))
            {
                EditorSceneManager.OpenScene(previousScene);
                SessionState.EraseString(SessionKeyPreviousScene);
            }
        }
    }
}
#endif
