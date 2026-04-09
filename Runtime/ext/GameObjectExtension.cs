using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using unvs.shares;

namespace unvs.ext
{
    public static class GameObjectExtension
    {
        public static void HideWhenPlaying(this GameObject obj)
        {
            // Kiểm tra xem có phải đang thực sự "chạy" game hay không
            if (Application.isPlaying)
            {
                obj.SetActive(false); // Ẩn khi đang chơi
            }
            else
            {
                obj.SetActive(true); // Hiện khi đang edit (cần [ExecuteAlways])
            }
        }
        /// <summary>
        /// Hủy đối tượng an toàn: Ưu tiên Addressables Release, sau đó mới Destroy.
        /// Quét sạch các con được gắn vào lúc Runtime để tránh Memory Leak.
        /// </summary>
        public static void SafeDestroy(this GameObject target, Action<GameObject> Before = null)
        {
            if (target == null) return;

            // 1. Duyệt ngược từ dưới lên các con để giải phóng Addressables con trước
            // (Rất quan trọng nếu bạn SetParent các Addressable khác vào GO này)
            for (int i = target.transform.childCount - 1; i >= 0; i--)
            {
                GameObject child = target.transform.GetChild(i).gameObject;

                // Đệ quy để đảm bảo các con của con cũng được kiểm tra
                SafeDestroy(child, Before);
            }

            // 2. Thử giải phóng đối tượng hiện tại bằng Addressables
            // Hàm ReleaseInstance sẽ trả về true nếu GO này là một Addressable Instance
            bool isAddressable = Addressables.ReleaseInstance(target);

            // 3. Nếu không phải Addressable (hàng Instantiate thường hoặc đã bị Release)
            // thì mới dùng lệnh Destroy vật lý của Unity
            if (!isAddressable)
            {
                Before?.Invoke(target);
                UnityEngine.Object.Destroy(target);
            }
        }
        public static async UniTask SafeDestroyChildrenAsync(this Transform target, Action<GameObject> Before = null)
        {
            await target.gameObject.SafeDestroyChildrenAsync(Before);
        }
        public static async UniTask SafeDestroyChildrenAsync(this GameObject target, Action<GameObject> Before = null)
        {
            if (target == null) return;

            int childCount = target.transform.childCount;
            if (childCount == 0) return;

            // 1. Thu thập tất cả các GameObject con trực tiếp
            GameObject[] children = new GameObject[childCount];
            for (int i = 0; i < childCount; i++)
            {
                children[i] = target.transform.GetChild(i).gameObject;
            }

            // 2. Tạo danh sách các Task để tiêu diệt từng đứa con
            // Ở đây ta gọi lại hàm SafeDestroyAsync (hàm đã có ở bước trước) 
            // để mỗi đứa con tự lo liệu việc giải phóng Addressables của chính nó và con của nó.
            UniTask[] destroyTasks = new UniTask[childCount];
            for (int i = 0; i < childCount; i++)
            {
                destroyTasks[i] = children[i].SafeDestroyAsync(Before);
            }

            // 3. Đợi tất cả các con được giải phóng hoàn toàn
            await UniTask.WhenAll(destroyTasks);

            // Lưu ý: Không có bước Addressables.ReleaseInstance(target) ở đây 
            // vì chúng ta chỉ muốn dọn dẹp bên trong.
        }
        public static async UniTask SafeDestroyAsync(this Transform target, Action<GameObject> Before = null)
        {
            await target.gameObject.SafeDestroyAsync(Before);
        }
        public static async UniTask SafeDestroyAsync(this GameObject target,Action<GameObject> Before=null)
        {
            if (target == null) return;

            // 1. Duyệt ngược và giải phóng các con trước
            // Sử dụng UniTask.WhenAll để chạy song song việc giải phóng các con, giúp tối ưu thời gian
            int childCount = target.transform.childCount;
            if (childCount > 0)
            {
                UniTask[] childTasks = new UniTask[childCount];
                for (int i = childCount - 1; i >= 0; i--)
                {
                    Before?.Invoke(target.transform.GetChild(i).gameObject);
                    childTasks[i] = SafeDestroyAsync(target.transform.GetChild(i).gameObject, Before);
                }
                await UniTask.WhenAll(childTasks);
            }

            // 2. Thử giải phóng bằng Addressables
            // Addressables.ReleaseInstance thực chất không có bản Async trực tiếp trả về bool,
            // nhưng ta có thể bọc nó để đảm bảo an toàn hoặc xử lý các logic chờ đợi nếu cần.
            bool isAddressable = Addressables.ReleaseInstance(target);

            // 3. Nếu không phải Addressable thì Destroy vật lý
            if (!isAddressable)
            {
                Before?.Invoke(target);
                UnityEngine. Object.Destroy(target);

                // Đợi 1 frame để đảm bảo Object đã thực sự bị xóa khỏi bộ nhớ Unity trước khi đi tiếp
                await UniTask.Yield(PlayerLoopTiming.Update);
            }
        }
        public static T GetComponentInChildrenByName<T>(this GameObject obj, string Name, bool includeInactive = true) 
        {
            // Cách này nhanh hơn: Tìm tất cả component T trong các con trước
            var components = obj.GetComponentsInChildren<Transform>(includeInactive).Where(p => !p.gameObject.IsDestroyed());

            foreach (var comp in components)
            {
                // Kiểm tra tên (bỏ qua hoa thường và có thể Trim khoảng trắng nếu cần)
                if (comp.gameObject.name.Equals(Name, StringComparison.OrdinalIgnoreCase) && !comp.gameObject.IsDestroyed())
                {
                    var ret = comp.GetComponent<T>();
                    if (ret != null) return ret;
                    return default(T);
                }
            }

            return default(T);
        }
        public static void CloneNewObject(this GameObject go, Canvas canvas, Vector2 pos, string name)
        {
            Commons.CreateClone(canvas, go, pos);
        }

        public static void ApplyNavigate<T>(this GameObject gameObject) where T : Component
        {
            EventSystem.current.SetSelectedGameObject(null);
            var firstElement= gameObject.GetComponentInChildren<T>();
            EventSystem.current.SetSelectedGameObject(firstElement.gameObject);
        }
        public static void ApplyNavigate<T>(this MonoBehaviour gameObject) where T : Component
        {
            EventSystem.current.SetSelectedGameObject(null);
            var firstElement = gameObject.GetComponentInChildren<T>();
            if(firstElement!=null) 
            EventSystem.current.SetSelectedGameObject(firstElement.gameObject);
        }
    }
}