
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using Script.unvs.ext;
using System;
using System.Collections;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using unvs.ext;
using unvs.gameword;
using unvs.interfaces;
using unvs.interfaces.sys;
using unvs.shares;

namespace unvs.manager
{

    public class SceneLoaderManager : MonoBehaviour, ISceneLoader
    {
        private IChunkScenes chunks;
        public GameObject goChunks;
        public Transform tempLoader;
        public Transform interiorScene;
        public Transform backupInteriror;
        public Transform tempDelete;
        public int horizontalChunkSize = 3;
        public Transform actorPlaceHolder;
        public Transform backupGlobalLight;

        public event Action<IScenePrefab, LoadTypeEnum> OnLoadBegin;
        public event Action<IScenePrefab, LoadTypeEnum> OnLoadComplete;

        public ISceneLoader Instance { get; private set; }

        public IChunkScenes Chunks => chunks;

        public IActorObject CurrentActor { get; set; }

        public int HorizontalChunkSize => horizontalChunkSize;

        public IScenePrefab CurrentScene { get; set; }

        public Transform ActorPlaceHolder => actorPlaceHolder;

        public IScenePrefab LastInteriorScene { get; set; }

        private void Awake()
        {
            Instance = this as ISceneLoader;
            GlobalApplication.SceneLoaderManagerInstance = Instance;
            chunks = this.AddChildIfNotExist<ChunksSceneObject>(Constants.ObjectsConst.CHUNK_SCENES);
            goChunks = (chunks as MonoBehaviour)?.gameObject;
            tempLoader = this.AddChildComponentIfNotExist<Transform>(Constants.ObjectsConst.CHUNK_SCENES_TEMP_LOADER);
            tempLoader.gameObject.SetActive(false);
            tempDelete = this.AddChildComponentIfNotExist<Transform>(Constants.ObjectsConst.CHUNK_SCENES_TEMP_DELETE);
            tempDelete.gameObject.SetActive(false);
            var interiorSceneObject = this.AddChildComponentIfNotExist<InteriorSceneContainerObject>(Constants.ObjectsConst.INTERIOR_SCENE);
            interiorScene = (interiorSceneObject as MonoBehaviour).transform;

            backupInteriror = this.AddChildComponentIfNotExist<Transform>(Constants.ObjectsConst.BACKUP_INTERIOR_SCENE);
            backupInteriror.gameObject.SetActive(false);
            actorPlaceHolder = this.AddChildComponentIfNotExist<Transform>(Constants.ObjectsConst.ACTOR_PLACE_HOLDER);
            backupGlobalLight = this.AddChildComponentIfNotExist<Transform>(Constants.ObjectsConst.BACKUP_GLOBAL_LIGHT);
            backupGlobalLight.gameObject.SetActive(false);
        }
        #region Implement of ISceneLoader
        public async UniTask ClearChunksAsync()
        {
            if (GlobalWorldBound.Instance.isMultiPolygon)
            {
                foreach (var chunk in (chunks as MonoBehaviour).GetComponentsInChildren<IScenePrefab>(true))
                {
                    chunk.IsDestroying = true;
                    IScenePrefabWorldBound worldBound = GlobalWorldBound.Instance.FindByOwner(chunk);
                    (worldBound as MonoBehaviour).transform.SetParent(chunk.GoWorld.transform.transform);
                    //chunk.WorldBound.Coll.transform.SetParent(chunk.GoWorld.transform.transform);
                    chunk.GoWorld.transform.SetParent(tempDelete.transform, false);
                }
            }

            await tempDelete.SafeDestroyChildrenAsync();
        }
        public async UniTask ClearAsync()
        {
            await ClearChunksAsync();

            await interiorScene.SafeDestroyChildrenAsync();
            await backupInteriror.SafeDestroyChildrenAsync();



        }

        public async UniTask<IScenePrefab> LoadInteriorAsync(string pathToWord, string targetName, IScenePrefab FromScene = null)
        {
            var actor = actorPlaceHolder.GetComponentInChildren<IActorObject>();
            if (actor != null)
            {
                (actor as MonoBehaviour).gameObject.SetActive(false);
            }


            if (FromScene != null)
            {
                if (!FromScene.IsInteriorScene())
                {

                    FromScene.GlobalightRestore();
                    FromScene.WorldBound.Restore();
                    FromScene.GoWorld.transform.SetParent(backupInteriror.transform);

                    FromScene.LeftTriggerZone.On();
                    FromScene.RightTriggerZone.On();
                    FromScene.LeftWall.isTrigger = !string.IsNullOrEmpty(FromScene.GetLeftScenePath());
                    FromScene.RightWall.isTrigger = !string.IsNullOrEmpty(FromScene.GetRightScenePath());



                }
            }
            await ClearChunksAsync();
            GlobalApplication.WorldTrackerObject?.Cts?.Stop();
            IScenePrefab scene = null;
            (this.chunks as MonoBehaviour).gameObject.SetActive(false);
            scene = backupInteriror.gameObject.GetComponentInChildrenByName<IScenePrefab>(pathToWord);
            if (scene == null)
            {
                scene = await Commons.LoadPrefabsAsync<IScenePrefab>(pathToWord, interiorScene);
            }
            else
            {
                scene.GoWorld.SetActive(false);
                scene.GoWorld.transform.SetParent(interiorScene, true);
            }
            if (Instance.LastInteriorScene != null && !Instance.LastInteriorScene.IsDestroying)
            {
                Instance.LastInteriorScene.GlobalightRestore();
                Instance.LastInteriorScene.WorldBound.Restore();
                GlobalApplication.LightManagerObjectInstance.FindLightByScene(Instance.LastInteriorScene);
                Instance.LastInteriorScene.GoWorld.transform.SetParent(backupInteriror.transform, true);
            }
            DisableWorldBound();
            GlobalApplication.LightManagerObjectInstance.SetLight(scene.Globalight.GlobalLight);



            DisableConflictController(scene);
            Instance.LastInteriorScene = scene;

            ISpawnTarget target = Instance.LastInteriorScene.FindSpawnTargetNullReturnStartPos(targetName);


            this.EpxandWorldBoundHorizontalBeforeAddGlobalWorldBound(scene);
            scene.TrimEdge();
            if (GlobalWorldBound.Instance.isMultiPolygon)
                GlobalWorldBound.Instance.AddBound(scene);
            else GlobalWorldBound.Instance.SetBound(scene);
            if (GlobalWorldBound.Instance.isMultiPolygon)
                SingleScene.Instance.VCam.SetOrthoSizeImmediate(scene.OrthographicSize);
            else
                SingleScene.Instance.VCam.UpdateByScenePrefab(scene);
            if (actor != null)
            {
                target.MoveOtherToMe(actor as MonoBehaviour);
                SingleScene.Instance.VCam.Watch(actor.CamWacher);
            }

            scene.GoWorld.SetActive(true);
            scene.WorkTracker.On();
            //await UniTask.DelayFrame(5, PlayerLoopTiming.Update);


            if (actor != null)
            {
                (actor as MonoBehaviour).gameObject.SetActive(true);
            }

            return scene;
        }

        public void BackupGlobalLight()
        {
            foreach (var item in GlobalApplication.LightManagerObjectInstance.gameObject.GetComponentsInChildren<IGlobalLightWapper>())
            {
                (item as MonoBehaviour).transform.SetParent(backupGlobalLight.transform, true);
            }
        }

        public async UniTask<IScenePrefab> LoadNewAsync(string pathToWord, string targetName)
        {
            await ClearAsync();

            (chunks as MonoBehaviour).gameObject.SetActive(true);
            await GlobalApplication.FadeScreenController.FadeInAsync();
            GlobalApplication.WorldTrackerObject?.Cts?.Stop();


            var scene = await Commons.LoadPrefabsAsync<IScenePrefab>(pathToWord, this.tempLoader);
            OnLoadBegin?.Invoke(scene, LoadTypeEnum.New);


            scene.TrimEdge();
            Instance.SetupLayout(scene);
            Instance.SetUpEnvironment(scene);


            EpxandWorldBoundHorizontalBeforeAddGlobalWorldBound(scene);

            GlobalWorldBound.Instance.SetBound(scene);

            scene.GoWorld.SetActive(true);
            OnLoadComplete?.Invoke(scene, LoadTypeEnum.New);

            LightManagerObject.Add(scene);

            this.InitActor(scene, targetName);
            if (GlobalWorldBound.Instance.isMultiPolygon)
                SingleScene.Instance.VCam.SetOrthoSizeImmediate(scene.OrthographicSize);
            else
                SingleScene.Instance.VCam.UpdateByScenePrefab(scene);
            await GlobalApplication.FadeScreenController.FadeOutAsync();

            return scene;

        }




        public bool PlayActiveSceneFromEditor()
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorPrefs.HasKey("PendingWorldPath"))
            {
                // Lấy đường dẫn ra
                string path = UnityEditor.EditorPrefs.GetString("PendingWorldPath");

                // Xóa ngay lập tức để lần sau nhấn Play bình thường 
                // nó không tự động load lại cái world cũ này
                UnityEditor.EditorPrefs.DeleteKey("PendingWorldPath");

                if (!string.IsNullOrEmpty(path))
                {

                    LoadNewAsync(path, null).Forget();
                    return true;
                }

            }

            return false;
#else
        return false;
#endif
        }



        public void InitActor(IScenePrefab scene, string targetName)
        {
            var actor = scene.Actor;
            var actorBehaviour = actor as MonoBehaviour;
            if (actorBehaviour != null && actor.IsActive)
            {
                //Instance.CurrentActor = actor;

                var spawn = scene.FindSpawnTargetByName(targetName);
                if (spawn == null) spawn = scene.StartPos;

                // kiem tra vi sao Instance.ActorPlaceHolder kg ghe add actor vao
                // Tra loi: Neu 'actor' dang la root object va duoc mark la DontDestroyOnLoad hoac Object tren Scene bi xoa,
                // hoac khi dung SetParent(..., false) - o day la (..., true) thi actorBehaviour phi li ko doi.
                actorBehaviour.transform.SetParent(Instance.ActorPlaceHolder, true);

                if (spawn != null)
                {
                    spawn.MoveOtherToMe(actorBehaviour);
                }

                var camWatcher = actor.CamWacher as MonoBehaviour;
                if (camWatcher != null)
                {
                    SingleScene.Instance.VCam.Watch(camWatcher.transform);
                }
                actor.OnDestroying = () =>
                {
                    SLog.Info($"destroy actor {actor}");
                };
                (actor as MonoBehaviour).GetComponent<SortingGroup>().sortAtRoot = true;

            }
        }

        public void SetupLayout(IScenePrefab scene)
        {
            var offset = (this.chunks as MonoBehaviour).transform.position - scene.GoWorld.transform.position;
            scene.GoWorld.SetActive(false);
            scene.GoWorld.transform.SetParent((this.chunks as MonoBehaviour).transform, false);
            scene.JoinInfo.WorldJoinInfo.LeftPos += (Vector2)offset;
            scene.JoinInfo.WorldJoinInfo.RightPos += (Vector2)offset;


        }

        public void SetUpEnvironment(IScenePrefab scene)
        {
            scene.Globalight.GlobalLight.TunOff();
            LightManagerObject.instance.GlobalLight.color = scene.Globalight.GlobalLight.Light.color;
            LightManagerObject.instance.GlobalLight.intensity = scene.Globalight.GlobalLight.Light.intensity;




        }

        public void SetUpCam(IScenePrefab scene)
        {
            SingleScene.Instance.Cam.GetComponent<ICam>().Body.UpdateSizeByLensSettings(SingleScene.Instance.VCam.Lens);
        }

        public async UniTask<IScenePrefab> LoadChunksAsync(ITriggerZone triggerZone)
        {
            var scene = (triggerZone as MonoBehaviour).GetComponentInParent<IScenePrefab>();
            if (scene == null) return null;
            if (scene.IsInteriorScene())
            {
                // start new game level routine
                await Instance.ClearChunksAsync();

                if (triggerZone.Direction == TriggerZoneDirection.Right)
                    scene.RightTriggerZone.Off();
                if (triggerZone.Direction == TriggerZoneDirection.Left)
                    scene.LeftTriggerZone.Off();
                scene.GoWorld.transform.SetParent((this.chunks as MonoBehaviour).transform, true);
                (this.chunks as MonoBehaviour).gameObject.SetActive(true);
                await interiorScene.SafeDestroyChildrenAsync();
                await backupInteriror.SafeDestroyChildrenAsync();

            }
            if (triggerZone.Direction == TriggerZoneDirection.Right)
                return await GlobalApplication.SceneLoaderManagerInstance.LoadRightSceneAsync(triggerZone);
            else if (triggerZone.Direction == TriggerZoneDirection.Left)
                return await GlobalApplication.SceneLoaderManagerInstance.LoadLeftSceneAsync(triggerZone);
            return null;
        }
        public async UniTask<IScenePrefab> LoadRightSceneAsync(ITriggerZone zone)
        {
            var scene = (zone as MonoBehaviour).GetComponentInParent<IScenePrefab>();
            if (scene.IsDestroying)
            {
                return null;
            }
            if (scene.IsInteriorScene())
            {
                scene.GoWorld.transform.SetParent((this.chunks as MonoBehaviour).transform, true);
                GlobalWorldBound.Instance.AddBound(scene);
                LastInteriorScene = null;
                ClearAllInteriorScenes().Forget();

            }
            // check if left scene in chunks limit free it
            await this.chunks.CheckLeftAsync(this.tempDelete, this.horizontalChunkSize);

            OnLoadBegin?.Invoke(scene, LoadTypeEnum.Right);
            scene.RightWall.isTrigger = true;
            scene.RightTriggerZone.Off();
            
            var rightScene = await Commons.LoadPrefabsAsync<IScenePrefab>(
                scene.GetRightScenePath(),
                this.tempLoader.transform);
            DisableConflictController(rightScene);
            rightScene.LeftWall.isTrigger = true;
            rightScene.LeftTriggerZone.Off();
            rightScene.Left = scene;
            scene.Right = rightScene;
            Instance.SetupLayout(rightScene);
            Instance.SetUpEnvironment(rightScene);
            rightScene.TrimEdge();

            EpxandWorldBoundHorizontalBeforeAddGlobalWorldBound(rightScene);
            MoveRightToLeft(rightScene, scene);


            var coll = rightScene.WorldBound.Coll;
            GlobalWorldBound.Instance.AddBound(rightScene);

            rightScene.GoWorld.SetActive(true);
            rightScene.GoWorld.transform.SetAsLastSibling();
            LightManagerObject.Add(rightScene);
            OnLoadComplete?.Invoke(scene, LoadTypeEnum.Right);
            rightScene.WorkTracker?.On();
            return scene;
        }

        private async UniTask ClearAllInteriorScenes()
        {
            await backupInteriror.SafeDestroyChildrenAsync();
            await interiorScene.SafeDestroyChildrenAsync();
        }

        public async UniTask<IScenePrefab> LoadLeftSceneAsync(ITriggerZone zone)
        {
            var scene = (zone as MonoBehaviour).GetComponentInParent<IScenePrefab>();
            if (scene.IsDestroying) return null;
            scene.LeftWall.isTrigger = true;
            scene.LeftTriggerZone.Off();
            if (scene.IsInteriorScene())
            {
                scene.GoWorld.transform.SetParent((this.chunks as MonoBehaviour).transform, true);
                GlobalWorldBound.Instance.AddBound(scene);
                LastInteriorScene = null;
                ClearAllInteriorScenes().Forget();

            }


            // check if right chunk is limit free it
            await this.chunks.CheckRightAsync(this.tempDelete, this.horizontalChunkSize);


            var leftScene = await Commons.LoadPrefabsAsync<IScenePrefab>(
                scene.GetLeftScenePath(),
                this.tempLoader.transform);
            leftScene.RightWall.isTrigger = true;
            leftScene.RightTriggerZone.Off();
            leftScene.Right = scene;
            scene.Left = leftScene;
            DisableConflictController(leftScene);
            OnLoadBegin?.Invoke(scene, LoadTypeEnum.Left);
            Instance.SetupLayout(leftScene);
            Instance.SetUpEnvironment(leftScene);
            leftScene.TrimEdge();
            EpxandWorldBoundHorizontalBeforeAddGlobalWorldBound(leftScene);
            MoveLeftToRight(leftScene, scene);
            LightManagerObject.Add(leftScene);

            var coll = leftScene.WorldBound.Coll;
            GlobalWorldBound.Instance.AddBound(leftScene);
            if (scene.LeftWall.gameObject.IsDestroyed()) return null;

            leftScene.GoWorld.SetActive(true);
            leftScene.GoWorld.transform.SetAsFirstSibling();
            OnLoadComplete?.Invoke(scene, LoadTypeEnum.Left);
            leftScene.WorkTracker.On();
            return scene;

        }
        #endregion
        #region Private supports
        private static void MoveLeftToRight(IScenePrefab left, IScenePrefab right)
        {
            if (left == null || right == null || left.IsDestroying || right.IsDestroying) return;
            if (left.JoinInfo == null)
            {
                throw new Exception($"JoinInfo of left={left.Name} is null");
            }
            if (left.JoinInfo.WorldJoinInfo == null)
            {
                throw new Exception($"JoinInfo.WorldJoinInfo of left={left.Name} is null");
            }
            if (right.JoinInfo == null)
            {
                throw new Exception($"JoinInfo of right={right.Name} is null");
            }
            if (right.JoinInfo.WorldJoinInfo == null)
            {
                throw new Exception($"JoinInfo.WorldJoinInfo of left={right.Name} is null");
            }
            //var targetPoint = scene.RightPosJoin;

            //// Điểm Hiện Tại của Scene mới (B): Mép bên trai của sàn Scene mới
            //var currentPoint = otherScene.LeftPosJoin;

            //// 3. Tính toán Snap Offset (Đích - Tại)
            //// Di chuyển sao cho currentPoint đè khít lên targetPoint
            //Vector3 snapOffset = targetPoint.position - currentPoint.position;

            // Áp dụng dịch chuyển cho toàn bộ root của Scene mới
            var offset = left.JoinInfo.WorldJoinInfo.RightPos - right.JoinInfo.WorldJoinInfo.LeftPos;
            left.GoWorld.transform.position -= (Vector3)offset;
            left.JoinInfo.WorldJoinInfo.Move(-offset);



        }
        private static void MoveRightToLeft(IScenePrefab right, IScenePrefab left)
        {
            if(right==null) throw new Exception($"right={right}");
            if (left == null) throw new Exception($"right={left}");
            if (right.JoinInfo == null) throw new Exception($"right.JoinInfo={right.JoinInfo}");
            if (left.JoinInfo == null)
            {
                Debug.LogError($"right.JoinInfo={left.JoinInfo}");
                return;
                
            }
            // Áp dụng dịch chuyển cho toàn bộ root của Scene mới
            var offset = right.JoinInfo.WorldJoinInfo.LeftPos - left.JoinInfo.WorldJoinInfo.RightPos;
            right.GoWorld.transform.position -= (Vector3)offset;
            right.JoinInfo.WorldJoinInfo.Move(-offset);


        }
        private void DisableWorldBound()
        {
            if (GlobalWorldBound.Instance.isMultiPolygon)
            {
                foreach (var item in (Chunks as MonoBehaviour).gameObject.GetComponentsInChildren<IScenePrefab>())
                {
                    item.WorldBound.Coll.gameObject.SetActive(false);
                }
                foreach (var item in backupInteriror.GetComponentsInChildren<IScenePrefab>())
                {
                    item.WorldBound.Coll.gameObject.SetActive(false);
                }
            }
            else
            {
                GlobalWorldBound.Instance.Clear();
            }
        }
        private void EnabelWorldBound()
        {
            foreach (var item in (Chunks as MonoBehaviour).gameObject.GetComponentsInChildren<IScenePrefab>())
            {
                item.WorldBound.Coll.gameObject.SetActive(true);
            }
            foreach (var item in backupInteriror.GetComponentsInChildren<IScenePrefab>())
            {
                item.WorldBound.Coll.gameObject.SetActive(true);
            }
        }
        private void DisableConflictController(IScenePrefab scene)
        {
            var currentCharactor = actorPlaceHolder.GetComponentInChildren<IActorObject>();
            foreach (var item in scene.GoWorld.GetComponentsInChildren<IActorObject>())
            {
                if ((currentCharactor as MonoBehaviour).name == (item as MonoBehaviour).name)
                {
                    UnityEngine.Object.Destroy((item as MonoBehaviour).gameObject);
                }
                else if (item.Controller != null)
                {

                    (item.Controller as MonoBehaviour).enabled = false;
                }
                else
                {
                    item.IsActive = false;
                }
                (item as MonoBehaviour).SetMeOnLayer(Constants.Layers.NPC);
            }
        }
        private void EpxandWorldBoundHorizontalBeforeAddGlobalWorldBound(IScenePrefab scene, float dx = 2)
        {
            var facetLeft = scene.JoinInfo.WorldJoinInfo.WorldFacets.Left;
            var left = scene.JoinInfo.WorldJoinInfo.LeftPos;

            scene.WorldBound.Coll.VerticalFacetAlign(facetLeft.Start, facetLeft.End, left.x - dx);
            var facetRight = scene.JoinInfo.WorldJoinInfo.WorldFacets.Right;
            var right = scene.JoinInfo.WorldJoinInfo.RightPos;
            scene.WorldBound.Coll.VerticalFacetAlign(facetRight.Start, facetRight.End, right.x + dx);
        }

        public async UniTask ClearAllAsync()
        {
            await ClearAsync();
            await this.actorPlaceHolder.SafeDestroyChildrenAsync();
        }


        #endregion
    }
}