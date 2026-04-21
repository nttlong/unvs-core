using UnityEngine;
using unvs.ext;
namespace unvs.game2d.objects
{
    /// <summary>
    /// This class define an object contains multiple interact points.
    /// </summary>
    public partial class UnvsMultiInteractPointsObject : UnvsBaseComponent
    {
        public Func<ActionBaseSender,UniTask> OnFirstTimeInteract;
        
        public UnsvInteractObjects[] interactPoints;
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
       [UnvsB   utton("Add Interact Point")]
       public void AddInteractPoint()
       {
           var name="InteractPoint"+interactPoints.Length;
           var interactPoint=this.AddChilComponent<UnsvInteractObjects>();
           interactPoint.name=name;
           
           interactPoints=ArrayUtility.Add(interactPoints, interactPoint);
       }
    }
    #endif

}