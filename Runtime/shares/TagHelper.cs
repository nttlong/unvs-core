namespace unvs.shares
{
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEngine;

    public static class TagHelper
    {
        public static void AddTag(string tagName)
        {
            // 1. Load TagManager
            SerializedObject tagManager = new SerializedObject(
                AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]
            );
            SerializedProperty tagsProp = tagManager.FindProperty("tags");

            // 2. Kiểm tra xem tag đã tồn tại chưa
            bool found = false;
            for (int i = 0; i < tagsProp.arraySize; i++)
            {
                if (tagsProp.GetArrayElementAtIndex(i).stringValue == tagName)
                {
                    found = true;
                    break;
                }
            }

            // 3. Nếu chưa có thì mới thêm
            if (!found)
            {
                int index = tagsProp.arraySize;
                tagsProp.InsertArrayElementAtIndex(index); // Lưu ý: Chữ I viết hoa -> InsertArrayElementAtIndex

                // Hoặc dùng cách hiện đại hơn ở các bản Unity mới:
                // tagsProp.arraySize++; 

                SerializedProperty newTag = tagsProp.GetArrayElementAtIndex(index);
                newTag.stringValue = tagName;

                // 4. Lưu lại thay đổi vào file .asset
                tagManager.ApplyModifiedProperties();
                AssetDatabase.SaveAssets();
            }
        }
    }


    public static class LayerHelper
    {
        public static void AddLayer(string layerName)
        {
            // 1. Mở TagManager.asset
            SerializedObject tagManager = new SerializedObject(
                AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]
            );
            SerializedProperty layersProp = tagManager.FindProperty("layers");

            // 2. Kiểm tra xem layer đã tồn tại chưa
            for (int i = 0; i < layersProp.arraySize; i++)
            {
                SerializedProperty layerEntry = layersProp.GetArrayElementAtIndex(i);
                if (layerEntry.stringValue == layerName) return; // Đã tồn tại
            }

            // 3. Tìm slot trống (Layer từ 8 đến 31 là slot người dùng có thể sửa)
            bool foundSlot = false;
            for (int i = 8; i <= 31; i++)
            {
                SerializedProperty layerEntry = layersProp.GetArrayElementAtIndex(i);
                if (string.IsNullOrEmpty(layerEntry.stringValue))
                {
                    layerEntry.stringValue = layerName;
                    foundSlot = true;
                    break;
                }
            }

            if (foundSlot)
            {
                tagManager.ApplyModifiedProperties();
                AssetDatabase.SaveAssets();
                Debug.Log($"<color=green>Success:</color> Đã thêm Layer '{layerName}' vào hệ thống.");
            }
            else
            {
                Debug.LogError("Failure: Không còn slot trống để thêm Layer mới (Max 31).");
            }
        }
    }
#endif
}