using System.IO;
using System.Text;
using UnityEditor;

public class UTF8EncodingFix : AssetPostprocessor
{
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
                                       string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string assetPath in importedAssets)
        {
            if (assetPath.EndsWith(".cs"))
            {
                string fullPath = Path.Combine(Directory.GetCurrentDirectory(), assetPath);
                string content = File.ReadAllText(fullPath, Encoding.Default);

                // 이미 UTF-8 BOM이 있다면 스킵
                if (!content.StartsWith("\uFEFF"))
                {
                    File.WriteAllText(fullPath, content, new UTF8Encoding(true)); // true = BOM 포함
                    UnityEngine.Debug.Log($"Converted {assetPath} to UTF-8 with BOM");
                }
            }
        }
    }
}