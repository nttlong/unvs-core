using UnityEngine;
using unvs.shares;

namespace unvs.game2d.scenes.actors {
    public class UnvsActorSwimmable : UnvsActorSkill
    {


        private void Update()
        {
            var layer = GetComponent<UnvsActorPhysical>().GetHitLayerDown(Constants.Layers.WORLD_LIQUID, Constants.Layers.WORLD_GROUND);
          
            if (layer==Constants.Layers.WORLD_LIQUID)
            {
                ActiveSkill();

            } else if (layer==Constants.Layers.WORLD_GROUND)
            {
                DeactiveSkill();
            }
        }
    }
}