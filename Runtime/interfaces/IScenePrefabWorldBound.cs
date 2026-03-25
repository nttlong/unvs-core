using System.Collections;
using UnityEngine;
namespace unvs.interfaces
{
    public interface IScenePrefabWorldBound
    {
        bool IsScale {  get; }
        PolygonCollider2D Coll { get; set; }
        //GameObject Owner { get; }
        PolygonCollider2D refColl { get; set; }
        IScenePrefab Owner { get; set; }
        GameObject TrOWner { get; }
        void Disable();
        void Restore();
        void DoScaleOnce(float scale);
    }
}
