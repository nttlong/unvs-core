using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using unvs.actions;
using unvs.ext;
using unvs.game2d.scenes;
using unvs.shares;
#if UNITY_EDITOR
using unvs.shares.editor;
#endif

namespace unvs.game2d.objects
{
    [RequireComponent(typeof(UnvsSpawnPoint))]
    public class UnvsTeleport : UnvsInteractObject
    {
        public AssetReference Target;
        public string TargetPath;

        public override void InitDesignTime()
        {
            base.InitDesignTime();
            if (Target != null)
            {
                this.TargetPath = Target.EditorGetAddressPath();
            }


        }

        public override void InitRuntime()
        {

        }
#if UNITY_EDITOR

        [UnvsButton("Apply data")]
        public void EditorApplyData()
        {
            if (Target != null)
            {
                this.TargetPath = Target.EditorGetAddressPath();
            }
        }
#endif
    }

}