using System;
using System.Collections;
using UnityEngine;

namespace unvs.shares
{
    [Serializable]
    public struct ChunkSceneRange
    {
        public BoxCollider2D Left { get; set; }
        public BoxCollider2D Right { get; set; }

        public bool IsEmpty()
        {
            return Left == null && Right == null;
        }

        public void UpdateLeft(BoxCollider2D leftWall)
        {
            Left = leftWall;
        }

        public void UpdateRight(BoxCollider2D rightWall)
        {
            Right = rightWall;
        }
    }
}