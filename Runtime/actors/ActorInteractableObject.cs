
//using System.Threading;
//using UnityEngine;
//using unvs.ext;
//using unvs.interfaces;

//namespace unvs.actors
//{
//    public class ActorInteractableObject : MonoBehaviour, IActorInteractable
//    {
//        private IActorInteractable instance;
//        public float scanWidth=5;
//        public float scanHeight=5;
//        public string[] layers=new string[] {
//            unvs.shares.Constants.Layers.INTERACT_OBJECT,
//            unvs.shares.Constants.Layers.NPC,
//             unvs.shares.Constants.Layers.ENEMIES,
//        };

//        public CancellationTokenSource Cts { get; set; }

//        public float ScanWidth => scanWidth;

//        public float ScanHeight => scanHeight;

//        public string[] Layers => layers;

//        public GameObject ScanObject()
//        {
//            var coll=GetComponent<Collider2D>();
//            return coll.ScanObject(instance.ScanWidth, instance.ScanHeight, LayerMask.GetMask(instance.Layers));
//        }
//        private void Awake()
//        {
//            instance = this;
//        }

//        public GameObject GetObject(Vector2 input)
//        {
//            return input.GetObjectInLayer(LayerMask.GetMask(instance.Layers));
//        }
//    }
//}