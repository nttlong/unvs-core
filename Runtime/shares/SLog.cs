using System.Collections;
using System.Diagnostics;
using UnityEngine;
namespace unvs.shares
{
    public static class SLog
    {
        // Chỉ chạy trong Editor hoặc bản Build dùng để Test (Development Build)
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        [System.Diagnostics.DebuggerHidden]

        public static void Info(object message)
        {
            UnityEngine.Debug.Log($"{message}");
        }

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        [System.Diagnostics.DebuggerHidden]
        public static void Warning(object message, UnityEngine.Object context = null)
        {
            UnityEngine.Debug.LogWarning($"{message}", context);
        }

        // Riêng lỗi (Error) thường nên để lại kể cả khi Build để dễ trace lỗi từ người chơi

        [System.Diagnostics.DebuggerHidden]
        public static void Error(object message, UnityEngine.Object context = null)
        {
            UnityEngine.Debug.LogError($"{message}", context);
        }
    }
}