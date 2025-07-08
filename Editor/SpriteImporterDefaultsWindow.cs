using UnityEditor;
using UnityEngine;

namespace YeKostenko.CoreKit.Editor
{
    public class SpriteImporterDefaultsWindow : EditorWindow
    {
        private bool _mipmapEnabled;
        private SpriteImportMode _importMode;
        private TextureImporterCompression _compression;
        private TextureImporterType _textureType;

        [MenuItem("Tools/CoreKit/Sprite Importer Defaults")]
        public static void ShowWindow()
        {
            GetWindow<SpriteImporterDefaultsWindow>("Sprite Importer Defaults");
        }

        private void OnEnable()
        {
            _mipmapEnabled = EditorPrefs.GetBool(SpriteImporter.MipmapKey, false);
            _importMode = (SpriteImportMode)EditorPrefs.GetInt(SpriteImporter.ImportModeKey, (int)SpriteImportMode.Single);
            _compression = (TextureImporterCompression)EditorPrefs.GetInt(SpriteImporter.CompressionKey, (int)TextureImporterCompression.Uncompressed);
            _textureType = (TextureImporterType)EditorPrefs.GetInt(SpriteImporter.TextureTypeKey, (int)TextureImporterType.Sprite);
        }

        private void OnGUI()
        {
            GUILayout.Label("Default Sprite Import Settings", EditorStyles.boldLabel);

            _mipmapEnabled = EditorGUILayout.Toggle("Mip Maps Enabled", _mipmapEnabled);
            _importMode = (SpriteImportMode)EditorGUILayout.EnumPopup("Import Mode", _importMode);
            _compression = (TextureImporterCompression)EditorGUILayout.EnumPopup("Compression", _compression);
            _textureType = (TextureImporterType)EditorGUILayout.EnumPopup("Texture Type", _textureType);

            if (GUILayout.Button("Save Defaults"))
            {
                EditorPrefs.SetBool(SpriteImporter.MipmapKey, _mipmapEnabled);
                EditorPrefs.SetInt(SpriteImporter.ImportModeKey, (int)_importMode);
                EditorPrefs.SetInt(SpriteImporter.CompressionKey, (int)_compression);
                EditorPrefs.SetInt(SpriteImporter.TextureTypeKey, (int)_textureType);
                EditorUtility.DisplayDialog("Saved", "Default sprite import settings saved.", "OK");
            }
        }
    }
}