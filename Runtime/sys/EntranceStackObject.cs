//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using unvs.interfaces.teleport;

//namespace unvs.sys
//{
//    [Serializable]
//    public struct EntrancePointInfo
//    {
//        [SerializeField]
//        public Vector2 Point;

//        public string Path;
//    }

//    public static class EntranceStackObject
//    {
//        // Sử dụng Stack để lưu trữ theo cơ chế LIFO (Vào sau ra trước)
//        private static Stack<EntrancePointInfo> _stack = new Stack<EntrancePointInfo>();

//        public static void Push(EntrancePointInfo entrance)
//        {
//            _stack.Push(entrance);
//            Debug.Log($"[Entrance] Pushed point: {entrance.Point} at path: {entrance.Path}");
//        }

//        public static EntrancePointInfo Pop()
//        {
//            if (_stack.Count > 0)
//            {
//                return _stack.Pop();
//            }

//            Debug.LogWarning("[Entrance] Stack is empty!");
//            return default; // Trả về struct trống
//        }

//        public static int Count => _stack.Count;

//        public static void Clear() => _stack.Clear();
//    }
//}