using UnityEngine;
using unvs.shares;

namespace unvs.game2d.scenes
{
    public class UnvsSceneTracker:MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
            if (collision.tag == Constants.Tags.TRIGGER_SCENE_CHANGE && collision.transform.parent == Camera.main.transform)
            {
                UnvsApp.Instance.RaiseEnterScene(this.GetComponentInParent<UnvsScene>());
               
            }
        
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
            if (collision.tag == Constants.Tags.TRIGGER_SCENE_CHANGE && collision.transform.parent == Camera.main.transform)
            {
                UnvsApp.Instance.RaiseExitScene(this.GetComponentInParent<UnvsScene>());
            }
        }
}
}