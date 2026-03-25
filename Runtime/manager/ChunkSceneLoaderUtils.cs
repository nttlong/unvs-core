using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using unvs.ext;
using unvs.gameword;
//using unvs.gameword.manager;
using unvs.interfaces;
using unvs.shares;


//using UnityEngine.AddressableAssets;
namespace unvs.manager
{

    [Obsolete("IActor is deprecated. Please use IEntity for the new IActorObject system instead.")]
    public class ChunkSceneLoaderUtilsOld
    {
        private static Transform tmp;
        public static event Action<IScenePrefab> OnLoadNew;
        public static Transform TempObject
        {
            get
            {
                
                if (tmp != null)
                {
                    return tmp;
                }
                var go = new GameObject("ChunkSceneLoaderUtils-tmp");
                go.SetActive(false);
                tmp = go.transform;
                return tmp;
            }
        }

        public static GameObject InteriorGameObject
        {
            get
            {
                if (interiorGameObject != null) return interiorGameObject;
                interiorGameObject = new GameObject("InteriorGameObject");
                return interiorGameObject;
            }
        }

        public static GameObject BackupInterior
        {
            get
            {
                if (backupInterior != null) return backupInterior;
                backupInterior = new GameObject(unvs.shares.Constants.ObjectsConst.BACKUP_INTERIOR_SCENE);
                backupInterior.SetActive(false);
                return backupInterior;
            }

        }

        public static Transform DeleteTempObject
        {
            get
            {
                if (deleteTempObject != null) return deleteTempObject;
                var go = new GameObject(unvs.shares.Constants.ObjectsConst.BACKUP_INTERIOR_SCENE);
                go.SetActive(false);
                deleteTempObject = go.transform;
                return deleteTempObject;
            }
        }

        public const int MAX_BUFFER_SIZE = 3;

        public static IScenePrefab[] chunkList = new IScenePrefab[MAX_BUFFER_SIZE];
        public static int currentIndex = 0;
        private static void HandleNeighborConnections(IScenePrefab sceneToUnload)
        {
            // Xử lý bên PHẢI
            if (sceneToUnload.Right != null)
            {
                var neighbor = sceneToUnload.Right;
                neighbor.Left = null;
                neighbor.LeftWall.isTrigger = false;
                if (neighbor.LeftTriggerZone != null) neighbor.LeftTriggerZone.On();
            }
            // Xử lý bên TRÁI
            if (sceneToUnload.Left != null && !sceneToUnload.Left.IsDestroying)
            {
                var neighbor = sceneToUnload.Left;
                neighbor.Right = null;
                neighbor.RightWall.isTrigger = false;
                if (neighbor.RightTriggerZone != null) neighbor.RightTriggerZone.On();
            }
        }
        public static async UniTask UnloadSceneAsync(IScenePrefab sceneToUnload)
        {

            if (sceneToUnload == null) return;
            sceneToUnload.GoWorld.transform.SetParent(DeleteTempObject);
            sceneToUnload.IsDestroying = true;
            var instance = SingleScene.Instance;
            LightManagerObject.Remove(sceneToUnload);
            // 1. NGẮT KẾT NỐI VÀ HÀN GẮN THẾ GIỚI (Chạy đồng bộ vì nhẹ)
            HandleNeighborConnections(sceneToUnload);

            // 2. DỌN DẸP HỆ THỐNG PHỤ TRỢ
            if (instance?.GlobalWorldBound != null)
            {
                instance.GlobalWorldBound.RemoveBound(sceneToUnload);
                // Chờ đến cuối frame để hệ thống vật lý và Confiner ổn định
                await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);
            }

            // 3. GIẢI PHÓNG BỘ NHỚ
            if (sceneToUnload.GoWorld != null)
            {
                // Tắt ngay lập tức để giải phóng CPU khỏi việc render/update logic
                sceneToUnload.GoWorld.SetActive(false);

                // Đợi 1 frame để Unity rảnh tay thực hiện Destroy
                await UniTask.NextFrame();

                // Xóa Scene. Dùng Destroy bình thường, UniTask sẽ quản lý luồng
                UnityEngine.Object.Destroy(sceneToUnload.GoWorld);
            }

            // 4. LOẠI BỎ KHỎI BUFFER (Thay thế RemoveFromBuffer)
            // Nếu chunkList là Mảng (Array), ta gán null
            for (int i = 0; i < chunkList.Length; i++)
            {
                if (chunkList[i] == sceneToUnload)
                {
                    chunkList[i] = null;
                    break;
                }
            }

            Debug.Log($"[Unload] Da giai phong Scene: {sceneToUnload.Name}");
        }
        public static void UnloadSceneOld(IScenePrefab sceneToUnload)
        {
            var instance = SingleScene.Instance;
            if (sceneToUnload == null) return;

            // 1. NGẮT KẾT NỐI VÀ HÀN GẮN THẾ GIỚI
            // Nếu thằng bị xóa có thằng bên PHẢI -> Bảo thằng bên phải đóng tường trái
            if (sceneToUnload.Right != null)
            {
                var neighborRight = sceneToUnload.Right;
                neighborRight.Left = null; // Ngắt liên kết lùi

                // Đóng tường để Player không đi vào vùng trống
                neighborRight.LeftWall.isTrigger = false;

                // Bật lại TriggerZone để sau này Player quay lại có thể load lại vùng vừa xóa
                if (neighborRight.LeftTriggerZone != null)
                {
                    neighborRight.LeftTriggerZone.On();
                    var tz = neighborRight.LeftTriggerZone;

                }
            }

            // Nếu thằng bị xóa có thằng bên TRÁI -> Bảo thằng bên trái đóng tường phải
            if (sceneToUnload.Left != null)
            {
                var neighborLeft = sceneToUnload.Left;
                if (!neighborLeft.IsDestroying)
                {
                    neighborLeft.Right = null; // Ngắt liên kết tiến

                    neighborLeft.RightWall.isTrigger = false;

                    if (neighborLeft.RightTriggerZone != null)
                    {
                        neighborLeft.RightTriggerZone.On();

                    }
                }
            }

            // 2. DỌN DẸP HỆ THỐNG PHỤ TRỢ
            // Xóa khỏi Global Bound để Camera Confiner không bị "văng"
            if (instance?.GlobalWorldBound != null)
            {
                instance.GlobalWorldBound.RemoveBound(sceneToUnload);
            }

            // 3. GIẢI PHÓNG BỘ NHỚ
            if (sceneToUnload.GoWorld != null)
            {
                sceneToUnload.GoWorld.SafeDestroy();
            }

            // 4. LOẠI BỎ KHỎI BUFFER
            for (int i = 0; i < chunkList.Length; i++)
            {
                if (chunkList[i] == sceneToUnload)
                {
                    chunkList[i] = null;
                    break;
                }
            }

            Debug.Log($"[Unload] Da giai phong Scene: {sceneToUnload.Name}");
        }
        public static void AddSceneToBuffer(IScenePrefab newScene)
        {
            // 1. Đếm số lượng Scene thực tế đang có trong mảng
            int currentCount = 0;
            for (int i = 0; i < chunkList.Length; i++)
            {
                if (chunkList[i] != null) currentCount++;
            }

            // 2. Nếu đầy bộ đệm, tìm thằng xa nhất để Unload
            if (currentCount >= MAX_BUFFER_SIZE)
            {
                // Tìm Scene nằm ở rìa xa nhất so với newScene
                IScenePrefab furthestScene = FindFurthestScene(newScene);

                if (furthestScene != null)
                {
                    UnloadSceneAsync(furthestScene).Forget();

                    // Xóa tham chiếu trong mảng chunkList
                    for (int i = 0; i < chunkList.Length; i++)
                    {
                        if (chunkList[i] == furthestScene)
                        {
                            chunkList[i] = null;
                            break;
                        }
                    }
                }
            }

            // 3. Tìm một slot trống bất kỳ trong mảng để lưu newScene
            for (int i = 0; i < chunkList.Length; i++)
            {
                if (chunkList[i] == null)
                {
                    chunkList[i] = newScene;
                    break;
                }
            }
        }
        private static void AddSceneToBufferOld(IScenePrefab newScene)
        {
            if (newScene == null) return;

            // Kiểm tra xem Scene này đã có trong bộ đệm chưa để tránh trùng lặp
            for (int i = 0; i < chunkList.Length; i++)
            {
                if (chunkList[i] == newScene) return;
            }

            // 1. Tìm slot trống và đếm
            int firstEmptySlot = -1;
            int currentCount = 0;

            for (int i = 0; i < chunkList.Length; i++)
            {
                if (chunkList[i] != null)
                {
                    currentCount++;
                }
                else if (firstEmptySlot == -1)
                {
                    firstEmptySlot = i;
                }
            }

            // 2. Nếu đầy, giải phóng 1 slot
            if (currentCount >= MAX_BUFFER_SIZE)
            {
                IScenePrefab furthestScene = FindFurthestScene(newScene);
                if (furthestScene != null)
                {
                    for (int i = 0; i < chunkList.Length; i++)
                    {
                        if (chunkList[i] == furthestScene)
                        {
                            UnloadSceneAsync(furthestScene).Forget();
                            chunkList[i] = null;
                            firstEmptySlot = i; // Slot này vừa trống, dùng nó luôn
                            currentCount--; // Giảm đếm vì vừa xóa
                            break;
                        }
                    }
                }
            }

            // 3. Thêm Scene mới vào slot trống
            if (firstEmptySlot != -1)
            {
                chunkList[firstEmptySlot] = newScene;
                currentCount++; // Tăng đếm sau khi thêm thành công
            }

            Debug.Log($"Scene Added! Current Buffer Count: {currentCount}");
        }

        // Hàm tìm Scene xa nhất dựa trên tọa độ X (Chính xác tuyệt đối cho game 2D)
        private static IScenePrefab FindFurthestScene(IScenePrefab reference)
        {
            IScenePrefab target = null;
            float maxDist = -1f;
            float refX = reference.GoWorld.transform.position.x;

            foreach (var s in chunkList)
            {
                if (s == null || s == reference) continue;

                float dist = Mathf.Abs(s.GoWorld.transform.position.x - refX);
                if (dist > maxDist)
                {
                    maxDist = dist;
                    target = s;
                }
            }
            return target;
        }


        public static async UniTask LoadSceneAsyncDelete(ITriggerZone zone)
        {
            IScenePrefab ret = null;

            // Router điều hướng
            if (zone.Direction == TriggerZoneDirection.Right)
                ret = await LoadRightSceneAsyncDelete(zone);
            else if (zone.Direction == TriggerZoneDirection.Left)
                ret = await LoadLeftSceneAsyncDelete(zone);

            // Quản lý tập trung tại đây

        }
        private static async UniTask<IScenePrefab> LoadLeftSceneAsyncDelete(ITriggerZone wall)
        {
            CheckInteriorScene();
            var instance = SingleScene.Instance;
            var scene = (wall as MonoBehaviour).GetComponentInParent<IScenePrefab>();
            var es = (wall as MonoBehaviour).GetComponentInParent<ElasticScene>();

            if (scene == null) return null;

            var s = await Commons.LoadPrefabsAsync(wall.TriggerPath, TempObject);
            if (s == null) return null;
            var otherScene = s.GetComponent<IScenePrefab>();
            if (otherScene == null) return null;

            GameEvents.TriggerOnSceneLoad(otherScene, SceneState.LoadLeft, null);

            // do bo buc tuong ben trai
            if (otherScene.Actor != null)
            {
                UnityEngine.Object.Destroy((otherScene.Actor as MonoBehaviour).gameObject);


            }




            instance.VCam.UpdateDamping();
            //AddSceneToBuffer(otherScene);


            otherScene.RightTriggerZone.Off();
            otherScene.LeftTriggerZone.Off();
            scene.LeftTriggerZone.Off();

            if (otherScene.Globalight != null && otherScene.Globalight.GlobalLight != null && otherScene.Globalight.GlobalLight.Light != null)
                otherScene.Globalight.GlobalLight.Light.gameObject.SetActive(false);
            otherScene.GoWorld.transform.SetParent(null);


            SnapToLeft(scene, otherScene);
            LightManagerObject.Add(otherScene);
            otherScene.TrimEdge();
            //var track = scene.WorkTracker;
            //var righFacetInfo = scene.JoinInfo.WorldJoinInfo.WorldFacets.Right;
            var leftFacetInfo = otherScene.JoinInfo.WorldJoinInfo.WorldFacets.Left;
            otherScene.WorldBound.Coll.VerticalFacetAlign(leftFacetInfo.Start, leftFacetInfo.End, otherScene.JoinInfo.WorldJoinInfo.LeftPos.x - 5);

            //otherScene.WorldBound.Coll.SnapLeftEdgeToX(scene.JoinInfo.WorldJoinInfo.RightPos.x - 5);
            var coll = otherScene.WorldBound.Coll;
            instance.GlobalWorldBound.AddBound(otherScene);


            //if (track != null)
            //{
            //    track.Coll?.VerticalFacetAlign(righFacetInfo.Start, righFacetInfo.End, otherScene.LeftWall.bounds.max.x);
            //}

            otherScene.RightWall.isTrigger = true;
            scene.LeftWall.isTrigger = true;
            // tat phai ben trai
            otherScene.RightTriggerZone.Off();
            scene.LeftTriggerZone.Off();
            scene.Left = otherScene;
            otherScene.Right = scene;
            otherScene.LeftTriggerZone.On();
            AddSceneToBuffer(otherScene);
            
            GameEvents.TriggerOnSceneLoadComplete(otherScene, SceneState.LoadLeft, null);

            return otherScene;
        }
        private static void CheckInteriorScene()
        {

            var qr1 = InteriorGameObject.gameObject.GetComponentsInChildren<IScenePrefab>();
            if (qr1.Any(p => p == SingleScene.Instance.CurrentWorld))
            {
                for (var i = 0; i < chunkList.Length; i++)
                {
                    if (chunkList[i] == null)
                    {
                        chunkList[i] = SingleScene.Instance.CurrentWorld;
                        SingleScene.Instance.CurrentWorld.GoWorld.transform.SetParent(null, true);
                        break;
                    }
                }
            }
        }
        private static async UniTask<IScenePrefab> LoadRightSceneAsyncDelete(ITriggerZone wall)
        {
            CheckInteriorScene();
            var instance = SingleScene.Instance;
            var scene = (wall as MonoBehaviour).GetComponentInParent<IScenePrefab>();
            var es = (wall as MonoBehaviour).GetComponentInParent<ElasticScene>();

            if (scene == null) return null;

            var s = await Commons.LoadPrefabsAsync(wall.TriggerPath, TempObject);
            if (s == null) return null;
            var otherScene = s.GetComponent<IScenePrefab>();
            if (otherScene == null) return null;
            GameEvents.TriggerOnSceneLoad(otherScene, SceneState.LoadRight, null);

            // do bo buc tuong ben trai
            if (otherScene.Actor != null)
            {
                UnityEngine.Object.Destroy((otherScene.Actor as MonoBehaviour).gameObject);


            }




            instance.VCam.UpdateDamping();
            //AddSceneToBuffer(otherScene);

            otherScene.RightTriggerZone.Off();
            otherScene.LeftTriggerZone.Off();
            otherScene.GoWorld.SetActive(false);
            otherScene.WorldBound.Coll.gameObject.SetActive(false);
            if (otherScene.Globalight != null && otherScene.Globalight.GlobalLight != null && otherScene.Globalight.GlobalLight.Light != null)
                otherScene.Globalight.GlobalLight.Light.gameObject.SetActive(false);
            otherScene.GoWorld.transform.SetParent(null);

            SnapToRight(scene, otherScene);
            LightManagerObject.Add(otherScene);
            otherScene.TrimEdge();
            var track = scene.WorkTracker;
            var righFacetInfo = scene.JoinInfo.WorldJoinInfo.WorldFacets.Right;
            var leftFacetInfo = otherScene.JoinInfo.WorldJoinInfo.WorldFacets.Left;
            otherScene.WorldBound.Coll.VerticalFacetAlign(leftFacetInfo.Start, leftFacetInfo.End, scene.JoinInfo.WorldJoinInfo.RightPos.x - 5);
            //otherScene.WorldBound.Coll.SnapLeftEdgeToX(scene.JoinInfo.WorldJoinInfo.RightPos.x - 5);
            var coll = otherScene.WorldBound.Coll;
            instance.GlobalWorldBound.AddBound(otherScene);


            if (track != null)
            {
                track.Coll?.VerticalFacetAlign(righFacetInfo.Start, righFacetInfo.End, otherScene.LeftWall.bounds.max.x);
            }
            otherScene.GoWorld.SetActive(true);
            coll.gameObject.SetActive(true);
            otherScene.LeftWall.isTrigger = true;
            scene.RightWall.isTrigger = true;
            // tat phai ben trai
            otherScene.LeftTriggerZone.Off();
            scene.RightTriggerZone.Off();
            otherScene.RightTriggerZone.On();
            otherScene.GoWorld.SetActive(true);
            scene.Right = otherScene;
            otherScene.Left = scene;
            AddSceneToBuffer(otherScene);
           
            GameEvents.TriggerOnSceneLoadComplete(otherScene, SceneState.LoadRight, null);

            return otherScene;
        }



        private static void SnapToRight(IScenePrefab scene, IScenePrefab otherScene)
        {
            //var targetPoint = scene.RightPosJoin;

            //// Điểm Hiện Tại của Scene mới (B): Mép bên trai của sàn Scene mới
            //var currentPoint = otherScene.LeftPosJoin;

            //// 3. Tính toán Snap Offset (Đích - Tại)
            //// Di chuyển sao cho currentPoint đè khít lên targetPoint
            //Vector3 snapOffset = targetPoint.position - currentPoint.position;

            // Áp dụng dịch chuyển cho toàn bộ root của Scene mới
            var offset = scene.JoinInfo.WorldJoinInfo.RightPos - otherScene.JoinInfo.WorldJoinInfo.LeftPos;
            otherScene.GoWorld.transform.position += (Vector3)offset;
            otherScene.JoinInfo.WorldJoinInfo.Move(offset);


        }
        private static void SnapToLeft(IScenePrefab scene, IScenePrefab otherScene)
        {

            var offset = (scene.JoinInfo.WorldJoinInfo.LeftPos - otherScene.JoinInfo.WorldJoinInfo.RightPos);
            otherScene.GoWorld.transform.position += (Vector3)offset;

            otherScene.JoinInfo.WorldJoinInfo.Move(offset);
        }


        //public static async UniTask<IScenePrefab> LoadNewAsync(string path, string spawnName, IActorObject actor = null)
        //{
        //    LightManagerObject.Clean();

        //    GlobalApplication.WorldTrackerObject?.Cts?.Stop();
        //    //WorldMonitorManager.Instance.CurrentScene = null;
        //    GlobalWorldBound.Instance?.Clear();
        //    var instance = SingleScene.Instance;
        //    if (instance == null) return null;
        //    await instance.FadeScreen.FadeInAsync();
        //    CleanAll();
        //    if (instance.CurrentWorld != null)
        //    {
        //        UnityEngine.Object.Destroy(instance.CurrentWorld.GoWorld);
        //        instance.CurrentWorld = null;
        //    }


        //    var go = await Commons.LoadPrefabsAsync(path, TempObject);
        //    var s = go.GetComponent<IScenePrefab>();
        //    GameEvents.TriggerOnSceneLoad(s, SceneState.LoadNew, s.StartPos);
        //    var startSpawn = s.StartPos;
        //    if (!string.IsNullOrEmpty(spawnName))
        //    {
        //        startSpawn = s.FindSpawnTargetByName(spawnName);
        //    }

        //    ResetVCam(s);


        //    instance.VCam.SetOrthoSizeImmediate(s.OrthographicSize);
        //    if (s.Actor != null)
        //    {
        //        instance.VCam.Follow = s.Actor.CamWatcher.transform;
        //        instance.VCam.Target.TrackingTarget = s.Actor.CamWatcher.transform;
        //        instance.VCam.Target.LookAtTarget = s.Actor.CamWatcher.transform;
        //    }
        //    if (instance.CurrentActor != null && !(instance.CurrentActor as MonoBehaviour).IsDestroyed())
        //    {
        //        instance.CurrentActor.DoDestroy();
        //    }
        //    instance.CurrentActor = s.Actor;
        //    instance.Cam.GetComponent<ICam>().UpdateSizeByOrthoGraphicSize(s.OrthographicSize);

        //    instance.GlobalWorldBound.SetBound(s);
        //    AddSceneToBuffer(s);
        //    s.GoWorld.transform.SetParent(null);
        //    s.TrimEdge();

        //    if (s.Actor != null)
        //    {
        //        if (actor != null)
        //        {
        //            UnityEngine.Object.Destroy((s.Actor as MonoBehaviour).gameObject);
        //            startSpawn.MoveOtherToMe(actor as MonoBehaviour);

        //        }
        //        else
        //        {
        //            (s.Actor as MonoBehaviour).transform.SetParent(null, true);
        //            startSpawn.MoveOtherToMe(s.Actor as MonoBehaviour);
        //        }



        //    }
        //    LightManagerObject.instance.GlobalLight.color = s.Globalight.GlobalLight.Light.color;
        //    LightManagerObject.instance.GlobalLight.intensity = s.Globalight.GlobalLight.Light.intensity;
        //    LightManagerObject.Add(s);

        //    GameEvents.TriggerOnSceneLoadComplete(s, SceneState.LoadNew, startSpawn);
        //    await instance.FadeScreen.FadeOutAsync();

        //    return s;
        //}

        private static async UniTask ClearInteriorScenesAsync()
        {
            // 1. Chuyển đổi sang List để duyệt an toàn, tránh lỗi thay đổi collection khi đang xóa
            var children = InteriorGameObject.transform.Cast<Transform>().ToList();

            for (int i = 0; i < children.Count; i++)
            {
                Transform p = children[i];

                // Kiểm tra null đề phòng object đã bị xóa bởi logic khác
                if (p == null) continue;

                var s = p.GetComponent<IScenePrefab>();
                if (s != null)
                {
                    // Xóa khỏi hệ thống quản lý vùng biên
                    GlobalWorldBound.Instance.RemoveBound(s);

                    // Giải phóng tài nguyên (Mesh, Texture, Memory)
                    s.GoWorld.SafeDestroy();
                }

                // 2. Cứ mỗi 2 object xử lý xong thì nhường quyền điều khiển cho CPU 1 khung hình
                // Giúp duy trì mức 16.7ms (60 FPS) ổn định
                if (i % 2 == 0)
                {
                    await UniTask.Yield();
                }
            }

            Debug.Log("[Emotion] Interior Scenes cleared asynchronously.");
        }

        private static async UniTask ClearBackupInteriorScenesAsync()
        {
            // 1. Chuyển đổi sang List để tránh lỗi "Collection was modified" 
            // khi duyệt qua các con của Transform trong lúc đang hủy chúng.
            var targets = BackupInterior.transform.Cast<Transform>().ToList();

            for (int i = 0; i < targets.Count; i++)
            {
                Transform p = targets[i];

                // Kiểm tra an toàn đề phòng Object đã bị hủy từ trước
                if (p == null) continue;

                var scenePrefab = p.GetComponent<IScenePrefab>();
                if (scenePrefab != null)
                {
                    // Xóa khỏi hệ thống quản lý Bound
                    GlobalWorldBound.Instance.RemoveBound(scenePrefab);

                    // Giải phóng Resource (Mesh, Texture, Sprite...)
                    scenePrefab.GoWorld.SafeDestroy();
                }

                // 2. Chia nhỏ gánh nặng: Cứ sau mỗi 2 Object thì nghỉ 1 frame (Yield)
                // Với MSI Cyborg i7, bạn có thể tăng lên 3-5 nếu muốn xóa nhanh hơn.
                if (i % 2 == 0)
                {
                    await UniTask.Yield();
                }
            }

            // Đảm bảo BackupInterior hoàn toàn sạch sẽ sau vòng lặp
            SLog.Info("[Emotion] Backup Interior Scenes cleared asynchronously.");
        }
        private static async UniTask ClearInteriorScenesIfInChunksAsync()
        {
            // Gom danh sách Transform để tránh lỗi "Collection was modified" khi duyệt foreach
            var backupTransforms = BackupInterior.GetComponentsInChildren<Transform>(true)
                .Where(t => t != BackupInterior.transform).ToList();

            foreach (var p in backupTransforms)
            {
                // Kiểm tra null đề phòng object đã bị xóa trước đó
                if (p == null) continue;

                if (chunkList.All(x => x != null && x.Name == p.name))
                {
                    var scenePrefab = p.GetComponent<IScenePrefab>();
                    if (scenePrefab != null)
                    {
                        GlobalWorldBound.Instance.RemoveBound(scenePrefab);
                        scenePrefab.GoWorld.SafeDestroy();
                    }

                    // Chia nhỏ gánh nặng: Đợi 1 frame sau mỗi lần xử lý nếu cần
                    // Nếu số lượng object ít, có thể chạy 3-5 cái rồi mới Yield
                    await UniTask.Yield();
                }
            }

            var interiorTransforms = InteriorGameObject.GetComponentsInChildren<Transform>(true)
                .Where(t => t != InteriorGameObject.transform).ToList();

            foreach (var p in interiorTransforms)
            {
                if (p == null) continue;

                if (chunkList.All(x => x != null && x.Name == p.name))
                {
                    var scenePrefab = p.GetComponent<IScenePrefab>();
                    if (scenePrefab != null)
                    {
                        scenePrefab.GoWorld.SafeDestroy();
                    }

                    await UniTask.Yield();
                }
            }
        }
        public static async UniTask ClearAllChunksAsync()
        {
            // 1. Duyệt qua danh sách và xóa từng chunk
            for (int i = 0; i < chunkList.Length; i++)
            {
                if (chunkList[i] != null)
                {
                    // Giải phóng an toàn thông qua Interface của bạn
                    chunkList[i].GoWorld.SafeDestroy();
                    chunkList[i] = null;

                    // CỨ MỖI 2-3 OBJECT THÌ NÊN NGHỈ 1 FRAME
                    // Điều này giúp CPU có thời gian xử lý Rendering và Physics
                    if (i % 3 == 0)
                    {
                        await UniTask.Yield();
                    }
                }
            }

            // 2. Reset các thông số quản lý
            chunkList = new IScenePrefab[MAX_BUFFER_SIZE];
            currentIndex = 0;

            if (SingleScene.Instance != null)
            {
                SingleScene.Instance.CurrentWorld = null;
            }

            // Đợi thêm 1 frame cuối để đảm bảo Garbage Collector có khoảng trống làm việc
            await UniTask.Yield();

            SLog.Info("[Emotion] All chunks cleared asynchronously.");
        }
        public static void HideAllChunks()
        {
            for (int i = 0; i < chunkList.Length; i++)
            {
                if (chunkList[i] != null)
                {
                    chunkList[i].GoWorld.SetActive(false);
                    //if(chunkList[i].RefColliderWorldBound!= null)
                    //{

                    //    //chunkList[i].RefColliderWorldBound.gameObject.SetActive(false);
                    //}
                    chunkList[i].WorldBound.Coll.transform.SetParent((chunkList[i].WorldBound as MonoBehaviour).transform, true);
                }
            }
        }
        public static void ShowAllChunks()
        {
            for (int i = 0; i < chunkList.Length; i++)
            {
                if (chunkList[i] != null)
                {
                    chunkList[i].GoWorld.SetActive(true);
                    GlobalWorldBound.Instance.AddBound(chunkList[i]);
                }
            }
        }

        public static void ResetVCam(IScenePrefab obj)
        {
            var controller = SingleScene.Instance;
            var vcam = controller.VCam;

            vcam.SetOrthoSizeImmediate(obj.OrthographicSize);

            //SingleSceneController.Instance.globalWorldBound.SetCollider2d(obj);

            controller.Confiner.Damping = 0;
            controller.Confiner.OversizeWindow.Enabled = false;
            controller.Confiner.InvalidateBoundingShapeCache();

            var target = vcam.Follow;

            if (target != null)
            {
                vcam.ForceCameraPosition(
                    target.position,
                    vcam.transform.rotation
                );
            }
            controller.Confiner.enabled = false;
            controller.Confiner.enabled = true;
            vcam.PreviousStateIsValid = false;
        }
        public static string CurrentScenePath;
        private static GameObject interiorGameObject;
        private static GameObject backupInterior;
        private static Transform deleteTempObject;
        private static async UniTask DoCleanBeforeLoadInteriror()
        {
            await ClearInteriorScenesIfInChunksAsync();
            await ClearAllChunksAsync();
        }
        public static async UniTask<IScenePrefab> LoadInteriorAsync(string path, string spawnName, IActorObject actor = null)
        {
            var startTime= DateTime.Now;
            double elapseTime = 0;
            
            LightManagerObject.Clean();

            GlobalApplication.WorldTrackerObject?.Cts?.Stop();
            var instance = SingleScene.Instance;

            await instance.FadeScreen.FadeInAsync(0.01f);
            DoCleanBeforeLoadInteriror().Forget();
            GameObject interior = InteriorGameObject;
            GameObject backupInterior = BackupInterior;

            var scene = interior.GetComponentInChildren<IScenePrefab>();
            if (scene != null && !scene.IsDestroying)
            {
                if (!scene.WorldBound.Coll.IsDestroyed())
                {
                    scene.WorldBound.Coll.transform.SetParent((scene.WorldBound as MonoBehaviour).transform, true);
                    (scene as MonoBehaviour).transform.SetParent(backupInterior.transform);
                }
                


            }
            GameObject go = null;
            IScenePrefab s = null;

            var tr = BackupInterior.GetComponentInChildrenByName<Transform>(path);
            if (tr != null)
            {
                go = tr.gameObject;
                s = go.GetComponent<IScenePrefab>();

            }
            if (s != null)
            {
                var ret = chunkList.Where(p => p != null && !p.GoWorld.IsDestroyed()).FirstOrDefault(p => p.Name.Equals(path, StringComparison.OrdinalIgnoreCase));
                if (ret != null)
                {
                    await ClearAllChunksAsync();

                }
                else
                {
                    s.GoWorld.transform.SetParent(InteriorGameObject.transform, true);


                    instance.Cam.GetComponent<ICam>().UpdateSizeByOrthoGraphicSize(s.OrthographicSize);
                    if (!s.WorldBound.Coll.IsDestroyed()) s.WorldBound.Coll.enabled = true;
                    if (!string.IsNullOrEmpty(spawnName))
                    {
                        //if (instance.CurrentActor != null && actor == null)
                        //{
                        //    var spawn = s.FindSpawnTargetByName(spawnName);
                        //    if (spawn == null) spawn = s.StartPos;
                        //    spawn.MoveOtherToMe(instance.CurrentActor as MonoBehaviour);
                        //}
                        //if (actor != null)
                        //{
                        //    var spawn = s.FindSpawnTargetByName(spawnName);
                        //    if (spawn == null) spawn = s.StartPos;

                        //    spawn.MoveOtherToMe(actor as MonoBehaviour);
                        //}

                    }
                    ResetVCam(s);
                    GlobalWorldBound.Instance.AddBound(s);
                    await instance.FadeScreen.FadeOutAsync(0.01f);
                    elapseTime=(DateTime.Now-startTime).TotalMilliseconds;
                    //
                    GlobalApplication.RealtimeStatsInstance.SaveTrack("LoadInteriorAsync", elapseTime);
                    //SLog.Info($"LoadInteriorAsync.time={elapseTime}");
                    return ret;
                }
            }
            var check = chunkList.FirstOrDefault(p => p != null && p.Name.Equals(path, StringComparison.OrdinalIgnoreCase));
            if (check != null)
            {
                //ShowAllChunks();
                //check.GoWorld.transform.SetParent(null, true);
                //ResetVCam(check);

                //SceneController.instance.Camera.GetComponent<ICam>().UpdateSizeByOrthoGraphicSize(check.OrthographicSize);
                //check.RefColliderWorldBound.enabled = true;

                //if (!string.IsNullOrEmpty(spawnName))
                //{
                //    var spawn = check.FindSpawnTargetByName(spawnName);
                //    if(spawn==null) spawn = check.StartPos;
                //    spawn.MoveOtherToMe(SingleSceneController.Instance.CurrentActor as MonoBehaviour);
                //}
                //GlobalWorldBound.Instance.AddBound(check);
                //ResetVCam(check);
                //await FadeScreenController.FadeOutAsync();


                //return check;
                await ClearAllChunksAsync();
            }
            HideAllChunks();
            go = await Commons.LoadPrefabsAsync(path, TempObject);
            s = go.GetComponent<IScenePrefab>();

            GameEvents.TriggerOnSceneLoad(s, SceneState.LoadInterior, null);
            if (s.Actor != null)
            {
                UnityEngine.Object.Destroy((s.Actor as MonoBehaviour).gameObject);
            }

            var startSpawn = s.StartPos;
            if (!string.IsNullOrEmpty(spawnName))
            {
                var spawn = s.FindSpawnTargetByName(spawnName);
                if (spawn != null) startSpawn = spawn;
            }
            
            s.GoWorld.transform.SetParent(interior.transform);


            
            ResetVCam(s);
            s.TrimEdge();


            instance.Cam.GetComponent<ICam>().UpdateSizeByOrthoGraphicSize(s.OrthographicSize);
            instance.GlobalWorldBound.SetBound(s);
            instance.CurrentWorld = s;
            LightManagerObject.Add(s);
            GameEvents.TriggerOnSceneLoadComplete(s, SceneState.LoadInterior, startSpawn);
            await instance.FadeScreen.FadeOutAsync(0.01f);
            elapseTime = (DateTime.Now - startTime).TotalMilliseconds;
            GlobalApplication.RealtimeStatsInstance.SaveTrack("LoadInteriorAsync", elapseTime);

            return s;
        }

        

//        public static bool PlayActiveSceneFromEditor()
//        {
//#if UNITY_EDITOR
//            if (UnityEditor.EditorPrefs.HasKey("PendingWorldPath"))
//            {
//                // Lấy đường dẫn ra
//                string path = UnityEditor.EditorPrefs.GetString("PendingWorldPath");

//                // Xóa ngay lập tức để lần sau nhấn Play bình thường 
//                // nó không tự động load lại cái world cũ này
//                UnityEditor.EditorPrefs.DeleteKey("PendingWorldPath");

//                if (!string.IsNullOrEmpty(path))
//                {

//                    LoadNewAsync(path, null).Forget();
//                    return true;
//                }
//                //else
//                //{
//                //    this.LoaNewWorldAsync(this.StartPrefab, new Vector2(0, 0), null).Forget();
//                //}
//                //return true;
//            }

//            return false;
//#else
//        return false;
//#endif


//        }

        internal static async UniTask CleanAll()
        {
            
            await ClearBackupInteriorScenesAsync();
            await ClearInteriorScenesIfInChunksAsync();
            await ClearAllChunksAsync();

        }

        
    }
}