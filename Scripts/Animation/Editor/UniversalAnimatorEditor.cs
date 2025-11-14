using UnityEditor;
using UnityEngine;

namespace YevheniiKostenko.CoreKit.Animation.Editor
{
    [CustomEditor(typeof(UniversalAnimator))]
    public class UniversalAnimatorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Runtime Controls", EditorStyles.boldLabel);
            UniversalAnimator ua = (UniversalAnimator)target;
            using (new EditorGUI.DisabledScope(!Application.isPlaying))
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Play Show"))
                {
                    ua.PlayShow();
                }
                if (GUILayout.Button("Play Hide"))
                {
                    ua.PlayHide();
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Play Idle"))
                {
                    ua.PlayIdle();
                }
                if (GUILayout.Button("Stop Idle"))
                {
                    ua.StopIdle();
                }
                EditorGUILayout.EndHorizontal();
            }
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Enter Play Mode to use runtime controls.", MessageType.Info);
            }
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("State", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Idle Active", ua.IsIdleActive ? "Yes" : "No");
            EditorGUILayout.LabelField("Driver", ua.DriverChoiceSetting.ToString());
        }
    }
}