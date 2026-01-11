using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace YevheniiKostenko.CoreKit.Editor
{
    /// <summary>
    /// Captures a GameView screenshot to PNG via menu item and Ctrl+Shift+S shortcut.
    /// Uses Unity's internal GameView capture API so the output matches what you see in Game view.
    /// </summary>
    public static class ScreenshotTaker
    {
        private const string DefaultFolder = "Screenshots";

        // Ctrl + Shift + S
        [MenuItem("Tools/CoreKit/Screenshot/Take Screenshot %#s")]
        private static void TakeScreenshot()
        {
            var folder = GetOutputFolderAbsolute(DefaultFolder);
            Directory.CreateDirectory(folder);

            var fileName = $"Screenshot_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png";
            var fullPath = Path.Combine(folder, fileName);

            if (TryCaptureGameViewToPng(fullPath))
            {
                // Make the file appear in the Project window if saved under Assets, otherwise just reveal in Explorer.
                if (IsUnderProjectPath(fullPath))
                {
                    AssetDatabase.Refresh();
                    var assetPath = ToProjectRelativePath(fullPath);
                    var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);
                    if (asset != null)
                        EditorGUIUtility.PingObject(asset);
                }
                else
                {
                    EditorUtility.RevealInFinder(fullPath);
                }

                Debug.Log($"Screenshot saved: {fullPath}");
            }
            else
            {
                Debug.LogError("Failed to capture GameView screenshot. Ensure the Game view exists and Unity supports capture for this version.");
            }
        }

        private static string GetOutputFolderAbsolute(string folderName)
        {
            // Save under the project root (next to Assets) to avoid importing images unless desired.
            var projectRoot = Directory.GetParent(Application.dataPath)?.FullName ?? Application.dataPath;
            return Path.Combine(projectRoot, folderName);
        }

        private static bool TryCaptureGameViewToPng(string path)
        {
            try
            {
                // UnityEditor.GameView is internal; use reflection to call its static CaptureScreenshot method.
                // Signatures vary across Unity versions, so try common overloads.
                var editorAssembly = typeof(EditorWindow).Assembly;
                var gameViewType = editorAssembly.GetType("UnityEditor.GameView");
                if (gameViewType == null)
                    return false;

                // Prefer: CaptureScreenshot(string path)
                var method = gameViewType.GetMethod(
                    "CaptureScreenshot",
                    BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
                    binder: null,
                    types: new[] { typeof(string) },
                    modifiers: null);

                if (method != null)
                {
                    method.Invoke(null, new object[] { path });
                    return true;
                }

                // Fallback: CaptureScreenshot(string path, int superSize)
                method = gameViewType.GetMethod(
                    "CaptureScreenshot",
                    BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic,
                    binder: null,
                    types: new[] { typeof(string), typeof(int) },
                    modifiers: null);

                if (method != null)
                {
                    method.Invoke(null, new object[] { path, 1 });
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        private static bool IsUnderProjectPath(string fullPath)
        {
            var projectRoot = Directory.GetParent(Application.dataPath)?.FullName ?? Application.dataPath;
            return fullPath.Replace('\\', '/').StartsWith(projectRoot.Replace('\\', '/'), StringComparison.OrdinalIgnoreCase);
        }

        private static string ToProjectRelativePath(string fullPath)
        {
            var projectRoot = Directory.GetParent(Application.dataPath)?.FullName ?? Application.dataPath;
            var normalizedFull = fullPath.Replace('\\', '/');
            var normalizedRoot = projectRoot.Replace('\\', '/').TrimEnd('/');
            var rel = normalizedFull.Substring(normalizedRoot.Length).TrimStart('/');
            return rel;
        }
    }
}
