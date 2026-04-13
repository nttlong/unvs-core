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
        public static void AddSortingLayerIfNotExist(string name)
        {
            // 1. Load TagManager
            SerializedObject tagManager = new SerializedObject(
                AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]
            );

            // 2. Truy cập vào thuộc tính mảng "m_SortingLayers"
            SerializedProperty sortingLayersProp = tagManager.FindProperty("m_SortingLayers");

            // 3. Kiểm tra xem tên Layer này đã tồn tại trong mảng chưa
            bool exists = false;
            for (int i = 0; i < sortingLayersProp.arraySize; i++)
            {
                SerializedProperty entry = sortingLayersProp.GetArrayElementAtIndex(i);
                SerializedProperty nameProp = entry.FindPropertyRelative("name");

                if (nameProp.stringValue == name)
                {
                    exists = true;
                    break;
                }
            }

            // 4. Nếu chưa có thì mới tiến hành thêm mới
            if (!exists)
            {
                int newIndex = sortingLayersProp.arraySize;
                sortingLayersProp.InsertArrayElementAtIndex(newIndex);

                SerializedProperty newLayer = sortingLayersProp.GetArrayElementAtIndex(newIndex);

                // Thiết lập tên cho Sorting Layer mới
                newLayer.FindPropertyRelative("name").stringValue = name;

                // Thiết lập ID duy nhất (Unity yêu cầu ID để quản lý nội bộ)
                // Sử dụng hàm băm từ tên hoặc lấy giá trị ngẫu nhiên độc nhất
                newLayer.FindPropertyRelative("uniqueID").intValue = name.GetHashCode();

                // 5. Lưu thay đổi
                tagManager.ApplyModifiedProperties();
                AssetDatabase.SaveAssets();

                Debug.Log($"<color=cyan>SortingLayer:</color> Đã tạo mới '{name}'.");
            }
        }
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