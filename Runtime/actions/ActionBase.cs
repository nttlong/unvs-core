//using Assets.Prefabs.InteractRequire;

using Cysharp.Threading.Tasks;
using NUnit.Framework.Constraints;
using PlasticPipe.PlasticProtocol.Messages;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using unvs.ext;

namespace unvs.actions
{
    public struct ActionBaseSender
    {
        public MonoBehaviour Source;
        public MonoBehaviour Target;
        public bool IsCancel;
        public CancellationTokenSource Cts;

        public void Cancel()
        {
            IsCancel=true;
            this.Cts?.Cancel();
        }

        public T GetTargetComponent<T>()
        {
            if(Target == null) return default(T);
            return Target.GetComponent<T>();
        }
        public T GetSourceComponent<T>()
        {
            if(Source == null) return default(T);
            return Source.GetComponent<T>();
        }
        public Vector2 GetSourceCenterPoint()
        {
            if (Source == null) return Vector2.negativeInfinity;
            var coll= Source.GetComponent<Collider2D>();  
            if(coll==null) return Vector2.negativeInfinity;
            return coll.bounds.center;
        }
        public Vector2 GetTargetCenterPoint()
        {
            if (Target == null) return Vector2.negativeInfinity;
            var coll = Target.GetComponent<Collider2D>();
            if (coll == null) return Vector2.negativeInfinity;
            return coll.bounds.center;
        }
        public Vector2 FromTargetToSourceDirection()
        {
            var a= GetTargetComponent<Vector2>();
            var b= GetSourceComponent<Vector2>();
            return (b-a).CalculateDiection();
        }
        public Vector2 FromSourceToTargetDirection()
        {
            var a = GetTargetComponent<Vector2>();
            var b = GetSourceComponent<Vector2>();
            return (a - b).CalculateDiection();
        }
    }
    [Serializable]
    public abstract class ActionBase // Chuyển thành abstract
    {


        public string name;

        public abstract UniTask ExecuteAsync(ActionBaseSender Sender);
    }
}