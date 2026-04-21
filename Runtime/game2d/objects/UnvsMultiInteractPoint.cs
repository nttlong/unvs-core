using Cysharp.Threading.Tasks;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using unvs.actions;
using unvs.actionsbasics;
using unvs.ext;
using unvs.game2d.scenes;

namespace unvs.game2d.objects
{
    [RequireComponent(typeof(SpriteRenderer))]
    public partial class UnvsMultiInteractPoint : UnvsInteractObject
    {
        public UnvsMultiInteractBody owner;
        public override UniTask<ActionBaseSender> ExecuteAsync(MonoBehaviour target, CancellationTokenSource cts)
        {
            if (this.Data == null) this.Data = new InteractionDefinition();
            if (this.Data.actions == null) this.Data.actions = new ActionBase[] { 
                new MoveTo(),
                new TransitionAction()
            };
            
            return base.ExecuteAsync(target, cts);
        }
    }
#if UNITY_EDITOR
    public partial class UnvsMultiInteractPoint : UnvsInteractObject
    {
       
    }
#endif
}