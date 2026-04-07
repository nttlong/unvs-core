using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.U2D.IK;
using unvs.ext;

namespace unvs.players{
    public abstract class PlayerBoneBase:MonoBehaviour{

#if UNITY_EDITOR
        public List<Transform> clear(out Transform target, out IKManager2D ikManager, out Transform sovler)
        {
            ikManager = transform.CreateRootIKManager2DIfNotExist();

            var sovlers = ikManager.transform.parent.AddChildComponentIfNotExist<Transform>($"solver");
            sovlers.EditRemoveChildComponentIfExists<Transform>(name);
            sovler = sovlers.AddChildComponentIfNotExist<Transform>($"{name}");
            target = ikManager.transform.parent.AddChildComponentIfNotExist<Transform>($"targets");
            var root = ikManager.transform;
            var lst = new List<Transform>();
            var travel = transform;
            while (travel != root)
            {
                lst.Add(travel);
                travel = travel.parent;
            }
            lst.Reverse();

            foreach (Transform t in lst)
            {
                var s = ikManager.solvers.FirstOrDefault(p => !p.IsDestroyed() && p.name == t.name);
                if (s != null)
                {
                    ikManager.RemoveSolver(s);
                    sovler.EditRemoveChildComponentIfExists<FabrikSolver2D>(t.name);
                }
                // target.EditRemoveChildComponentIfExists<Transform>(t.name);

            }

            ikManager.solvers.RemoveAll(s => s == null);
            return lst;
        }

        public void ClearSolve()
        {
            clear(out var target, out var ikManager, out var sovler);

        }

        public abstract void GenerateSolve(); 
#endif
    }
}