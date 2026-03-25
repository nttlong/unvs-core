//using Cysharp.Threading.Tasks;
//using System.Collections;
//using System.Threading;
//using System.Threading.Tasks;
//using UnityEngine;
//using UnityEngine.Rendering;
//using unvs.actions;
//using unvs.ext;
//using unvs.interfaces.teleport;
//using unvs.interfaces;
//using unvs.shares;
//namespace unvs.baseobjects
//{
//    [RequireComponent(typeof(BoxCollider2D))]
//    [RequireComponent(typeof(SpriteRenderer))]
//    public class EntranceObject : MonoBehaviour, IEntrance, IInteractableObject
//    {
//        [SerializeField] public bool isNewWorld;
       
//        public InteractionDefinition data;
//        public BoxCollider2D coll;

//        [SerializeField]
//        public Sprite defaultSprite;
//        private Sprite mySprite;
//        public string scenePath;
//        [SerializeField] public AudioClip openSound;
//        [SerializeField] public AudioClip closeSound;
//        public string spawnPos;

//        public bool IsNewWorld => isNewWorld;
        

//        public ExploringType Exploring => ExploringType.Unknown;

//        public InteractionDefinition Data => data;

//        public Collider2D Collider
//        {
//            get
//            {
//                if (coll != null) return coll;
//                coll = GetComponent<BoxCollider2D>();
//                return coll;
//            }
//        }



//        public Sprite MySprite
//        {
//            get => mySprite;
//            set
//            {
//                mySprite = value;
//                GetComponent<SpriteRenderer>().sprite = value;
//            }
//        }

//        public string PathTo => scenePath;

//        public AudioClip OpenSound => openSound;

//        public AudioClip CloseSound => closeSound;

//        public Vector2 Position
//        {
//            get
//            {
//                return GetPosition();
//            }
//        }

//        public string ScenePath
//        {
//            get
//            {
//                return GetComponentInParent<IScenePrefab>().Name;
//            }
//        }

//        public string SpawnPos => spawnPos;

//        public bool IsTempEntrance { get; set ; }

//        public async UniTask<bool> ExecAsync(MonoBehaviour target, CancellationTokenSource token)
//        {
//            return await data.ExecAsync(this, target, token);
//        }

//        public Vector2 GetPosition()
//        {
            
//            EdgeCollider2D edge = BoxCollider2dExtension.FindBottomEdge(coll, Constants.Layers.GROUND_TERRANT);
//            if(edge==null)
//            {
//                var scene = this.GetComponentInParent<IScenePrefab>();
//                edge = scene.Floor;
//            }
//            return edge.GetIntersetPoint(coll.bounds.center.x);
//        }
//        private void Awake()
//        {
//            this.SetMeOnLayer(Constants.Layers.INTERACT_OBJECT);
//            coll= GetComponent<BoxCollider2D>();
//        }
//#if UNITY_EDITOR
//        private void OnValidate()
//        {
//            if (Application.isPlaying) return;
//            GetComponent<BoxCollider2D>().isTrigger = true;
//            if (mySprite == null && defaultSprite != null)
//            {

//                GetComponent<SpriteRenderer>().sprite = defaultSprite;
//            }
//            UnityEditor.EditorUtility.SetDirty(this);
//            //string path=  MonoBehaviourExt.EditorModeGetAddressablePath(this as MonoBehaviour, "EntranceObjectDefalult.asset");


//        }
//#endif
//    }
//}