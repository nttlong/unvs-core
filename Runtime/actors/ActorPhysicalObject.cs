using System;
using System.Collections;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.U2D.Animation;
using UnityEngine.U2D.IK;
using unvs.ext;
using unvs.gameobjects;
using unvs.interfaces;
using unvs.shares;


namespace unvs.actors
{
    [ExecuteInEditMode]
    public class ActorPhysicalObject : ActorPhysicsSuite, IActorPhysical
    {
        public Transform rootArm;
        public Transform topArm;
        public Transform socketBack;
        public Transform socketFront;
        public MonoBehaviour currentHoldingObject;
        private DirectionEnum direction;
        private IActorIK actorIK;
        public Transform IKControls;
        public Transform rootBone;
        private ContactFilter2D floorFilter;
        //private ContactPoint2D[] contacts = new ContactPoint2D[10]; // Cache sẵn mảng 10 phần tử
        public bool lockIK;
        public float reachTolerance=Constants.Settings.REACH_TOLERRANCE;
        float _armLen = -1;
        public float ArmLen
        {
            get
            {

               
                return Vector2.Distance(rootArm.position, topArm.position) + topArm.GetSegment().Length;
            }
        }
        public Vector2 GetHandPosition()
        {
            if (topArm == null)
            {
                throw new NullReferenceException($"topArm of {name} is null");
            }
            return topArm.transform.position;
        }
        public Transform RootArm => rootArm;

        public Transform TopArm => topArm;

        public Transform SocketBack => socketBack;

        public Transform SocketFront => socketFront;

        public MonoBehaviour CurrentHoldingObject { get => currentHoldingObject; set => currentHoldingObject = value; }
        public DirectionEnum Direction
        {
            get => direction;
            set
            {
                direction = value;
                GetComponent<IActorMotion>()?.Flip(value == DirectionEnum.Backward ? -1 : 1);
            }
        }

        public IActorIK ActorIK => actorIK;

        public float ReachTolerance => reachTolerance;

        public void HoldItem(MonoBehaviour item)
        {
            item.GetComponent<Collider2D>().enabled = false;
            item.GetComponent<SortingGroup>().enabled = false;
            var obj = item.GetComponent<ICarryableObject>();
            if (obj == null) return;
            currentHoldingObject = item;
          
            
            CurrentHoldingObject.transform.AttachToParent(SocketBack.transform);

            SpriteRenderer sr = (this as MonoBehaviour).FindSpriteRenderer(SocketBack.transform.parent);
            if (sr != null)
            {
                CurrentHoldingObject.SetSortingOrder(sr, true);
                
            }



        }
#if UNITY_EDITOR
        private void OnValidate()
        {
            if(Application.isPlaying) return;
            if (UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() == null) return;
            //var spriteSkins = this.GetComponentsInChildren<SpriteSkin>(true);
            //rootBone = spriteSkins.SelectMany(x => x.boneTransforms).GetRoot();
            if (rootBone != null)
            {
                IKControls = rootBone.parent.AddChildComponentIfNotExist<Transform>("IKControls");
                FindRootBone();
            }
            //spriteSkins.CreateIK(this.AddChildComponentIfNotExist<ActorIKObject>("IK").transform);
        }
        private Transform FindRootBone()
        {

            if(lockIK) return null;
            var spriteSkins = this.GetComponentsInChildren<SpriteSkin>(true);
            if (IKControls == null) return null;
            spriteSkins.CreateIK(IKControls.transform);
            return null;
        }
#endif


        public void Validate()
        {
            if (!Application.isPlaying) return;
            _ = this.socketBack.position;
            _ = this.socketFront.position;
            _ = this.rootArm.position;
            _ = this.topArm.position;


        }
        protected override void Awake()
        {
            base.Awake();
            //if (!Application.isPlaying)
            //{
            //    actorIK = this.AddChildComponentIfNotExist<ActorIKObject>("IK");
            //}
            //else
            //{
            //    actorIK = this.GetComponentInChildrenByName<ActorIKObject>("IK");
            //}
        }
        private void Start()
        {
            
            Validate();
        }

        

        public bool CanReachTarget(Vector2 pos)
        {
            

            return  math.abs(math.abs(GetPosition().x-pos.x) - ArmLen)<this.ReachTolerance;
        }

        public Vector2 GetPosition()
        {
            return GetComponent<Collider2D>().bounds.center;
        }

        public bool IsTargetLower(Vector2 pos)
        {
            return this.GetHandPosition().y >= pos.y;
        }
    }
}