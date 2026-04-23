using Cysharp.Threading.Tasks;

using game2d.scenes;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using Unity.Cinemachine;
using Unity.Mathematics;
using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using unvs.ext;
using unvs.game2d.scenes.actors;

using unvs.shares;

namespace unvs.game2d.scenes{
    public class UnvsCinema : UnvsUIComponentInstance<UnvsCinema>
    {
        [Header("Cinema light")]
        public float DurationTimeSmoothChangeSate = 1.5f;
        public int MaintainGlobalLightNumber = 5;
        [SerializeField]
        public GlobalLightChunkInfo[] worldLightMaintain;
        private List<GlobalLightChunkInfo> _lights = new List<GlobalLightChunkInfo>();
        public event Action<UnvsScene> BeforeUpdate;
        public event Action<UnvsScene> AfterUpdate;
        public Camera cam;
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
            
            if (cam.orthographic)
            {
                bool flowControl = chaneCameSteByOrthoSize(s, Imediately);
                if (!flowControl)
                {
                    return;
                }
            } else
            {
                camColl.size = cam.GetCameraWorldSize();
                chaneCameSteByOffset(s, Imediately);
            }
                
            //ChangeCameraStateAsync(s).Forget();
        }
        private bool chaneCameSteByOffset(List<UnvsScene> s, bool Imediately)
        {
            UnvsScene nearset = null;
            if (Imediately)
            {

                
                vcam.GetComponent<CinemachineFollow>().FollowOffset = new Vector3(s[0].followOffset.x, s[0].followOffset.y, cam.OrthoSizeToPerspectiveDistance(s[0].OrthographicSize));

                float height = s[0].OrthographicSize * 2f;
                float width = height * cam.aspect; // cam.aspect = Screen.width / Screen.height
                camColl.size = new Vector2((float)width, (float)height);
                confiner.InvalidateBoundingShapeCache();
                return false;
            }
            nearset = CalculateNearestScene(s);
            ctsChangeOffset = ctsChangeOffset.Refresh();
            //ctsChangeOrthoSize = ctsChangeOrthoSize.Refresh();
            var newOffset= new Vector3(nearset.followOffset.x, nearset.followOffset.y, cam.OrthoSizeToPerspectiveDistance(nearset.OrthographicSize));
            vcam.SetFollowOffsetSmoothlyAsync(newOffset, 3,DurationTimeSmoothChangeSate, ctsChangeOffset.Token).ContinueWith(() =>
            {
                camColl.size = cam.GetCameraWorldSize();
                confiner.InvalidateBoundingShapeCache();
                
            }).Forget();

            return true;
        }
        private bool chaneCameSteByOrthoSize(List<UnvsScene> s, bool Imediately)
        {
            UnvsScene nearset = null;
            if (Imediately)
            {

                vcam.SetOrthoSizeImmediate(s[0].OrthographicSize);
                vcam.GetComponent<CinemachineFollow>().FollowOffset = s[0].followOffset;

                float height = s[0].OrthographicSize * 2f;
                float width = height * cam.aspect; // cam.aspect = Screen.width / Screen.height
                camColl.size = new Vector2((float)width, (float)height);
                return false;
            }
            nearset = CalculateNearestScene(s);
            ctsChangeOffset = ctsChangeOffset.Refresh();
            ctsChangeOrthoSize = ctsChangeOrthoSize.Refresh();
            if (vcam.Lens.OrthographicSize > nearset.OrthographicSize)
            {
                vcam.ChangeFollowOffsetSmoothAsync(nearset.followOffset, ctsChangeOffset.Token, DurationTimeSmoothChangeSate).Forget();
                vcam.SetOrthoSizeSmoothlyAsync(nearset.OrthographicSize, DurationTimeSmoothChangeSate, 3, ctsChangeOrthoSize.Token).ContinueWith(() =>
                {
                    camColl.size = cam.GetCameraWorldSize();
                }).Forget();

            }
            else
            {
                vcam.SetOrthoSizeSmoothlyAsync(nearset.OrthographicSize, DurationTimeSmoothChangeSate, 3, ctsChangeOrthoSize.Token)
                    .ContinueWith(() => { camColl.size = cam.GetCameraWorldSize(); }).Forget();
                vcam.ChangeFollowOffsetSmoothAsync(nearset.followOffset, ctsChangeOffset.Token, DurationTimeSmoothChangeSate).Forget();
            }

            return true;
        }

        public async UniTask ChangeCameraStateAsync(List<UnvsScene> s)
        {
            UnvsScene nearset = CalculateNearestScene(s);
            ctsChangeOffset = ctsChangeOffset.Refresh();
            ctsChangeOrthoSize = ctsChangeOrthoSize.Refresh();
            if(vcam.Lens.OrthographicSize> nearset.OrthographicSize)
            {
                await vcam.ChangeFollowOffsetSmoothAsync(nearset.followOffset, ctsChangeOffset.Token);
                await vcam.SetOrthoSizeSmoothlyAsync(nearset.OrthographicSize, -1, 3, ctsChangeOrthoSize.Token);
               
            }
            else
            {
                await vcam.SetOrthoSizeSmoothlyAsync(nearset.OrthographicSize, -1, 3, ctsChangeOrthoSize.Token);
                await vcam.ChangeFollowOffsetSmoothAsync(nearset.followOffset, ctsChangeOffset.Token);
            }
                
            camColl.size = cam.GetCameraWorldSize();
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
        /// Update chunk load scene tracker size
        /// </summary>
        public void UpdateMainCameraBoxCollider2dSize()
        {


            if (cam.orthographic)
            {
                float orthoSize = vcam.Lens.OrthographicSize;
                float aspect = vcam.Lens.Aspect;


                float height = orthoSize * 2f;
                float width = height * aspect;


                camColl.size = new Vector2(width, height);


                camColl.isTrigger = true;


                camColl.offset = Vector2.zero;
            } else
            {
                float planeZ = 0f;
                float distance = Mathf.Abs(cam.transform.position.z - planeZ);
                // Nếu vẫn bằng 0 (do cả 2 chưa init), hãy bỏ qua frame này hoặc dùng giá trị mặc định
                if (distance < 0.1f)
                {
                    // Có thể đây là frame đầu tiên khi load, hãy đợi hoặc tạm dừng tính toán
                    return;
                }
                // Truy cập trực tiếp vào Lens của Virtual Camera để đảm bảo đồng bộ
                float currentFOV = vcam.Lens.FieldOfView;
                float aspect = vcam.Lens.Aspect;

                // Tính toán bằng Radians
                float halfFovRad = currentFOV * 0.5f * Mathf.Deg2Rad;
                float tanAlpha = Mathf.Tan(halfFovRad);

                float height = 2f * distance * tanAlpha;
                float width = height * aspect;

                // Log này sẽ cho bạn biết chính xác biến số nào đang "hủy diệt" kích thước của bạn
                if (width < 1 || height < 1)
                {
                    Debug.LogError($"[Fix] Dist: {distance}, FOV: {currentFOV}, Tan: {tanAlpha}, Aspect: {aspect}");
                }

                camColl.size = new Vector2(width, height);
            }
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
            //Action<UnvsScene> OnSceneDestroyTmp = null;
            //Action<UnvsScene> OnSceneDestroy = (s) =>
            //{
            //    this.worldBoundDict.Remove(s);
            //    //UpdateWorldBoundAsync().Forget();
            //    this.lightDict.Remove(s);
            //    ret.OnDestroying -= OnSceneDestroyTmp;
            //    //requestUpdate = true;

            //};
            //OnSceneDestroyTmp = OnSceneDestroy;
            //ret.OnDestroying += OnSceneDestroy;

            //_lights = this.lightDict.Select(p => p.Value).ToArray();
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
        private void updateWorldBound()
        {
            
            var bounds = this.worldBoundDict.Where(p => p.Key != null && !p.Key.IsDestroying && !p.Key.IsDestroyed()).Select(p => p.Value).ToArray();
          
            this.worldBoundCollider2d.SetPath(0, bounds.CreateRectFromVectorList());
            this.confiner.InvalidateBoundingShapeCache();

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

            Debug.Log("Camera Transparency Sort Mode set to Orthographic Axis");

        }
        //void OnPreRender() // Trước khi render bất cứ thứ gì
        //{
            
        //    // Ép cách tính Z luôn nhất quán, bất kể bạn thêm Layer nào sau này
        //    cam.transparencySortMode = TransparencySortMode.Orthographic;
        //    cam.transparencySortAxis = new Vector3(0, 0, 1);
        //}

        private void LateUpdate()
        {
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
        [UnvsButton()]
        public void Generate()
        {
            var cinema = this;
            cinema.cam = cinema.AddChildComponentIfNotExist<Camera>("Main Camera");
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
            confiner.BoundingShape2D = cinema.worldBoundCollider2d;
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

       









#endif
    }
}