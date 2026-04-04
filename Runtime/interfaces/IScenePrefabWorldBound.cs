using System.Collections;
using UnityEngine;
namespace unvs.interfaces
{
    /// <summary>
    /// Each Scene has its own a World-Bounding for Limitation of Camera movement
    /// Mỗi cảnh đều có giới hạn riêng về phạm vi di chuyển của máy quay.
    /// </summary>
    public interface IScenePrefabWorldBound
    {
        bool IsScale {  get; }
        PolygonCollider2D Coll { get; set; }
        //GameObject Owner { get; }
        PolygonCollider2D refColl { get; set; }
        IScenePrefab Owner { get; set; }
        GameObject TrOWner { get; }
        void Disable();
        /// <summary>
        /// World-binding of scene will be add to Composite Collider 2D of Virtual Camemra. 
        /// When Scene put behind (temp hidden), call this method for avoiding conflict
        /// Liên kết thế giới của cảnh sẽ được thêm vào Composite Collider 2D của Camera ảo. 
        /// Khi Cảnh được đặt phía sau (tạm thời ẩn), hãy gọi phương thức này, nhằm tránh xung đột
        /// </summary>
        void Restore();
        void DoScaleOnce(float scale);
    }
}
