using System;
using unvs.actor.skills;
using unvs.components;
using unvs.ui;

namespace UNVS.Core.Actors.Skills
{
    public class ActorUnvsInventorySkill : ActorDefaultSkill
    {
        public void ToggleInventoryPanel()
        {
            var bagger = Owner.GetComponent<UnvsBagger>();
            if (bagger==null)
            {
                return;
            }
            if (UnvsUIInventory.Instance!=null)
            {
                UnvsUIInventory.Instance.Toggle(bagger);
            }
        }
    }
}