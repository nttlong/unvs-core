using Cysharp.Threading.Tasks;
using game2d.scenes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.Cinemachine;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using unvs.components;
using unvs.ext;
using unvs.game2d.actors;
using unvs.game2d.objects.editor;
using unvs.shares;
using unvs.ui;

namespace unvs.game2d.scenes{
    public class UnvsCinema : UnvsUIComponentInstance<UnvsCinema>
    {
        public UniTask CamChangeFollowOffsetTask;
        /// <summary>
        /// Limit z of cinema michonne, the distance of cam to plane of view is always bigger or equal by this value 
        /// That mean z of follow offset o cinemachine is alway add negative of this value
        /// </summary>
        [Header("Camera")]
        [SerializeField]
        public Vector3 DefaultTargetOffset=new Vector3(0,0,-35);
        public float CamWacherDistance = -30;
        [Header("Cinema light")]
        public float DurationTimeSmoothChangeSate = 1.5f;
        public int MaintainGlobalLightNumber = 5;
        [SerializeField]
        public GlobalLightChunkInfo[] worldLightMaintain;
        private List<GlobalLightChunkInfo> _lights = new List<GlobalLightChunkInfo>();
        public event Action<UnvsScene> BeforeUpdate;
        public event Action<UnvsScene> AfterUpdate;
        public Camera cam;
        public Physics2DRaycaster physics2DRaycaster;
        public CinemachineCamera vcam;
        public CompositeCollider2D compositeCollider2D;
        public CinemachineConfiner2D confiner;
       
        public BoxCollider2D camColl;
        
        Dictionary<UnvsScene,PolygonCollider2D> worldBoundDict=new Dictionary<UnvsScene, PolygonCollider2D>();
        //Dictionary<UnvsScene, Light2D> lightDict = new Dictionary<UnvsScene, Light2D>();
       
        public PolygonCollider2D worldBoundCollider2d;
        public Transform centerWatch;
        CancellationTokenSource ctsChangeOffset;
        CancellationTokenSource ctsChangeOrthoSize;
        public Transform sceneLoaderTracing;
        public BoxCollider2D centerCamTracing;
        public Light2D globalLight;
       
       
       
        
        [SerializeField] public AudioSource audioSource;

        
        public void ChangeCameraState(List<UnvsScene> s,bool Imediately)
        {

            changeCameStepByOffset(s, Imediately, () =>
            {
                
            });

            //ChangeCameraStateAsync(s).Forget();
        }
        private bool changeCameStepByOffset(List<UnvsScene> s, bool Imediately,Action OnChange)
        {
            ctsChangeOffset.Refresh();
            UnvsScene nearset = null;
            if (Imediately)
            {


                vcam.UpdateFollowOffset(s[0].followOffset);
                camColl.size = cam.GetCameraWorldSize();
                return false;
            }
            nearset = CalculateNearestScene(s);
            if(nearset==null) return false;
            ctsChangeOffset = ctsChangeOffset.Refresh();
            if (nearset.followOffset == Vector3.zero)
            {
                nearset.followOffset = this.DefaultTargetOffset;
            }
            var tsk =
            vcam.ChangeFollowOffsetSmoothAsync(OnChange, nearset.followOffset, ctsChangeOffset.Token, 3).ContinueWith(() =>
            {
                camColl.size = cam.GetCameraWorldSize();
            }).Preserve();
            //tsk.Forget();
            this.CamChangeFollowOffsetTask = tsk;

            return true;
        }
        //private bool chaneCameSetByOrthoSize(List<UnvsScene> s, bool Imediately,Action OnChange)
        //{
        //    UnvsScene nearset = null;
        //    if (Imediately)
        //    {

                
        //        return false;
        //    }
        //    nearset = CalculateNearestScene(s);
        //    ctsChangeOffset = ctsChangeOffset.Refresh();
            
        //        vcam.ChangeFollowOffsetSmoothAsync(OnChange, nearset.followOffset, ctsChangeOffset.Token, DurationTimeSmoothChangeSate).Forget();
          

        //    return true;
        //}

        public async UniTask ChangeCameraStateAsync(List<UnvsScene> s)
        {
            UnvsScene nearset = CalculateNearestScene(s);
            ctsChangeOffset = ctsChangeOffset.Refresh();
            ctsChangeOrthoSize = ctsChangeOrthoSize.Refresh();
            await vcam.ChangeFollowOffsetSmoothAsync(() => {
                camColl.size = cam.GetCameraWorldSize();
            }, nearset.followOffset, ctsChangeOffset.Token);


        }
        private UnvsScene CalculateNearestScene(List<UnvsScene> scenes)
        {
            if (scenes == null || scenes.Count == 0) return null;

            UnvsScene closestScene = null;
            float minDistance = float.MaxValue;
            float centerX = centerCamTracing.bounds.center.x;

            foreach (var s in scenes)
            {
                if(s==null||s.IsDestroying||s.IsDestroyed()) continue;
               
                float distLeft = math.abs(s.wallLeft.bounds.max.x - centerX);
                float distRight = math.abs(s.wallRight.bounds.min.x - centerX);
                float currentMin = math.min(distLeft, distRight);

               
                if (currentMin < minDistance)
                {
                    minDistance = currentMin;
                    closestScene = s;
                }
            }

            return closestScene;
        }
        
        /// <summary>
        /// This method update cam worls bound. light,...
        /// The next pharse is ambient
        /// </summary>
        /// <param name="ret"></param>
        public void UpdateWorld(UnvsScene ret,bool reset, UpdateWorldEmun UpdateType)
        {
            if (reset)
            {
                this.worldBoundDict = new Dictionary<UnvsScene, PolygonCollider2D>();
                this._lights.Clear();
                //this.lightDict.Clear();
            }
            this.BeforeUpdate?.Invoke(ret);
            
            this.worldBoundDict.Add(ret, ret.worldBound);
            if (ret.light2d != null)
            {
                ret.light2d.enabled = false;
                ret.light2d.gameObject.SetActive(false);
                ret.light2d.transform.position = ret.worldBound.bounds.center;
                if(_lights.Count> this.MaintainGlobalLightNumber)
                {
                    removeLight(UpdateType);
                }
                this._lights.Add(new GlobalLightChunkInfo
                {
                    color = ret.light2d.color,
                    createdOn=DateTime.Now,
                    intensity = ret.light2d.intensity,
                    position= ret.light2d.transform.position,
                });
            }
            worldLightMaintain = this._lights.ToArray();
           

         
            updateWorldBound();
            this.AfterUpdate?.Invoke(ret);
            
                
                
         
        }

        private void removeLight(UpdateWorldEmun UpdateType)
        {
            // Define the camera's horizontal center for distance calculation
            float camCenterX = this.camColl.bounds.center.x;
            int targetIndex = -1;

            if (UpdateType == UpdateWorldEmun.Left)
            {
                // New world added to the left: Find and remove the light furthest to the RIGHT of the camera
                float maxDistance = float.MinValue;
                for (int i = 0; i < _lights.Count; i++)
                {
                    // Calculate relative distance. Positive values are to the right.
                    float relativeX = _lights[i].position.x - camCenterX;
                    if (relativeX > maxDistance)
                    {
                        maxDistance = relativeX;
                        targetIndex = i;
                    }
                }
            }
            else if (UpdateType == UpdateWorldEmun.Right)
            {
                // New world added to the right: Find and remove the light furthest to the LEFT of the camera
                float minDistance = float.MaxValue;
                for (int i = 0; i < _lights.Count; i++)
                {
                    // Calculate relative distance. Negative values are further to the left.
                    float relativeX = _lights[i].position.x - camCenterX;
                    if (relativeX < minDistance)
                    {
                        minDistance = relativeX;
                        targetIndex = i;
                    }
                }
            }

            // Remove the furthest element if the list is not empty
            if (targetIndex != -1)
            {
                _lights.RemoveAt(targetIndex);
            }
        }

        bool hasUpdate;
        private bool _needInvalidate;
        bool _hasWorldBoudChange;
        private void updateWorldBound()
        {
            //if (vcam.transform.parent != null)
            //{
            //    vcam.transform.SetParent(null, false);
            //}
            //if (cam.transform.parent != null)
            //{
            //    cam.transform.SetParent(null, false);
            //}
            //if(this.compositeCollider2D.transform.parent != null)
            //{
            //    compositeCollider2D.transform.SetParent(null, false);
            //}
            var bounds = this.worldBoundDict.Where(p => p.Key != null && !p.Key.IsDestroying && !p.Key.IsDestroyed()).Select(p => p.Value).ToArray();
          
            this.worldBoundCollider2d.SetPath(0, bounds.CreateRectFromVectorList());
           
            this.worldBoundCollider2d.compositeOperation = Collider2D.CompositeOperation.Merge;
            _hasWorldBoudChange = true;
            this.vcam.PreviousStateIsValid = false;
            

        }

        public void ClearWorlds()
        {
            this.worldBoundDict.Clear();
            this._lights.Clear();
            
        }
        public override void InitEvents()
        {
            //throw new System.NotImplementedException();
        }
        public event Action OnCameraMove;
        public event Action OnCameraStop;
        
        float _lastPosition = 0;
        bool _wasMoving;
       

        public override bool DisablePlayerInput => false;

        public override bool EnablePlayerInput => false;

        float getValue(float x)
        {
            return Mathf.Round(x * 10f) / 10f;
        }
        public override void InitRunTime()
        {
            base.InitRunTime();
            _lastPosition = getValue(cam.transform.position.x);
            audioSource=this.GetComponentInChildren<AudioSource>(true);
            //cam.transparencySortMode = TransparencySortMode.Orthographic;

            //// Thiết lập trục ưu tiên là trục Z (0, 0, 1)
            //cam.transparencySortAxis = new Vector3(0, 0, 1);
            //this.confiner.BoundingShape2D = this.compositeCollider2D;
         //   this.confiner.InvalidateBoundingShapeCache();
           
            

        }
        
        
        void  SayText(string msg)
        {
            if(UnvsApp.Instance != null && UnvsApp.Instance.currentActor != null)
            {
                UnvsApp.Instance.currentActor.SayText(msg);
            }
        }
        private void FixedUpdate()
        {

            //SayText($"this.confiner.BoundingShape2D={this.confiner.BoundingShape2D}");

            if (this.confiner.BoundingShape2D == null)
            {

                this.confiner.BoundingShape2D = this.compositeCollider2D;

            }
           

        }
        public Action OnFirstStart;
        public bool IsStart=true;
        internal bool needFadeOut;

        async void LateUpdate()
        {
            this.confiner.InvalidateBoundingShapeCache();
            float newPos = getValue(cam.transform.position.x);
            // Tính toán độ lệch
            float delta = Mathf.Abs(newPos - _lastPosition);

            // Kiểm tra nếu đang di chuyển (vượt ngưỡng)
            if (delta > 0.01f) // Ngưỡng nhỏ để nhạy hơn
            {
                OnCameraMove?.Invoke();
                
                _wasMoving = true; // Đánh dấu là đang di chuyển
            }
            else
            {
                // Nếu trước đó đang di chuyển mà giờ dừng lại (hoặc dưới ngưỡng)
                if (_wasMoving)
                {
                    OnCameraStop?.Invoke();
                   
                    _wasMoving = false;
                   
                }
            }

            _lastPosition = newPos;

            // Các logic về Light và Tracing giữ nguyên
            this.sceneLoaderTracing.transform.position =new Vector3( this.cam.transform.position.x, this.cam.transform.position.y,0);
            this.centerWatch.transform.position = new Vector3( this.camColl.bounds.center.x, this.camColl.bounds.center.y,0);
            var data = Light2DExtension.MixGlobalLightSources(this.camColl.bounds.center, _lights);
            this.globalLight.intensity = data.Intensity;
            this.globalLight.color = data.Color;
            
            
        }



#if UNITY_EDITOR
        [UnvsButton]
        public void Force()
        {
            GraphicsSettings.transparencySortMode = TransparencySortMode.Perspective;
            GraphicsSettings.transparencySortAxis = new Vector3(0, 0, 1);
            // Tìm đúng file Renderer 2D mà đại ca vừa tạo lại
            var assets = AssetDatabase.FindAssets("t:Renderer2DData");
            Debug.Log("Tìm thấy: " + assets.Length + " file Renderer 2D"); // Thêm dòng này để check
           
            if (assets.Length == 0)
            {
                Debug.LogError("Đéo tìm thấy file nào hết đại ca ơi!");
            }
                foreach (var guid in assets)
            {
                
                var path = AssetDatabase.GUIDToAssetPath(guid);
                Debug.Log($"file={path}");
                var data = AssetDatabase.LoadAssetAtPath<Renderer2DData>(path);

                // Dùng SerializedObject để chọc vào biến ẩn m_TransparencySortMode
                SerializedObject so = new SerializedObject(data);
                so.FindProperty("m_TransparencySortMode").intValue = (int)TransparencySortMode.Perspective;
                so.FindProperty("m_TransparencySortAxis").vector3Value = new Vector3(0, 0, 1);
                so.ApplyModifiedProperties();
            }
            AssetDatabase.SaveAssets();
            Debug.Log("Đã mở khóa trục Z chuẩn Studio cho Renderer 2D!");
        }
        [UnvsButton()]
        public void Generate()
        {
            var cinema = this;
            cinema.cam = cinema.AddChildComponentIfNotExist<Camera>("Main Camera");
            this.physics2DRaycaster= cinema.cam.AddComponentIfNotExist<Physics2DRaycaster>();
            cinema.cam.tag = "MainCamera";
            cinema.cam.orthographic = true;
            cinema.cam.AddComponentIfNotExist<CinemachineBrain>();
            cinema.cam.AddComponentIfNotExist<AudioListener>();
            cinema.vcam = cinema.AddChildComponentIfNotExist<CinemachineCamera>("VCam");
            cinema.vcam.AddComponentIfNotExist<CinemachineFollow>();
            cinema.compositeCollider2D = cinema.AddChildComponentIfNotExist<CompositeCollider2D>("compositeCollider2D");

            cinema.compositeCollider2D.geometryType = CompositeCollider2D.GeometryType.Polygons;
            cinema.confiner = cinema.vcam.AddComponentIfNotExist<CinemachineConfiner2D>();
            cinema.confiner.BoundingShape2D = cinema.compositeCollider2D;
          
            
            cinema.worldBoundCollider2d = this.AddChildComponentIfNotExist<PolygonCollider2D>("worldBoundCollider2d");
            cinema.worldBoundCollider2d.compositeOperation = Collider2D.CompositeOperation.Merge;
         
            cinema.worldBoundCollider2d.transform.SetParent(cinema.compositeCollider2D.transform);
            cinema.compositeCollider2D.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            cinema.compositeCollider2D.isTrigger = true;
            this.sceneLoaderTracing = this.AddChildComponentIfNotExist<Transform>("sceneLoaderTracing");
            var b = this.sceneLoaderTracing.AddComponentIfNotExist<Rigidbody2D>();
            b.bodyType = RigidbodyType2D.Kinematic;
            b.gravityScale = 0;
            b.angularDamping = 0;
            var c = b.AddComponentIfNotExist<BoxCollider2D>();
            c.SetMeOnTag(Constants.Tags.TRIGGER_LOAD_SCENE);
            c.isTrigger = true;
            camColl = c;
            c.size = cam.GetCameraWorldSize();
            this.centerWatch = this.AddChildComponentIfNotExist<Transform>("center-watch");
            var rb = this.centerWatch.AddComponentIfNotExist<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0;
            rb.angularDamping = 0;
            var cwc = this.centerWatch.AddComponentIfNotExist<BoxCollider2D>();
            cwc.isTrigger = true;
            cwc.size = new Vector2(0.1f, 0.1f);
            cwc.SetMeOnTag(Constants.Tags.TRIGGER_SCENE_CHANGE);
            cwc.SetMeOnTag(Constants.Layers.TRIGGER_SCENE_CHANGE);
            cwc.isTrigger = true;
            this.centerCamTracing= cwc;
            this.globalLight = this.AddChildComponentIfNotExist<Light2D>("globalLight");
            this.globalLight.lightType = Light2D.LightType.Global;
            this.globalLight.enabled = true;
            this.audioSource = this.AddChildComponentIfNotExist<AudioSource>("audio-source");
            
          
            

        }


        public override void OnDrawGizmos()
        {
            var coll = this.worldBoundCollider2d.GetComponentInChildren<PolygonCollider2D>();
            if(coll!=null)
            {
                coll.GizmosDraw(Color.yellow);
            }
        }

        public void UpdateLoadChunkSceneTrackerSize(Vector3 followOfsset)
        {
            this.vcam.PreviousStateIsValid = false;
            vcam.GetComponent<CinemachineFollow>().FollowOffset = followOfsset;
            
            UnvsCinema.Instance.camColl.size = UnvsCinema.Instance.cam.GetCameraWorldSize();
        }

        public void UpdateLoadChunkSceneTrackerSizeByCurrentFollowOffset()
        {
            UnvsCinema.Instance.camColl.size = UnvsCinema.Instance.cam.GetCameraWorldSize();
        }








#endif
    }
}