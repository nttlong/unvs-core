using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D.Animation;
using UnityEngine.U2D.IK;
using UnityEngine.UI;
using unvs.ext;
using unvs.shares;

namespace unvs.game2d.scenes.actors
{
    [RequireComponent(typeof(AudioSource))]
    public class UnvsAnimStates : UnvsBaseComponent
    {
       
       
        private UnvsActor actor;
        private Vector3 oririnalScale;
        [SerializeField]
        public MotionAudio[] motionAudio;
        public Transform motionObject;
        [SerializeField]
        public AnimStateInfo[] animStates;
        private Collider2D coll;
        private float _direction;
       

        public float direction
        {
            get
            {
                return _direction;
            }
            set
            {
                _direction = value;
                if (_direction > 0)
                {
                    transform.localScale = oririnalScale;
                }
                if (_direction < 0)
                {
                    transform.localScale = new Vector3(-oririnalScale.x, oririnalScale.y, oririnalScale.z);
                }
            }
        }

        public virtual void Awake()
        {
            if (Application.isPlaying)
            {
                actor = GetComponent<UnvsActor>();
                oririnalScale = this.transform.localScale.CloneToNew();
                actor.motions = this;
                coll = this.GetComponent<Collider2D>();
                if(this.motionObject!=null)
                {
                    motionObject.gameObject.SetActive(false);
                }
            }
        }
        public void DirectionTo(Vector3 direction)
        {
            this.direction = this.coll.bounds.center.CalculateDirection(direction);
        }
        public void DirectionBy(Vector2 direction)
        {
            if (direction.x > 0)
                this.direction = 1;
            if (direction.x < 0)
                this.direction = -1;
        }
        public void BaseMotion(string name,string overideState=null)
        {
            if (string.IsNullOrEmpty( overideState))
            {
                if (actor.physical.currentHoldingItem)
                {
                    overideState = $"{name}-hangging-item";
            }
            }
            this.animStates.PlayBaseLayer(name, overideState);
           
        }
        public void Motion(string name)
        {
            this.animStates.PlayCrossFadeMotion(name);
        }
        public void AddtiveMotion(string name)
        {
            this.animStates.PlayAddtiveMotion(name);
        }
        public void Disable()
        {
           this.GetComponentInChildren<Animator>().enabled = false;
        }
        public void Enable()
        {
            this.GetComponentInChildren<Animator>().enabled = true;
        }
#if UNITY_EDITOR
        [UnvsButton("Load Motions")]
        public void LoadMotions()
        {
            this.GetComponentInChildren<Animator>().AddComponentIfNotExist<UnsvPalyerAnimatorEvent>();
            var animController = this.GetComponentInChildren<Animator>();
            if (animController == null)
            {
                throw new Exception($"{typeof(Animator)} was not found in {name}");
            }
            this.animStates = animController.EditorExtractAllMotions().ToArray();
            var lsAudio = this.motionAudio.Cast<MotionAudio?>().ToList();
            foreach (var mot in this.animStates)
            {
                var audio = lsAudio.FirstOrDefault(p => p?.name == mot.motionName && p?.LayerIndex==mot.layerIndex);
                if (audio == null)
                {
                    lsAudio.Add(new MotionAudio
                    {
                        name = mot.motionName,
                        LayerIndex= mot.layerIndex,
                        LayerName= mot.layerName,
                        blendName = mot.blendName,
                        value = mot.value,
                    });
                }
            }
            this.motionAudio = lsAudio.Cast<MotionAudio>().ToArray();
            this.motionObject = this.GetComponentInChildrenByName<Transform>("motions-object");
        }
        private Animator editorAnimController;
        internal void EditotPlay(AnimStateInfo animStateInfo)
        {
            foreach (var mot in this.animStates)
            {
                animStateInfo.animationController.SetLayerWeight(mot.layerIndex, 0);
            }
            if (!string.IsNullOrEmpty(animStateInfo.blendName))
            {
                animStateInfo.animationController.SetLayerWeight(animStateInfo.layerIndex, 1f);
                animStateInfo.animationController.SetFloat(animStateInfo.paramName, animStateInfo.value);
            }
            else
            {
                animStateInfo.animationController.SetLayerWeight(animStateInfo.layerIndex, 1f);
                animStateInfo.animationController.PlayInFixedTime(animStateInfo.motionName, animStateInfo.layerIndex, 0f);
                editorAnimController = animStateInfo.animationController;
            }
            EditorApplication.update -= EditorUpdate;
            EditorApplication.update += EditorUpdate;
        }
        void EditorUpdate()
        {
            if (editorAnimController == null) return;


            editorAnimController.Update(Time.deltaTime);

           
            EditorApplication.QueuePlayerLoopUpdate();
            SceneView.RepaintAll();
        }



#endif
    }
}