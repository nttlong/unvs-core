#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

// Class chứa thông tin Layer đơn giản để bạn dễ quản lý
public class LayerInfo
{
    public string layerName;
    public bool isVisible;
    public string layerGroup; // Tên của Group chứa layer này
}

public class PsdHelper
{
    /// <summary>
    /// Hàm 1: Lấy danh sách tất cả các layer (bao gồm thông tin Group và ẩn/hiện)
    /// </summary>
    public static List<LayerInfo> GetLayerList(string psdPath)
    {
        List<LayerInfo> results = new List<LayerInfo>();
        List<string> fullNames = PsdLoader.GetLayerNames(psdPath);

        foreach (var fullName in fullNames)
        {
            // Tách tên layer và group từ đường dẫn Group/LayerName
            string[] parts = fullName.Split('/');
            results.Add(new LayerInfo
            {
                layerName = fullName, // Sử dụng toàn bộ đường dẫn để load chính xác
                isVisible = true,      // Mặc định true vì parser thô có thể chưa lấy được view state
                layerGroup = parts.Length > 1 ? parts[parts.Length - 2] : "Root"
            });
        }
        return results;
    }

    /// <summary>
    /// Hàm 2: Nhận vào đường dẫn và tên Layer, trả về Texture2D (Bitmap trong Unity)
    /// </summary>
    public static Texture2D GetLayerBitmap(string psdPath, string targetLayerName)
    {
        return PsdLoader.LoadLayerBitmap(psdPath, targetLayerName);
    }
}
#endif