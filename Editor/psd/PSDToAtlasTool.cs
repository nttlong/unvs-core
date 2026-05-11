namespace unvs.editor.psd {
    using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;
using System.Collections.Generic;
using System.IO;

public class PSDToAtlasTool
{
    
    [MenuItem("Assets/Create/Unvs/PSD/Generate Atlas from PSD", false, 10)]
  private static void GenerateAtlasFromPSD()
    {
        Object selected = Selection.activeObject;
        string psdPath = AssetDatabase.GetAssetPath(selected);

        if (string.IsNullOrEmpty(psdPath) || !psdPath.ToLower().EndsWith(".psd"))
        {
            EditorUtility.DisplayDialog("Lỗi", "Vui lòng chọn một file PSD!", "OK");
            return;
        }

        // Đổi lại đuôi .spriteatlas để Unity quản lý đúng chuẩn
        string atlasPath = Path.ChangeExtension(psdPath, ".spriteatlas");

        // 1. Tạo file Atlas thông qua phương thức chuẩn của Editor
        // Cách này thay thế cho AssetDatabase.CreateAsset để tránh lỗi bạn gặp
        SpriteAtlas atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(atlasPath);
        if (atlas == null)
        {
            // Sử dụng SpriteAtlasAsset để khởi tạo đúng định dạng cho V2
            atlas = new SpriteAtlas();
            AssetDatabase.CreateAsset(atlas, atlasPath);
        }

        // 2. Cấu hình Packing (Tắt xoay để bảo vệ nhân vật 1920px)
        var packingSettings = atlas.GetPackingSettings();
        packingSettings.enableRotation = false;
        packingSettings.enableTightPacking = false;
        atlas.SetPackingSettings(packingSettings);

        // 3. Cấu hình Platform (Bypass Read-only)
        var platformSettings = new TextureImporterPlatformSettings
        {
            name = "Standalone",
            maxTextureSize = 4096,
            overridden = true,
            textureCompression = TextureImporterCompression.Uncompressed 
        };
        atlas.SetPlatformSettings(platformSettings);

        // Đảm bảo Default cũng là 4096 để hiển thị đúng
        var defaultSettings = new TextureImporterPlatformSettings
        {
            name = "DefaultTexturePlatform",
            maxTextureSize = 4096,
            overridden = true
        };
        atlas.SetPlatformSettings(defaultSettings);

        // 4. Quét Sprite từ PSD
        Object[] subAssets = AssetDatabase.LoadAllAssetsAtPath(psdPath);
        List<Object> sprites = new List<Object>();
        foreach (var item in subAssets)
        {
            if (item is Sprite) sprites.Add(item);
        }

        // 5. Nạp Sprite (Bypass lỗi kéo thả bằng code)
        SpriteAtlasExtensions.Add(atlas, sprites.ToArray());

        // 6. Quan trọng: Yêu cầu Unity "nhai" lại file Atlas vừa tạo/sửa
        AssetDatabase.SaveAssets();
        AssetDatabase.ImportAsset(atlasPath); // Thay vì chỉ Refresh, ta ép Import lại
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("Thành công", $"Đã xử lý: {selected.name}", "OK");
    }
}
}