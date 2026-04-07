using UnityEngine;
using UnityEngine.U2D.IK;
using unvs.ext;

namespace unvs.players{
    public class PlayerBoneSolverLimb : PlayerBoneBase
    {
#if UNITY_EDITOR
        public override void GenerateSolve()
        {
            var lst = this.clear(out var target, out var ikManager, out var solver);
            var currentTarget = target;
            foreach (var item in lst)
            {
                currentTarget = currentTarget.AddChildComponentIfNotExist<Transform>(item.name);
            }
            var s = solver.AddChildComponentIfNotExist<LimbSolver2D>(name);
            s.enabled = false;
            var ch = s.GetChain(0);
            ch.effector = transform;
            ch.target = currentTarget;

            ikManager.AddSolver(s);
        } 
#endif
    }
}