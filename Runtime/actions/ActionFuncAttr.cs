using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;

using UnityEngine;
namespace unvs.actions
{
    // Dùng cho Method
    [AttributeUsage(AttributeTargets.Method)]
    public class ActionFuncAttr : Attribute { }
    [System.Serializable]
    public class ActionEntry
    {
        // Biến này để chạy thực tế khi Build (lưu tên Class, ví dụ: "SaySomthing")
        public string scriptClassName;
        public string functionName;

#if UNITY_EDITOR
        // Biến này chỉ tồn tại trong Editor để bạn kéo thả file .cs
        // Nó sẽ không bị tính vào bản Build
        public UnityEditor.MonoScript targetScript;
#endif
    }
    public struct ActionSender
    {
        public MonoBehaviour Source;
        public MonoBehaviour Target;
        public CancellationTokenSource Cts;

        public bool Ok;

        
        public ActionSender Cancel()
        {
            this.Cts?.Cancel();
            // _cts?.Dispose();
            this.Cts = new CancellationTokenSource();
            this.Ok = false;
            return this;
        }
    }
    public abstract class ActionsObjectBase : MonoBehaviour
    {
        public List<ActionEntry> actions = new List<ActionEntry>();

        public virtual Vector2 GetPosition(float direction=0) {
            Collider2D coll = GetComponent<Collider2D>();
            if(coll==null) return Vector2.zero;
            return new Vector2(coll.bounds.center.x, 0);
        }
       
    }
}