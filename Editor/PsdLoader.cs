#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Ntreev.Library.Psd;

public static class PsdLoader
{
    // Hàm 1: Lấy danh sách layer (Hỗ trợ đệ quy để lấy sạch layer trong Group)
    public static List<string> GetLayerNames(string path)
    {
        if (!File.Exists(path)) return new List<string>();

        using (var stream = File.OpenRead(path))
        {
            var document = PsdDocument.Create(stream);
            List<string> layerNames = new List<string>();

            // Bắt đầu duyệt đệ quy từ các con của document
            foreach (var child in document.Childs)
            {
                ExtractLayerNames(child, "", layerNames);
            }
            return layerNames;
        }
    }

    private static void ExtractLayerNames(IPsdLayer layer, string parentPath, List<string> names)
    {
        // Tạo đường dẫn hiển thị: Group/LayerName
        string currentPath = string.IsNullOrEmpty(parentPath) ? layer.Name : $"{parentPath}/{layer.Name}";

        // Nếu không phải là Group (không có con hoặc là layer chứa pixel), thêm vào danh sách
        // Hoặc bạn có thể thêm tất cả để hiển thị cả tên Group
        names.Add(currentPath);

        // Đệ quy vào các con nếu có
        foreach (var child in layer.Childs)
        {
            ExtractLayerNames(child, currentPath, names);
        }
    }

    // Hàm 2: Lấy Bitmap chính xác từ tên Layer (Duyệt đệ quy để tìm)
    public static Texture2D LoadLayerBitmap(string path, string targetFullName)
    {
        using (var stream = File.OpenRead(path))
        {
            var document = PsdDocument.Create(stream);
            IPsdLayer foundLayer = null;

            // Tìm layer bằng cách duyệt đệ quy
            foreach (var child in document.Childs)
            {
                foundLayer = FindLayerRecursive(child, "", targetFullName);
                if (foundLayer != null) break;
            }

            if (foundLayer == null || foundLayer.Width <= 0 || foundLayer.Height <= 0)
            {
                Debug.LogWarning("Layer không tìm thấy hoặc không có dữ liệu hình ảnh.");
                return null;
            }

            return ConvertPsdLayerToTexture(foundLayer);
        }
    }

    private static IPsdLayer FindLayerRecursive(IPsdLayer layer, string parentPath, string targetName)
    {
        string currentPath = string.IsNullOrEmpty(parentPath) ? layer.Name : $"{parentPath}/{layer.Name}";

        if (currentPath == targetName || layer.Name == targetName)
            return layer;

        foreach (var child in childs(layer))
        {
            var found = FindLayerRecursive(child, currentPath, targetName);
            if (found != null) return found;
        }
        return null;
    }

    // Helper because layer.Childs might be problematic to iterate if not wrapped correctly sometimes, 
    // but here it was already working. Just keeping it simple.
    private static IEnumerable<IPsdLayer> childs(IPsdLayer layer)
    {
        foreach (var child in layer.Childs) yield return child;
    }

    private static Texture2D ConvertPsdLayerToTexture(IPsdLayer layer)
    {
        int w = layer.Width;
        int h = layer.Height;

        // Khởi tạo các mảng chứa dữ liệu kênh màu
        byte[] rData = null, gData = null, bData = null, aData = null;

        // Duyệt qua mảng Channels để tìm đúng loại kênh màu
        foreach (var channel in layer.Channels)
        {
            // ChannelType thường là enum: Red = 0, Green = 1, Blue = 2, Alpha = -1
            switch ((int)channel.Type)
            {
                case 0: rData = channel.Data; break;
                case 1: gData = channel.Data; break;
                case 2: bData = channel.Data; break;
                case -1: aData = channel.Data; break;
            }
        }

        // Kiểm tra nếu thiếu các kênh màu cơ bản (R, G, B)
        if (rData == null || gData == null || bData == null)
        {
            Debug.LogWarning($"Layer {layer.Name} thiếu dữ liệu kênh màu cơ bản.");
            return null;
        }

        Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        Color32[] pixels = new Color32[w * h];

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                int psdIdx = y * w + x;
                int unityIdx = (h - 1 - y) * w + x; // Lật ngược trục Y

                pixels[unityIdx] = new Color32(
                    rData[psdIdx],
                    gData[psdIdx],
                    bData[psdIdx],
                    aData != null ? aData[psdIdx] : (byte)255
                );
            }
        }

        tex.SetPixels32(pixels);
        tex.Apply();
        return tex;
    }
}
#endif