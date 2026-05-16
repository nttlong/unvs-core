using Cysharp.Threading.Tasks;
using DG.Tweening;
using game2d.objects;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using unvs.actions;
using unvs.actionsbasics;
using unvs.ext;
using unvs.game2d.scenes;
using static Unity.VisualScripting.Member;

namespace unvs.game2d.objects
{
    [RequireComponent(typeof(SpriteRenderer))]
    public partial class UnvsMultiInteractPoint : UnvsInteractObject
    {
        public UnvsMultiInteractBody owner;
        public UnvsTransitionable trasistion;

        public override UniTask<ActionBaseSender> ExecuteAsync(MonoBehaviour target, CancellationTokenSource cts)
        {
            if (this.InteractData == null) this.InteractData = new data.UnsvInteractableData()
            {
                definition = new InteractionDefinition
                {
                    actions=new ActionBase[]
                    {
                         new MoveTo(),
                    }
                }
            };
            
            
            var ret= base.ExecuteAsync(target, cts);
            var unvsRigidObject=owner.GetComponentInChildren<UnvsRigidObject>();
            
                unvsRigidObject.ToggleHeight();
                
            return ret;
        }
        public override void Awake()
        {
            base.Awake();
            if (owner != null)
            {
               
                this.trasistion = owner.GetComponentInParent<UnvsTransitionable>();
            }
        }
    }
#if UNITY_EDITOR
    public partial class UnvsMultiInteractPoint : UnvsInteractObject
    {
       
    }
#endif
}