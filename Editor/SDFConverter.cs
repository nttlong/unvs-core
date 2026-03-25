#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class SDFConverter : Editor
{
    private static readonly string[] SupportedExtensions = { ".psd", ".png", ".jpg", ".jpeg", ".tga", ".tiff" };

    [MenuItem("Assets/Convert to SDF", false, 1)]
    private static void SDFConverterMenu()
    {
        Object[] selectedObjects = Selection.objects;

        foreach (Object obj in selectedObjects)
        {
            string assetPath = AssetDatabase.GetAssetPath(obj);
            if (string.IsNullOrEmpty(assetPath)) continue;

            string extension = Path.GetExtension(assetPath).ToLower();
            if (SupportedExtensions.Contains(extension))
            {
                Debug.Log($"Converting {assetPath} to SDF...");

                if (extension == ".psd")
                {
                    // Chạy class PSD_SDFConvertFile (bản đã refactor)
                    var psdConverter = new PSD_SDFConvertFile();
                    psdConverter.Convert(assetPath);
                }
                else
                {
                    // Xử lý các định dạng ảnh chuẩn bằng Texture2D
                    // Buộc Unity load texture này với quyền Read/Write tạm thời
                    TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                    bool oldIsReadable = importer.isReadable;

                    if (!oldIsReadable)
                    {
                        importer.isReadable = true;
                        importer.SaveAndReimport();
                    }

                    Texture2D sourceTex = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
                    if (sourceTex != null)
                    {
                        // Sử dụng class tính toán SDF đã Unity-hóa
                        Texture2D sdfTex = Texture_ConvertTo_SDF_JFA.ConvertToMultiShapeSDF(sourceTex);

                        string directory = Path.GetDirectoryName(assetPath);
                        string fileName = Path.GetFileNameWithoutExtension(assetPath);
                        string savePath = Path.Combine(directory, $"{fileName}_sdf.png");

                        byte[] bytes = sdfTex.EncodeToPNG();
                        File.WriteAllBytes(savePath, bytes);

                        Debug.Log($"[SDFConverter] Generated SDF for image: {savePath}");

                        // Dọn dẹp
                        Object.DestroyImmediate(sdfTex);
                    }

                    // Trả lại trạng thái Read/Write cũ để tiết kiệm bộ nhớ game
                    if (!oldIsReadable)
                    {
                        importer.isReadable = false;
                        importer.SaveAndReimport();
                    }
                }
            }
        }

        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/Convert to SDF", true)]
    private static bool ValidateConvertToSFD()
    {
        Object[] selectedObjects = Selection.objects;
        if (selectedObjects == null || selectedObjects.Length == 0) return false;

        return selectedObjects.Any(obj =>
        {
            string path = AssetDatabase.GetAssetPath(obj);
            return !string.IsNullOrEmpty(path) && SupportedExtensions.Contains(Path.GetExtension(path).ToLower());
        });
    }
}
#endif