using unvs.ext;
using unvs.game2d.scenes;
using unvs.shares;
using UnityEngine;
namespace unvs.game2d.objects
{
    [ExecuteAlways]
    public class UnvsInteractObject : UnvsComponent
    {
        public override void InitDesignTime()
        {
            this.SetMeOnLayer(Constants.Layers.INTERACT_OBJECT);
        }

        public override void InitRuntime()
        {
            throw new System.NotImplementedException();
        }
    }
}