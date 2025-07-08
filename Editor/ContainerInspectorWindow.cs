using UnityEditor;
using UnityEngine;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace YeKostenko.CoreKit.DI.Editor
{
    public class ContainerInspectorWindow : EditorWindow
    {
        private Vector2 _scroll;
        private string _search = "";
        private Container _selectedContainer;

        [MenuItem("Tools/CoreKit/DI Containers")]
        public static void ShowWindow()
        {
            GetWindow<ContainerInspectorWindow>("DI Containers");
        }

        private void OnGUI()
        {
            GUILayout.Label("Active Containers", EditorStyles.boldLabel);
            _scroll = GUILayout.BeginScrollView(_scroll, GUILayout.Height(120));

            foreach (var container in ContainerRegistry.All)
            {
                string label = container.Label;
                if (GUILayout.Button(label,
                        _selectedContainer == container ? EditorStyles.toolbarButton : GUI.skin.button))
                {
                    _selectedContainer = container;
                }
            }

            GUILayout.EndScrollView();

            if (_selectedContainer != null)
            {
                GUILayout.Space(10);
                GUILayout.Label($"Inspecting: {_selectedContainer.Label}", EditorStyles.boldLabel);

                GUILayout.BeginHorizontal();
                GUILayout.Label("Search", GUILayout.Width(50));
                _search = GUILayout.TextField(_search);
                GUILayout.EndHorizontal();

                DrawContainerBindings(_selectedContainer);
            }
        }

        private void DrawContainerBindings(Container container)
        {
            var bindingsField = typeof(Container)
                .GetField("bindings", BindingFlags.Instance | BindingFlags.NonPublic);

            if (bindingsField == null)
            {
                EditorGUILayout.HelpBox("Can't access bindings field", MessageType.Error);
                return;
            }

            var bindings = bindingsField.GetValue(container) as Dictionary<Type, object>;
            if (bindings == null) return;

            foreach (var kvp in bindings.OrderBy(k => k.Key.FullName))
            {
                var type = kvp.Key;
                if (!string.IsNullOrEmpty(_search) && !type.Name.ToLower().Contains(_search.ToLower()))
                    continue;

                var binding = kvp.Value;
                var infoType = binding.GetType();
                var impl = (Type)infoType.GetField("ImplementationType")?.GetValue(binding);
                var lifeTime = (LifeTime)infoType.GetField("LifeTime")?.GetValue(binding);
                var instance = infoType.GetField("SingletonInstance")?.GetValue(binding)
                               ?? infoType.GetField("FixedInstance")?.GetValue(binding);

                GUILayout.BeginVertical("box");
                GUILayout.Label($"Type: {type.FullName}", EditorStyles.boldLabel);
                GUILayout.Label($"Impl: {impl?.FullName ?? "Instance"}");
                GUILayout.Label($"LifeTime: {lifeTime}");
                GUILayout.Label($"Resolved: {(instance != null ? "Yes" : "No")}");
                GUILayout.EndVertical();
            }
        }
    }
}