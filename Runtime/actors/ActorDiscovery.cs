//using Cysharp.Threading.Tasks;
//using UnityEngine;
//using unvs.interfaces;
//namespace unvs.actors
//{
//    public class ActorDiscovery : MonoBehaviour, IActorDiscovery
//    {
//        public IDiscoveryDialog Dialog
//        {
//            get
//            {
                
//                return unvs.shares.GlobalApplication.UIDiscoveryDialog;
//            }
//        }

//        public async UniTask ShowDialogAsync(MonoBehaviour source)
//        {
//            if (unvs.shares.GlobalApplication.UIDiscoveryDialog == null) return;
//            await unvs.shares.GlobalApplication.UIDiscoveryDialog.DoShowDialogAsync(this, source);
//        }
//    }
//}