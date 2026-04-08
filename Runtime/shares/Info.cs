using System;
using System.Collections;
using UnityEngine;
namespace unvs.shares
{
    /// <summary>
    /// This struct will gold index of point of an edge
    /// </summary>
    [Serializable]
    public class FacetInfo
    {
        [SerializeField]
        public int Start;
        [SerializeField]
        public int End;
    }
    [Serializable]
    public class WorldBoundFacets
    {
        [SerializeField]
        public FacetInfo Top;
        [SerializeField]
        public FacetInfo Bottom;
        [SerializeField]
        public FacetInfo Left;
        [SerializeField]
        public FacetInfo Right;

        
    }

    [Serializable]
    public class Egde2d
    {
        [SerializeField]
        public Vector2 Start;
        [SerializeField]
        public Vector2 End;
    }
    
    [Serializable]
    public class WorldJoinInfo
    {
        /// <summary>
        /// danh sach cac canh
        /// </summary>
        [SerializeField]
        public Egde2d[] Poly;
        /// <summary>
        /// Chi muc canh 1 cat voi x1
        /// </summary>
        
        public int LeftGroundIndex;
        /// <summary>
        /// Chi muc canh 2 cat voi x2
        /// </summary>
        public int RightGroundIndex;
        /// <summary>
        /// Diem cat voi x1
        /// </summary>
        [SerializeField]
        public Vector2 LeftPos;
        /// <summary>
        /// Diem cat voi x2
        /// </summary>
        [SerializeField]
        public Vector2 RightPos;
        [SerializeField]
        public Vector2 Center;
        [SerializeField]
        public Vector2 Min;
        [SerializeField]
        public Vector2 Max;
        [SerializeField]
        public WorldBoundFacets WorldFacets;
        [SerializeField]
        public Vector2 Size;
       

        public void Move(Vector2 OffsetPos)
        {
            RightPos += OffsetPos;
            LeftPos += OffsetPos;
            Center+= OffsetPos;
            Min += OffsetPos;
            Max += OffsetPos;
        }
    }
}