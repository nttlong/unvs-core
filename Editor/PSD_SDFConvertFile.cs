#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class PSD_SDFConvertFile 
{
    const string SDFLayer = "sdf";

    public void Convert(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError($"[PSD_SDFConvertFile] File not found: {path}");
            return;
        }

        string directory = Path.GetDirectoryName(path);
        string psdFileName = Path.GetFileName(path);

        var layers = PsdHelper.GetLayerList(path);

        foreach (var layer in layers)
        {
            bool isSdfLayer = (layer.layerName != null && layer.layerName.ToLower().Contains($".{SDFLayer}")) ||
                              (layer.layerGroup != null && layer.layerGroup.ToLower().Contains(SDFLayer));

            if (isSdfLayer)
            {
                // 1. Lấy trực tiếp Texture2D (Không dùng Bitmap nữa)
                Texture2D texture = PsdHelper.GetLayerBitmap(path, layer.layerName);
                if (texture == null) continue;

                // 2. Gọi hàm Convert mới sử dụng Texture2D
                // Bạn cần cập nhật hàm ConvertToMultiShapeSDF trong class kia (xem mục 2 bên dưới)
                Texture2D sdfTexture = Texture_ConvertTo_SDF_JFA.ConvertToMultiShapeSDFOld(texture);

                // 3. Lưu file bằng API của Unity
                string safeLayerName = layer.layerName.Replace("/", "_").Replace("\\", "_");
                string outputFileName = $"{psdFileName}.{safeLayerName}.sfd.png";
                string fullOutputPath = Path.Combine(directory, outputFileName);

                byte[] pngData = sdfTexture.EncodeToPNG();
                File.WriteAllBytes(fullOutputPath, pngData);

                Debug.Log($"[PSD_SDFConvertFile] Generated SDF: {fullOutputPath}");

                // Giải phóng bộ nhớ
                Object.DestroyImmediate(texture);
                Object.DestroyImmediate(sdfTexture);
            }
        }

        AssetDatabase.Refresh();
    }
}
#endif