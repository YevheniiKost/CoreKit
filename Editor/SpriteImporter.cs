using UnityEditor;

namespace YeKostenko.CoreKit.Editor
{
    public class SpriteImporter : AssetPostprocessor
    {
        public const string MipmapKey = "CoreKit_SpriteImporter_Mipmap";
        public const string ImportModeKey = "CoreKit_SpriteImporter_ImportMode";
        public const string CompressionKey = "CoreKit_SpriteImporter_Compression";
        public const string TextureTypeKey = "CoreKit_SpriteImporter_TextureType";

        private void OnPreprocessTexture()
        {
            TextureImporter textureImporter = (TextureImporter)assetImporter;

            textureImporter.textureType = (TextureImporterType)EditorPrefs.GetInt(TextureTypeKey, (int)TextureImporterType.Sprite);
            textureImporter.spriteImportMode = (SpriteImportMode)EditorPrefs.GetInt(ImportModeKey, (int)SpriteImportMode.Single);
            textureImporter.mipmapEnabled = EditorPrefs.GetBool(MipmapKey, false);
            textureImporter.textureCompression = (TextureImporterCompression)EditorPrefs.GetInt(CompressionKey, (int)TextureImporterCompression.Uncompressed);
        }
    }
}