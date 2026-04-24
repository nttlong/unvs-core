using System;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.U2D.IK;
using unvs.ext;
using unvs.game2d.objects.editor;

namespace unvs.animators_controllers
{
    [Serializable]
    public partial class ik_manager_controllers : unvs.types.UnvsEditableProperty
    {
        [Header("IK Setup")]
        [SerializeField]
        public Transform[] Chain;
        [SerializeField]
        public Transform[] Limbs;
        [Header("Bone setup")]
        public Transform rootBone;
        public Transform ikControl;
        public Transform targets;
        public Transform solvers;
        public Transform solversLimbs;
        public Transform solversChains;
        public IKManager2D ikManager;

        public MonoBehaviour owner;

    }
#if UNITY_EDITOR
    public partial class ik_manager_controllers:unvs.types.UnvsEditableProperty
    {

        [UnvsButton("Create IK Manager")]
        public void CreateIKManager()
        {
            this.rootBone = findRootBone();
            var animator = owner.GetComponentInChildren<Animator>();
            if (animator == null)
            {
                
                unvs.editor.utils.Dialogs.Show($"{typeof(Animator)} was not found in {owner.name}");
                return;
            }
            var anim = animator.transform;
           
            if (Chain == null && Chain.Length == 0 && Limbs == null && Limbs.Length == 0)
            {
                unvs.editor.utils.Dialogs.Show($"Chains or Limbs in the {owner.name}," +
                    $" section must be assigned. To create an IK Manager, " +
                    $"you must select at least one Transform in the Chains list or the Limbs list.");
                return;
            }
            this.ikControl = anim.AddChildComponentIfNotExist<Transform>("IK-Control");
            this.targets = this.ikControl.AddChildComponentIfNotExist<Transform>("targets");
            this.solvers = this.ikControl.AddChildComponentIfNotExist<Transform>("solvers");
            //if (this.solversLimbs != null)
            //{
            //    this.solversLimbs.name = $"{this.solversLimbs.name}-delete";
            //    UnityEngine.Object.Destroy(this.solversLimbs);
            //}
                
            this.solversLimbs = this.solvers.AddChildIfNotExist<Transform>("solversLims");
            this.solversChains = this.solvers.AddChildIfNotExist<Transform>("solversChains");
            fetchTarget(this.targets, this.rootBone);
            this.ikManager = this.rootBone.AddComponentIfNotExist<IKManager2D>();
            createSolver();
        }

        private void createSolver()
        {
            //foreach (Transform t in this.solversLimbs)
            //{
            //    UnityEngine.Object.DestroyImmediate(t.gameObject);
            //}

            foreach (var tr in Limbs)
            {
                var l = this.solversLimbs.AddChildIfNotExist<LimbSolver2D>(tr.name);
                l.enabled = false;
                this.ikManager.AddSolver(l);
                var c = l.GetChain(0);
                c.effector = tr;
                c.target = this.targets.GetComponentInChildrenByName<Transform>(tr.name);
            }
            foreach (Transform t in this.solversChains)
            {
                UnityEngine.Object.DestroyImmediate(t.gameObject);
            }
            foreach (var tr in Chain)
            {

                var l = this.solversChains.AddChildIfNotExist<FabrikSolver2D>(tr.name);
                l.enabled = false;
                this.ikManager.AddSolver(l);
                var c = l.GetChain(0);
                c.effector = tr;
                c.target = this.targets.GetComponentInChildrenByName<Transform>(tr.name);
            }
            this.ikManager.solvers.RemoveAll(p => p == null);
        }

        private void fetchTarget(Transform targets, Transform rootBone)
        {
            var rr = targets.AddChildComponentIfNotExist<Transform>(rootBone.name);
            rr.transform.position = rootBone.position;
            foreach (Transform tr in rootBone.GetComponentInChildren<Transform>(true))
            {
                fetchTarget(rr, tr);
            }
        }

        private Transform findRootBone()
        {
            var spriteSkins = owner.GetComponentsInChildren<SpriteSkin>(true);
            return spriteSkins.SelectMany(p => p.boneTransforms).GetRoot();
        }

    }
#endif
}