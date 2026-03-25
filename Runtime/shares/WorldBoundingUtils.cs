//using System;
//using System.Collections;
//using System.Collections.Generic;
//using Unity.VisualScripting;
//using Unity.VisualScripting.YamlDotNet.Core;
//using UnityEngine;
//using unvs.ext;
//using unvs.interfaces;

//namespace unvs.shares
//{
   
//    public struct WorldBoundGameInfo
//    {
//        public Vector2[] Points;

//        public FacetInfo Left;
//        public FacetInfo Right;
//        public FacetInfo Top;
//        public FacetInfo Bottom;
//        public IScenePrefab Owner;
//        public bool IsClockWiseVertialCalculate
//        {
//            get
//            {
//                return Left.GetMax()>Right.GetMax();
//            }
//        }

        
//    }
//    public class WorldBoundingUtils
//    {
//        public static bool IsClockWise(Vector2[] points)
//        {
//            float sum = 0;
//            for (int i = 0; i < points.Length; i++)
//            {
//                Vector2 p1 = points[i];
//                Vector2 p2 = points[(i + 1) % points.Length];
//                sum += (p2.x - p1.x) * (p2.y + p1.y);
//            }
//            return sum > 0;
//        }
//        public static WorldBoundGameInfo Create(IScenePrefab scene)
//        {
//            var worldFacets = scene.JoinInfo.WorldJoinInfo.WorldFacets;
//            var ret=new WorldBoundGameInfo();
//            ret.Points = CloneToNew( scene.WorldBound.Coll.points, 
//                scene.WorldBound.Coll.transform.position,
//                scene.WorldBound.Coll.transform.rotation);
//            ret.Left = worldFacets.Left.CloneToNew();
//            ret.Right = worldFacets.Right.CloneToNew();
//            ret.Top=worldFacets.Top.CloneToNew();
//            ret.Bottom = worldFacets.Bottom.CloneToNew();
//            ret.Owner = scene;
//            //ret.IsClockWiseVertialCalculate = scene.JoinInfo.WorldJoinInfo.IsClockWiseVertialCalculate;
//            return ret;
//        }
//        /// <summary>
//        /// // --- 1. ĐI TOÀN BỘ CHU VI A (Nguoc chieu) ---
//        /// stepA=-1 co nghia la leftBound.Right.Start<leftBound.Right.End
//        /// stepA=-1 co nghia la cach danh thu tu index cua cac diem bi nguoc
//        /// va nguoc lai
//        /// int stepA = leftBound.Right.GetStep();
//        /// StepB=-1 co nghia la rightBound.Left.Start<rightBound.Left.End va nguoc lai
//        /// StepB=-1 co nghia la cach danh thu tu index cua cac diem bi nguoc
//        /// Cach noi 2 vong
//        /// tuy theo thu tu cac diem tren da giac la tuan hay ngich ma duyet
//        /// duyet vong A theo chieu thuan, duyet B theo chieu sao cho Start cua B noi tiep 
//        /// Voi cai End cua A
//        /// vi du:
//        /// A=[1,2,3,0] right=(start=1,end=2)<=> 2 diem (2,3) thuoc A
//        /// B=[3,4,1,0] left= (start=0, end=1)<=> 2 diem (3,4) thuoc B
//        /// Duyet A tu start :2->3->0->1 Duyet B tu End: 4->1-0-3 ==> KQ: (2->3->0->1) -> (4-1-0-3)
//        // 
//        /// </summary>
//        /// <param name="leftBound"></param>
//        /// <param name="rightBound"></param>
//        /// <returns></returns>
//        public static WorldBoundGameInfo JoinLeftRight_Fixed(WorldBoundGameInfo leftBound, WorldBoundGameInfo rightBound)
//        {
//            var A = leftBound.Points;
//            var B = rightBound.Points;

//            int aStart = leftBound.Right.Start;
//            int aEnd = leftBound.Right.End;
//            int bStart = rightBound.Left.Start;
//            int bEnd = rightBound.Left.End;

//            int aCount = A.Length;
//            int bCount = B.Length;

//            var result = new List<Vector2>(aCount + bCount);

//            // --- 1. DUYỆT KHỐI A (Lấy vỏ ngoài) ---
//            // Đi từ aStart, ngược hướng cạnh giáp lai để về aEnd
//            int stepA_Outer = -leftBound.Right.GetStep();
//            int currA = aStart;
//            while (true)
//            {
//                result.Add(A[currA]);
//                if (currA == aEnd) break;
//                currA = (currA + stepA_Outer + aCount) % aCount;
//            }

//            // --- 2. QUYẾT ĐỊNH CỔNG VÀO KHỐI B (Khử lỗi bắt chéo) ---
//            // Kiểm tra xem aEnd gần bStart hay bEnd hơn để chọn điểm bắt đầu nhảy sang
//            float distToBStart = Vector2.Distance(A[aEnd], B[bStart]);
//            float distToBEnd = Vector2.Distance(A[aEnd], B[bEnd]);

//            int entryB, exitB;
//            if (distToBStart < distToBEnd)
//            {
//                entryB = bStart;
//                exitB = bEnd;
//            }
//            else
//            {
//                entryB = bEnd;
//                exitB = bStart;
//            }

//            // --- 3. DUYỆT KHỐI B (Lấy vỏ ngoài) ---
//            // Tương tự A, ta đi từ entryB vòng qua lưng để về exitB
//            // Hướng đi phải là hướng "né" cạnh giáp lai (entryB -> exitB)
//            int stepB_Bridge = (exitB - entryB == 1 || exitB - entryB < -1) ? 1 : -1;
//            int stepB_Outer = -stepB_Bridge;

//            int currB = entryB;
//            while (true)
//            {
//                // Tránh trùng điểm tại mối nối
//                if (Vector2.Distance(result[result.Count - 1], B[currB]) > 0.001f)
//                    result.Add(B[currB]);

//                if (currB == exitB) break;
//                currB = (currB + stepB_Outer + bCount) % bCount;
//            }

//            // --- 4. ĐÓNG GÓI ---
//            var ret = new WorldBoundGameInfo();
//            ret.Points = result.ToArray();

//            // Tái cấu trúc Facet (Sử dụng hàm của bạn)
//            int s, e;
//            ret.Points.LeftVerticalFacet(out s, out e); ret.Left = new FacetInfo { Start = s, End = e };
//            ret.Points.RightVerticalFacet(out s, out e); ret.Right = new FacetInfo { Start = s, End = e };
//            ret.Points.TopHorizontalFacet(out s, out e); ret.Top = new FacetInfo { Start = s, End = e };
//            ret.Points.BottomHorizontalFacet(out s, out e); ret.Bottom = new FacetInfo { Start = s, End = e };

//            return ret;
//        }

//        private static Vector2[] CloneToNew(Vector2[] points, Vector3 position, Quaternion rotation)
//        {
//            var ret = new Vector2[points.Length];   
//            for(var i=0; i<points.Length; i++)
//            {
//                ret[i]= (Vector2)position + (Vector2)(rotation * points[i]);
//            }
//            return ret;
//        }

//        public static Vector2[] ClonePoint(PolygonCollider2D coll)
//        {
            
//            return CloneToNew(coll.points, coll.transform.position, coll.transform.rotation);
//        }
//    }
//}