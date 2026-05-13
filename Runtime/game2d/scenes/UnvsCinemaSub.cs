
using game2d.ext;
using Unity.Cinemachine;
using UnityEngine;
using unvs.ext;
using unvs.ui;

namespace unvs.game2d.scenes
{
    public partial class UnvsCinemaSub : UnvsNonUIComponentInstance<UnvsCinemaSub>
    {
       
        public CinemachineCamera VCam;
        public Transform watcher;
        public CinemachineFollow follower;
        public override void InitRunTime()
        {
            base.InitRunTime();
            this.VCam.Watch(watcher);
        }
    }
#if UNITY_EDITOR
    public partial class UnvsCinemaSub : UnvsNonUIComponentInstance<UnvsCinemaSub>
    {
        

        [unvs.game2d.objects.editor.UnvsButton("Generate")]
        public void EditorGenerate()
        {
            this.VCam = this.AddChildComponentIfNotExist<CinemachineCamera>("vcam");
            this.VCam.Priority = 0;
            this.follower= VCam.AddComponentIfNotExist<CinemachineFollow>();
            this.watcher = this.AddChildComponentIfNotExist<Transform>("watcher");
            

        }
    }
#endif
}