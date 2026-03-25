using System.Collections;
using UnityEngine;

namespace unvs.interfaces
{
    public interface ICam
    {

        ICamCenterCamTracking Tracker { get; }

        Rigidbody2D Body { get; }
        void BoundByOrthoGraphicSize(PolygonCollider2D coll);
        void UpdateSizeByOrthoGraphicSize(float size, bool skipBoundSize = false);
    }
}