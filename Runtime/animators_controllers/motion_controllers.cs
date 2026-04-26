using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D.Animation;
using unvs.ext;
using unvs.game2d.actors;
using unvs.game2d.objects.components;
using unvs.game2d.objects.editor;
using unvs.shares;

namespace unvs.animators_controllers
{
    [Serializable]
    public partial class motion_controllers<T> : unvs.types.UnvsProperty<T> where T : UnvsBaseComponent
    {
        public Animator editorAnimController;
        public GameObject animEle;
        public Animator animator;
        //public MonoBehaviour owner;
        [SerializeField]
        public AnimStateInfo[] animStates;
        [SerializeField]
        public MotionAudio[] motionAudio = new MotionAudio[] { };
        [SerializeField]
        public AnimStateInfo currentBaseAnimState;
        private Dictionary<string, AnimStateInfo> _dictBaseMotion;
        private AnimStateInfo currentOrerideAnimState;
        private Dictionary<string, AnimStateInfo> _dictOverideMotion;

        public void BaseMotion(string name, string overideState = null, AnimatorOverrideController animatorOverrideController = null)
        {
            if (_dictBaseMotion == null) _dictBaseMotion = new Dictionary<string, AnimStateInfo>();
            if (_dictBaseMotion.ContainsKey(name.ToLower()))
            {
                currentBaseAnimState = _dictBaseMotion[name.ToLower()];


            }
            else
            {
                this.currentBaseAnimState = this.animStates.GetBaseStateByName(name);


                //this.currentBaseAnimState = this.animStates.PlayBaseLayer(name, overideState, animatorOverrideController);
                _dictBaseMotion.Add(name.ToLower(), currentBaseAnimState);

            }
            currentBaseAnimState.PlayAsBaseState();

        }
        public void Motion(string name)
        {
            this.animStates.PlayCrossFadeMotion(name);
        }
        public async UniTask MotionAsync(string name, CancellationToken tk = default, string overideState = null, Func<bool> OnPlay = null, Action OnFinish = null)
        {
            await this.animStates.PlayMotionAsync(name, tk, null, OnPlay, OnFinish);
        }
        public void OverideMotion(string name)
        {
            if (_dictOverideMotion == null) _dictOverideMotion = new Dictionary<string, AnimStateInfo>();
            if (_dictOverideMotion.ContainsKey(name.ToLower()))
            {
                this.currentOrerideAnimState = _dictOverideMotion[name.ToLower()];


            }
            else
            {
                this.currentOrerideAnimState = this.animStates.GetOverideStateByName(this.currentBaseAnimState, name);
                _dictOverideMotion.Add(name.ToLower(), this.currentOrerideAnimState);
            }
            this.currentOrerideAnimState.PlayAsOverideState();
        }

    }
#if UNITY_EDITOR
    public partial class motion_controllers<T> : unvs.types.UnvsProperty<T> where T : UnvsBaseComponent
    {
        [UnvsButton("Load Motions")]
        public void LoadMotions()
        {
            this.Owner.GetComponentInChildren<Animator>().AddComponentIfNotExist<UnsvPalyerAnimatorEvent>();
            var animController = Owner.GetComponentInChildren<Animator>();
            if (animController == null)
            {
                unvs.editor.utils.Dialogs.Show($"{typeof(Animator)} was not found in {Owner.name}");
                return;
            }
            this.animStates = animController.EditorExtractAllMotions().ToArray();
            var lsAudio = new List<MotionAudio?>();
            foreach (var mot in this.animStates)
            {
                var audio = lsAudio.FirstOrDefault(p => p?.name == mot.motionName && p?.LayerIndex == mot.layerIndex);
                if (audio == null)
                {
                    lsAudio.Add(new MotionAudio
                    {
                        name = mot.motionName,
                        LayerIndex = mot.layerIndex,
                        LayerName = mot.layerName,
                        blendName = mot.blendName,
                        value = mot.value,
                    });
                }
            }
            this.motionAudio = lsAudio.Cast<MotionAudio>().ToArray();
            GenerateAnimatorController();
        }


        [UnvsButton("Create Anim controller")]
        public void GenerateAnimatorController()
        {
            this.animEle = Owner.GetComponentInChildren<SpriteSkin>(true).transform.parent.gameObject;
            string folderPath = unvs.editor.utils.UnvsEditorUtils.EditorGetFolder(this.animEle);
            if (this.animator == null) this.animator = this.Owner.GetComponentInChildren<Animator>();
            if (this.animator!=null)
            {
                var msg = $"{this.animator.name} is already in {this.Owner.name}. Do you want to create new and overide it";
                if (unvs.editor.utils.Dialogs.Confirm(msg))
                {
                    var controller = unvs.editor.utils.UnvsEditorUtils.EditorCreateAnimatorController(folderPath, this.animEle.name);
                    this.animator = this.Owner.transform.AddComponentIfNotExist<Animator>();
                    this.animator.runtimeAnimatorController = controller;
                }
            }
           
            
        }

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

    }
#endif
}