using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D.Animation;
using unvs.components;

using unvs.ext;
using unvs.game2d.actors;
using unvs.game2d.objects.editor;
using unvs.shares;
using unvs.types;

namespace unvs.animators_controllers
{
    
    [Serializable]
    public partial class motion_controllers<T> : unvs.types.UnvsProperty<T> where T : UnvsBaseComponent
    {
        public Animator editorAnimController;
        //public GameObject animEle;
        //public Animator animator;
        //public MonoBehaviour owner;
        [SerializeField]
        public AnimStateInfo[] animStates;
        [SerializeField]
        public MotionAudio[] motionAudio = new MotionAudio[] { };
        [SerializeField]
        public AnimStateInfo currentBaseAnimState;
        public Animator animator;
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
#if UNITY_EDITOR
        [SerializeField]
        public unvs.animators_controllers.texturesEditor[] textures;
#endif
        [UnvsButton("Load Motions")]
        public void LoadMotions() { 


        var anim = this.Owner.GetComponentInChildren<Animator>();
            if (anim == null)
            {
                unvs.editor.utils.Dialogs.Show($"Please, create animator for {Owner.name}");
            }
            this.Owner.GetComponentInChildren<Animator>().AddComponentIfNotExist<UnsvPalyerAnimatorEvent>();
           
            this.animStates = this.Owner.GetComponentInChildren<Animator>().EditorExtractAllMotions().ToArray();
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
           // GenerateAnimatorController();
        }


        [UnvsButton("Extract all sprite skin")]
        public void GenerateAnimatorController()
        {
            Debug.Log($"GenerateAnimatorControlle.Ownerr={Owner}");
            var spriteRenders = Owner.GetComponentsInChildren<SpriteRenderer>(true)
                .Where(p => p.sprite != null)
                .GroupBy(p => p.sprite.texture)
                .Select(p => p.First()).ToArray();
            if (spriteRenders.Length==0)
            {
                var msg = $"{Owner.name}.SpriteRenderer was not found";
                unvs.editor.utils.Dialogs.Show(msg);
                return;
            }
            textures = spriteRenders.Select(p => new texturesEditor
            {
                name = p.sprite.texture.name,
                texttue=p.sprite.texture,
                transform=p.transform.parent,
                owner=Owner as MonoBehaviour,
                folderPath= unvs.editor.utils.UnvsEditorUtils.EditorGetTueFolder((Owner as MonoBehaviour).gameObject),
               
            }).ToArray();
           
           
            //var sprite = Owner.GetComponentInChildren<SpriteSkin>(true);
            //if(sprite == null)
            //{
            //    //var msg = $"{Owner.name}.sprite is null. Please create skining sprite editor";
            //    //unvs.editor.utils.Dialogs.Show(msg);
            //    unvs.editor.utils.UnvsEditorUtils.OpenSpriteEditor(textures.FirstOrDefault().texttue);

            //    return;
            //}
            
            //if (Owner == null || Owner.GetComponentInChildren<SpriteSkin>(true)==null)
            //{
            //    return;
            //}
            
            
            
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
    [Serializable]
    public partial class texturesEditor : UnvsEditableProperty
    {
        public string name;
        public Texture2D texttue;
        public Transform transform;
        public Animator animator;
        public MonoBehaviour owner;
        public string folderPath;
        

      

        [UnvsButton("Set As Default")]
        public void EditorSetAsDefault()
        {
            if (this.animator == null)
            {
                unvs.editor.utils.Dialogs.Show($"Can ot use this ad default! (Animator=null)");
                return;
            }

        }
        [UnvsButton("Edit srpite")]
        public void EditSprite()
        {
            unvs.editor.utils.UnvsEditorUtils.OpenSpriteEditor(texttue);
        }
        [UnvsButton("Create Anim")]
        public void EditCreateAnim()
        {
            

            
            if (this.animator == null) this.animator = this.transform.gameObject.GetComponent<Animator>();
            if (this.animator != null)
            {
                var msg = $"{this.animator.name} is already in {this.animator.transform.parent.name}. Do you want to create new and overide it";
                if (!unvs.editor.utils.Dialogs.Confirm(msg))
                {
                    return;

                }
            }
            
            var controller = unvs.editor.utils.UnvsEditorUtils.EditorCreateAnimatorController(folderPath, this.transform.name);

            this.animator = this.transform.AddComponentIfNotExist<Animator>();
            this.animator.runtimeAnimatorController = controller;
        }
    }
#endif
}