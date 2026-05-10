using UnityEngine;

namespace unvs.ui
{
    public partial class UnvsUIInpsectCam : UnvsUIComponentInstance<UnvsUIInpsectCam>
    {
        public override bool DisablePlayerInput => false;

        public override bool EnablePlayerInput => false;

        public override void InitEvents()
        {
            throw new System.NotImplementedException();
        }
    }
}