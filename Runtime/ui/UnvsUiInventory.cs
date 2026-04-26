using unvs.components;

namespace unvs.ui{
    public class UnvsUiInventory : UnvsUIComponentInstance<UnvsUiInventory>
    {
        public override bool DisablePlayerInput => true;

        public override bool EnablePlayerInput => true;

        public override void InitEvents()
        {
            throw new System.NotImplementedException();
        }
    }
}