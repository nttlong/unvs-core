using UnityEngine;
using unvs.shares;

namespace unvs.game2d.scenes
{
    public class UnvsSceneTracker:MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
            if (collision.tag == Constants.Tags.TRIGGER_SCENE_CHANGE  || collision.tag==Constants.Tags.PLAYER_CAM_WATCHER)
            {
                UnvsApp.Instance.RaiseEnterScene(this.GetComponentInParent<UnvsScene>());
               
            }
        
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
            if (collision.tag == Constants.Tags.TRIGGER_SCENE_CHANGE || collision.tag == Constants.Tags.PLAYER_CAM_WATCHER)
            {
                UnvsApp.Instance.RaiseExitScene(this.GetComponentInParent<UnvsScene>());
            }
        }
}
}