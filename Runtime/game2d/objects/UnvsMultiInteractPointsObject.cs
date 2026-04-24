
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using System;
#if UNITY_EDITOR
using UnityEditor; 
#endif
using UnityEngine;
using unvs.actions;
using unvs.ext;
using unvs.game2d.objects.components;
using unvs.game2d.objects.editor;
using unvs.game2d.scenes;
namespace unvs.game2d.objects
{
    /// <summary>
    /// This class define an object contains multiple interact points.
    /// </summary>
    public partial class UnvsMultiInteractPointsObject : UnvsBaseComponent
    {
        public Func<ActionBaseSender,UniTask> OnFirstTimeInteract;
        
        public UnvsMultiInteractPoint[] interactPoints=new UnvsMultiInteractPoint[] {};
        public virtual void Awake(){
            if(Application.isPlaying)
            foreach(var interactPoint in interactPoints){
                interactPoint.OnFirstTimeInteract=OnFirstTimeInteract;
            }
        }
    }
    #if UNITY_EDITOR
    public  partial class UnvsMultiInteractPointsObject : UnvsBaseComponent
    {
        public UnvsMultiInteractBody body;

        
        [UnvsButton("Add Interact Point")]
       public void AddInteractPoint()
       {
            if(body == null)
            {
                body = this.AddChildComponentIfNotExist<UnvsMultiInteractBody>("body");
            }
           var name="InteractPoint"+interactPoints.Length;
           var interactPoint=this.AddChildComponentIfNotExist<UnvsMultiInteractPoint>(name);
            interactPoint.owner = body;
            ref var interPoint = ref interactPoints;
             ArrayUtility.Add(ref interPoint, interactPoint);
       }
    }
    #endif

}