using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace YeKostenko.CoreKit.Editor
{
    public class ProjectStructureToolWindow : EditorWindow
    {
        private const string CompanyKey = "CoreKit_ProjectStructure_Company";
        private const string ProjectKey = "CoreKit_ProjectStructure_Project";
        
        private string _companyName = "";
        private string _projectName = "";
        private List<string> _folders = new List<string> { "Scenes", "Scripts", "Prefabs", "Sprites", "Materials", "Resources", "Audio" };
        private Vector2 _scrollPos;
        
        [MenuItem("Tools/CoreKit/Create Project Structure")]
        public static void ShowWindow()
        {
            ProjectStructureToolWindow window = GetWindow<ProjectStructureToolWindow>("Create Project Structure");
            window.minSize = new Vector2(300, 400);
            window.maxSize = new Vector2(300, 400);
        }
        
        private void OnEnable()
        {
            _companyName = EditorPrefs.GetString(CompanyKey, "");
            _projectName = EditorPrefs.GetString(ProjectKey, "");
        }

        private void OnGUI()
        {
            GUILayout.Label("Project Structure Setup", EditorStyles.boldLabel);

            string newCompany = EditorGUILayout.TextField("Company Name", _companyName);
            if (newCompany != _companyName)
            {
                _companyName = newCompany;
                EditorPrefs.SetString(CompanyKey, _companyName);
            }

            string newProject = EditorGUILayout.TextField("Project Name", _projectName);
            if (newProject != _projectName)
            {
                _projectName = newProject;
                EditorPrefs.SetString(ProjectKey, _projectName);
            }

            GUILayout.Space(10);
            GUILayout.Label("Folders", EditorStyles.label);

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.Height(140));
            for (int i = 0; i < _folders.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                _folders[i] = EditorGUILayout.TextField(_folders[i]);
                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    _folders.RemoveAt(i);
                    i--;
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("Add Folder"))
            {
                _folders.Add("NewFolder");
            }

            GUILayout.Space(10);
            DrawCreateStructureButton();
            DrawDeleteStructureButton();
        }

        private void DrawCreateStructureButton()
        {
            GUI.enabled = !string.IsNullOrEmpty(_companyName) && !string.IsNullOrEmpty(_projectName);
            if (GUILayout.Button("Create Structure"))
            {
                string rootPath = Path.Combine("Assets", _companyName, _projectName);
                foreach (string folder in _folders)
                {
                    if (string.IsNullOrWhiteSpace(folder)) continue;
                    string fullPath = Path.Combine(rootPath, folder);
                    if (!Directory.Exists(fullPath))
                    {
                        Directory.CreateDirectory(fullPath);
                    }
                }
                AssetDatabase.Refresh();
                EditorUtility.DisplayDialog("Success", "Project structure created!", "OK");
            }
            GUI.enabled = true;
        }
        
        private void DrawDeleteStructureButton()
        {
            GUI.enabled = !string.IsNullOrEmpty(_companyName) && !string.IsNullOrEmpty(_projectName);
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("Delete Structure"))
            {
                string rootPath = Path.Combine("Assets", _companyName, _projectName);
                string companyPath = Path.Combine("Assets", _companyName);
                if (Directory.Exists(rootPath))
                {
                    bool hasNonEmpty = _folders.Any(folder =>
                    {
                        string fullPath = Path.Combine(rootPath, folder);
                        return Directory.Exists(fullPath) && Directory.EnumerateFileSystemEntries(fullPath).Any();
                    });

                    string message = hasNonEmpty
                        ? "Some folders are not empty. Are you sure you want to delete the entire project structure?"
                        : "Are you sure you want to delete the entire project structure?";

                    if (EditorUtility.DisplayDialog("Warning", message, "Delete", "Cancel"))
                    {
                        Directory.Delete(companyPath, true);
                        
                        string companyMetaPath = Path.Combine("Assets", _companyName + ".meta");
                        if (File.Exists(companyMetaPath))
                        {
                            File.Delete(companyMetaPath);
                        }
                        
                        AssetDatabase.Refresh();
                        EditorUtility.DisplayDialog("Deleted", "Project structure deleted.", "OK");
                    }
                }
                else
                {
                    EditorUtility.DisplayDialog("Info", "Project structure does not exist.", "OK");
                }
            }
            GUI.backgroundColor = Color.white;
            GUI.enabled = true;
        }
    }
}

