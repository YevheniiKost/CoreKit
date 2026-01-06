using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace YevheniiKostenko.CoreKit.Animation.Editor
{
    [CustomEditor(typeof(AnimSequence))]
    public class AnimSequenceEditor : UnityEditor.Editor
    {
        private SerializedProperty _stepsProp;
        private SerializedProperty _playParallelProp;
        private SerializedProperty _loopCountProp;
        private SerializedProperty _loopModeProp;

        private ReorderableList _list;

        private static readonly Type[] StepTypes = new Type[]
        {
            typeof(MoveStep),
            typeof(ScaleStep),
            typeof(RotateStep),
            typeof(FadeCanvasStep),
            typeof(WaitStep),
            typeof(CallEventStep)
        };

        private void OnEnable()
        {
            _stepsProp = serializedObject.FindProperty("_steps");
            _playParallelProp = serializedObject.FindProperty("_playParallel");
            _loopCountProp = serializedObject.FindProperty("_loopCount");
            _loopModeProp = serializedObject.FindProperty("_loopMode");

            _list = new ReorderableList(serializedObject, _stepsProp, true, true, true, true);
            _list.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Steps");
            _list.elementHeightCallback = index =>
            {
                SerializedProperty el = _stepsProp.GetArrayElementAtIndex(index);
                float h = EditorGUI.GetPropertyHeight(el, true) + 6f;
                return h;
            };
            _list.drawElementCallback = (rect, index, active, focused) =>
            {
                SerializedProperty el = _stepsProp.GetArrayElementAtIndex(index);
                rect.y += 2f;
                GUIContent label = new GUIContent(GetStepTitle(el));
                EditorGUI.PropertyField(rect, el, label, true);
            };
            _list.onAddDropdownCallback = (rect, l) =>
            {
                GenericMenu menu = new GenericMenu();
                foreach (Type t in StepTypes)
                {
                    string name = t.Name.Replace("Step", string.Empty);
                    menu.AddItem(new GUIContent(name), false, () => AddStepOfType(t));
                }

                menu.ShowAsContext();
            };
        }

        private void AddStepOfType(Type t)
        {
            serializedObject.Update();
            int i = _stepsProp.arraySize;
            _stepsProp.InsertArrayElementAtIndex(i);
            SerializedProperty el = _stepsProp.GetArrayElementAtIndex(i);
            el.managedReferenceValue = Activator.CreateInstance(t);
            serializedObject.ApplyModifiedProperties();
        }

        private string GetStepTitle(SerializedProperty prop)
        {
            if (prop == null)
            {
                return "Step";
            }

            AnimStep obj = prop.managedReferenceValue as AnimStep;
            if (obj != null)
            {
                return obj.DisplayName;
            }

            string full = prop.managedReferenceFullTypename;
            if (!string.IsNullOrEmpty(full))
            {
                int lastSpace = full.LastIndexOf(' ');
                if (lastSpace >= 0 && lastSpace < full.Length - 1)
                {
                    return full.Substring(lastSpace + 1);
                }

                return full;
            }

            return "Step";
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_playParallelProp);
            EditorGUILayout.PropertyField(_loopCountProp);
            EditorGUILayout.PropertyField(_loopModeProp);
            EditorGUILayout.Space();
            _list.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
        }
    }
}