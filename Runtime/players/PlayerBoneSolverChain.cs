using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.U2D.IK;
using unvs.ext;

namespace unvs.players{
    public enum SolverChainFecting
    {
        FromLeaf,
        Single,
        FromSelected
    }
    public  class PlayerBoneSolverChain:PlayerBoneBase{


#if UNITY_EDITOR
        public SolverChainFecting applyType =SolverChainFecting.FromSelected;
        //private void Reset()
        //{
        //    GenerateSolve();
        //}

        private void createSingle()
        {
            var lst=clear(out var target,out var ikManager,out var sovler);
            var root = ikManager.transform;
            var currentTarget = target;
            currentTarget = currentTarget.AddChildComponentIfNotExist<Transform>(name);
            
            var s = sovler.AddChildComponentIfNotExist<FabrikSolver2D>(name);
            s.enabled = false;
            var ch = s.GetChain(0);
            if (ch == null)
            {
                Debug.LogError($"{s.name} GetChain is null");

            }
            ch.target = target.GetComponentInChildrenByName<Transform>(s.name);
            //ch.target.transform.position = bone.transform.position;
            ch.effector = transform;
            ch.target.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            ch.transformCount = lst.Count-1;
            ikManager.AddSolver(s);
        }


        private void createFromSelected(Transform tr)
        {
            // 1. Thực hiện logic tạo Solver cho Transform hiện tại (t)
            var lst = clear(out var target, out var ikManager, out var solver);
           
            
            bool flowControl = creaeChildFromSelected(lst,tr, target, ikManager, solver);
            if (!flowControl)
            {
                return;
            }
        }

        private bool creaeChildFromSelected(List<Transform> lst, Transform t, Transform target, IKManager2D ikManager, Transform solver)
        {
            var currentTarget = target;
            foreach (Transform tr in lst)
            {
                if (t == tr) break;
                currentTarget = currentTarget.AddChildComponentIfNotExist<Transform>(tr.name);
            }
            var s = solver.AddChildComponentIfNotExist<FabrikSolver2D>(t.name);
            s.enabled = false;

            var ch = s.GetChain(0);
            if (ch == null)
            {
                Debug.LogError($"{s.name} GetChain is null");
                return false; // Thoát để tránh lỗi NullReference tiếp theo
            }

            // Thiết lập Chain
            ch.target = currentTarget.AddChildComponentIfNotExist<Transform>(s.name);
            ch.effector = t;
            ch.target.position = t.position;
            ch.transformCount = 2;

            ikManager.AddSolver(s);

            // 2. DUYỆT ĐỆ QUY: Chỉ duyệt qua các con TRỰC TIẾP của t
            // Dùng foreach (Transform child in t) là cách nhanh nhất để lấy con cấp 1
            foreach (Transform child in t)
            {
                creaeChildFromSelected( lst, child,target,ikManager,solver);
            }

            return true;
        }

        private void createMulti()
        {
           
           
            var lst = clear(out var target,out var ikManager, out var sovler);
            var root= ikManager.transform;
            var currentTarget = target;
            foreach (Transform t in lst)
            {
                currentTarget=currentTarget.AddChildComponentIfNotExist<Transform>(t.name);
            }
            foreach (Transform t in lst)
            {
                if (t.parent == root) continue;
               var s= sovler.AddChildComponentIfNotExist<FabrikSolver2D>(t.name);
                s.enabled = false;
                var ch = s.GetChain(0);
                if (ch == null)
                {
                    Debug.LogError($"{s.name} GetChain is null");
                   
                }
                ch.target = target.GetComponentInChildrenByName<Transform>(s.name);
                //ch.target.transform.position = bone.transform.position;
                ch.effector = t;
                ch.target.position = new Vector3(t.position.x, t.position.y, t.position.z);
                ch.transformCount = 2;
                ikManager.AddSolver(s);
            }
           
        }



        public override void GenerateSolve()
        {

            if (applyType == SolverChainFecting.FromLeaf)
                createMulti();
            else if (applyType == SolverChainFecting.Single)
                createSingle();
            else
                createFromSelected(transform);

        }
#endif

    }
}