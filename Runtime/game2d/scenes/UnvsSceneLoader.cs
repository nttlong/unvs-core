using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using game2d.scenes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using unvs.actor.player;
using unvs.components;
using unvs.ext;
using unvs.game2d.actors;
using unvs.game2d.objects;
using unvs.game2d.objects.editor;
using unvs.shares;
using unvs.ui;

namespace unvs.game2d.scenes
{
    public partial class UnvsSceneLoader : UnvsUIComponentInstance<UnvsSceneLoader>
    {
        public Transform chunks;
        public Transform buffer;
        public Transform bufferDelete;
        public Transform interior;
        public Transform backupInterior;
        public Transform actorContainer;
        private UnvsScene lastInteriorScene;
        private static bool _isShow=true;
        private List<UnvsTeleport> _litsOfTempSpawns;
        private Transform _chunksBackupForTempLoadScene;
        private Transform _tmpLoadScene;
        private UniTask<UnvsScene> _leftLoadingTask ;
        private UniTask<UnvsScene> _rightLoadingTask ;

        public override bool DisablePlayerInput => false;

        public override bool EnablePlayerInput => false;

        public static void GameShow()
        {
            if (Instance == null) return;
            if(_isShow) return;
            Instance.gameObject.SetActive(true);
            _isShow = true;
        }
        public static void GameHide()
        {
            if (Instance == null) return;
            Instance.gameObject.SetActive(false);
            _isShow = false;
        }
        public override void InitEvents()
        {
            this.buffer.gameObject.SetActive(false);
        }
        public async UniTask ReturnFromTempScene(UnvsTeleport tmpTeleportObject, string spawnName)
        {
            this._tmpLoadScene.SafeDestroyChildrenAsync().Forget();
            await UniTask.Yield();
            this.chunks.gameObject.SetActive(true);
            if(UnvsApp.Instance.currentActor!=null)
            {
                UnvsApp.Instance.currentActor.StandBy(tmpTeleportObject.GetPosition());

            } 
        }
        private async UniTask EnsureAllChunksLoadedAsync()
        {
            
            
        
            if (_leftLoadingTask.Status == UniTaskStatus.Pending)
            {
                await _leftLoadingTask;
            }

          
            if (_rightLoadingTask.Status == UniTaskStatus.Pending)
            {
                await _rightLoadingTask;
            }

           
            
            

        }
        public async UniTask<UnvsScene> LoadTempSceneAsync(AssetReference sceneRef, UnvsTeleport tmpTeleportObject, string spawnName)
        {
            if (UnvsCinema.Instance != null)
            {
                if (UnvsCinema.Instance.CamChangeFollowOffsetTask.Status == UniTaskStatus.Pending)
                {
                    await UnvsCinema.Instance.CamChangeFollowOffsetTask;
                    UnvsCinema.Instance.UpdateLoadChunkSceneTrackerSizeByCurrentFollowOffset();
                }
            }
            await UnvsFadeScreen.Instance.FadeInAsync(UnvsApp.Instance.Settings.DefaultFadeTimeLoadScene);
            if (this._litsOfTempSpawns == null)
            {
                this._litsOfTempSpawns = new List<UnvsTeleport>();
            }
            this._litsOfTempSpawns.Add(tmpTeleportObject);
            if (this._chunksBackupForTempLoadScene == null)
            {
                this._chunksBackupForTempLoadScene = this.AddChildComponentIfNotExist<Transform>("_chunksBackupForTempLoadScene");
                this._chunksBackupForTempLoadScene.gameObject.SetActive(false);
            }
            if (this._tmpLoadScene == null)
            {
                this._tmpLoadScene = this.AddChildComponentIfNotExist<Transform>("_tmpLoadScene");
            }
            //foreach(var scene in this.chunks.GetComponentsInChildren<UnvsScene>())
            //{
            //    scene.transform.SetParent(this._chunksBackupForTempLoadScene, true);
            //}
            this.interior.gameObject.SetActive(false);
            this.chunks.gameObject.SetActive(false);
           
            
            

         
            var ret = await Commons.LoadPrefabsAsync<UnvsScene>(sceneRef, this.buffer);
            ret.transform.SetParent(this._tmpLoadScene, true);
            UnvsActor actor = ret.GetActiveActor();
            if (actor != null)
            {
                this.validateCurrentActor(ret);
            }

            ret.SetTempSpawnInfo(spawnName, tmpTeleportObject);
            UnvsApp.Instance.currentActor.StandBy(ret.GetStartPosition(spawnName));

            UnvsCinema.Instance.vcam.Watch(UnvsApp.Instance.currentActor.camWatcher);
            UnvsCinema.Instance.UpdateLoadChunkSceneTrackerSize(ret.followOffset);


            UnvsCinema.Instance.UpdateWorld(ret, true, UpdateWorldEmun.Interior);





           
            ret.TurnOnLeft().TurnOnRight();
            ret.gameObject.SetActive(true);



            lastInteriorScene = ret;
            await UnvsFadeScreen.Instance.FadeOutAsync(UnvsApp.Instance.Settings.DefaultFadeTimeLoadScene);
            UnvsApp.Instance.RaiseEnterScene(ret);
            return ret;
        }
        public async UniTask<UnvsScene> LoadInteriorAsync(AssetReference sceneRef, string spawnName, UnvsScene fromScene)
        {
            UnvsCinema.Instance.requestInvalidateBoundingShapeCache = true;
            if (UnvsCinema.Instance != null)
            {
                if (UnvsCinema.Instance.CamChangeFollowOffsetTask.Status == UniTaskStatus.Pending)
                {
                    await UnvsCinema.Instance.CamChangeFollowOffsetTask;
                    UnvsCinema.Instance.UpdateLoadChunkSceneTrackerSizeByCurrentFollowOffset();
                }
            }
            await UnvsFadeScreen.Instance.FadeInAsync(UnvsApp.Instance.Settings.DefaultFadeTimeLoadScene);
            if (fromScene == null) return null;
            lastInteriorScene = fromScene;
            lastInteriorScene.transform.SetParent(this.backupInterior.transform, true);
            lastInteriorScene.TurnOffLeft().TurnOffRight();
            if (lastInteriorScene.leftScene != null)
            {
                lastInteriorScene.leftScene.rightScene = null;
            }
            if (lastInteriorScene.rightScene != null)
            {
                lastInteriorScene.rightScene.leftScene = null;
            }
            lastInteriorScene.leftScene = null;
            lastInteriorScene.rightScene = null;
            UnvsCinema.Instance.ClearWorlds();
            UnvsApp.Instance.CleanUp();
            this.interior.gameObject.SetActive(true);
            this.chunks.gameObject.SetActive(false);
            this.clearAllChunksForLoadInterior();


            UnvsCinema.Instance.ClearWorlds();
            UnvsScene ret;
            ret = this.backupInterior.GetComponentInChildrenByName<UnvsScene>(sceneRef.GetObjectName());
            if (ret == null)
            {
                ret = await Commons.LoadPrefabsAsync<UnvsScene>(sceneRef, this.buffer);
                
                if (ret.followOffset == Vector3.zero)
                {
                    if (UnvsCinema.Instance != null)
                    {
                        ret.followOffset = fromScene.followOffset;
                        
                       
                    }
                }
                
            }
            ret.transform.SetParent(this.interior.transform, true);
            
            UnvsActor actor = ret.GetActiveActor();
            if (actor != null)
            {
                this.validateCurrentActor(ret);
            }
           

            UnvsApp.Instance.currentActor.StandBy(ret.GetStartPosition(spawnName));

            UnvsCinema.Instance.vcam.Watch(UnvsApp.Instance.currentActor.camWatcher);

            UnvsCinema.Instance.UpdateLoadChunkSceneTrackerSize(ret.followOffset);
           
            ret.TurnOnLeft().TurnOnRight();
            ret.gameObject.SetActive(true);
           
            await EnsureAllChunksLoadedAsync();

            lastInteriorScene = ret;
            //UnvsCinema.Instance.confiner.BoundingShape2D = null;
            UnvsCinema.Instance.vcam.PreviousStateIsValid = false;
            UnvsCinema.Instance.UpdateWorld(ret, true, UpdateWorldEmun.Interior);
            UnvsCinema.Instance.compositeCollider2D.GenerateGeometry();
            UnvsCinema.Instance.confiner.InvalidateBoundingShapeCache();
            await UniTask.DelayFrame(UnvsApp.Instance.Settings.DelayFrameBeforeInteriorSceneShow);
            await UnvsFadeScreen.Instance.FadeOutAsync(UnvsApp.Instance.Settings.DefaultFadeTimeLoadScene);
            UnvsApp.Instance.RaiseEnterScene(ret);
            UnvsCinema.Instance.requestInvalidateBoundingShapeCache = false;
            return ret;
        }

        public async UniTask<UnvsScene> LoadNewAsync(AssetReference sceneRef, string spawnName, bool byPlayerFail=false)
        {
            if (UnvsCinema.Instance != null)
            {
                if (UnvsCinema.Instance.CamChangeFollowOffsetTask.Status == UniTaskStatus.Pending)
                {
                    //await UnvsCinema.Instance.CamChangeFollowOffsetTask.SuppressCancellationThrow();
                    await UnvsCinema.Instance.CamChangeFollowOffsetTask;
                    UnvsCinema.Instance.UpdateLoadChunkSceneTrackerSizeByCurrentFollowOffset();
                   
                }
            }
            await UnvsFadeScreen.Instance.FadeInAsync(UnvsApp.Instance.Settings.DefaultFadeTimeLoadScene);
            this.clearAllChunks();
            this.chunks.gameObject.SetActive(true);
            this.interior.gameObject.SetActive(false);
            this.interior.gameObject.SetActive(false);
            if (UnvsApp.Instance.currentActor != null && !byPlayerFail)
            {
                UnvsApp.Instance.currentActor.gameObject.SafeDestroy();
            }
            UnvsCinema.Instance.ClearWorlds();
            var ret = await Commons.LoadPrefabsAsync<UnvsScene>(sceneRef, this.buffer);
            if (ret.followOffset == Vector3.zero)
            {
                if (UnvsCinema.Instance != null)
                {
                    ret.followOffset = UnvsApp.Instance.Settings.DefaultTargetOffset;
                }
            }
            UnvsCinema.Instance.UpdateLoadChunkSceneTrackerSize(ret.followOffset);
            ret.transform.SetParent(this.chunks.transform, true);

           
            UnvsActor actor = ret.GetActiveActor();

            if (actor != null)
            {
                if (!byPlayerFail)
                    actor.transform.SetParent(this.actorContainer.transform, true);
                else
                {
                    actor.gameObject.SafeDestroy();
                    actor = UnvsApp.Instance.currentActor;
                }
            } else
            {
                actor = UnvsApp.Instance.currentActor;
            }



          


            
            
          
            
            if (actor != null)
            {
                actor.StandBy(ret.GetStartPosition(spawnName));

                UnvsCinema.Instance.vcam.Watch(actor.camWatcher);
                UnvsApp.Instance.currentActor = actor;
            } else
            {
                UnvsCinema.Instance.vcam.Watch(ret.defaulCamWatcher);
            }
            UnvsCinema.Instance.UpdateWorld(ret, true, UpdateWorldEmun.New);
            UnvsCinema.Instance.vcam.UpdateByUnvsScene(ret);
            ret.gameObject.SetActive(true);
            await UnvsFadeScreen.Instance.FadeOutAsync(UnvsApp.Instance.Settings.DefaultFadeTimeLoadScene);
            UnvsApp.Instance.RaiseEnterScene(ret, true);
            return ret;
        }
        public async UniTask<UnvsScene> LoadChunkLeftAsync(UnvsScene fromScene, AssetReference sceneRef)
        {
           
            var utcs = new UniTaskCompletionSource<UnvsScene>();
            _leftLoadingTask = utcs.Task;
            try
            {
                this.clearChunkRightIfExeedeAsync().Forget();
                if (fromScene.transform.parent == this.interior)
                {
                    fromScene.transform.SetParent(this.chunks.transform, true);
                    interior.gameObject.SetActive(false);
                    backupInterior.SafeDestroyChildrenAsync().Forget();
                    this.chunks.gameObject.SetActive(true);
                }
                if (fromScene.transform.parent == this.interior)
                {
                    fromScene.transform.SetParent(this.chunks.transform.parent, true);
                }
                fromScene.TurnOffLeft();


                var ret = await Commons.LoadPrefabsAsync<UnvsScene>(sceneRef, this.buffer);
                ret.TurnOffRight();

                this.validateCurrentActor(ret);


                var offset = ret.JoinInfo.RightPos - fromScene.JoinInfo.LeftPos;


                ret.transform.SetParent(this.chunks.transform, false);
                ret.transform.position -= (Vector3)offset;
                ret.JoinInfo.LeftPos -= offset;
                ret.JoinInfo.RightPos -= offset;
                ret.transform.SetAsFirstSibling();
                UnvsCinema.Instance.UpdateWorld(ret, false, UpdateWorldEmun.Left);

                ret.gameObject.SetActive(true);
                fromScene.leftScene = ret;
                ret.rightScene = fromScene;
                //CenterScene();
                utcs.TrySetResult(ret);
                return ret;
            }
            catch (System.Exception e)
            {
                
                utcs.TrySetException(e);
                throw;
            }
        }

        

        public void ClearUpAll()
        {
            UnvsCinema.Instance.ClearWorlds();
            if (UnvsApp.Instance.currentActor != null)
            {
                UnvsApp.Instance.currentActor.gameObject.SafeDestroyAsync().Forget();
            }
            this.actorContainer.SafeDestroyChildrenAsync().Forget();
            this.chunks.SafeDestroyChildrenAsync().Forget();
            this.interior.SafeDestroyChildrenAsync().Forget();
            this.buffer.SafeDestroyChildrenAsync().Forget();
            this.backupInterior.SafeDestroyChildrenAsync().Forget();

        }
        public async UniTask<UnvsScene> LoadChunkRightAsync(UnvsScene fromScene, AssetReference sceneRef)
        {
            var utcs = new UniTaskCompletionSource<UnvsScene>();
            _rightLoadingTask = utcs.Task;
            try
            {
                this.clearChunkLeftIfExeedeAsync().Forget();
                if (fromScene.transform.parent == this.interior)
                {
                    fromScene.transform.SetParent(this.chunks.transform, true);
                    interior.gameObject.SetActive(false);
                    backupInterior.SafeDestroyChildrenAsync().Forget();
                    this.chunks.gameObject.SetActive(true);
                }
                fromScene.TurnOffRight();

                var ret = await Commons.LoadPrefabsAsync<UnvsScene>(sceneRef, this.buffer);

                ret.TurnOffLeft();
                this.validateCurrentActor(ret);


                var offset = ret.JoinInfo.LeftPos - fromScene.JoinInfo.RightPos;

                ret.transform.SetParent(this.chunks.transform, false);
                ret.transform.position -= (Vector3)offset;
                ret.JoinInfo.LeftPos -= offset;
                ret.JoinInfo.RightPos -= offset;
                //  ret.transform.SetAsLastSibling();
                UnvsCinema.Instance.UpdateWorld(ret, false, UpdateWorldEmun.Right);
                ret.gameObject.SetActive(true);
                fromScene.rightScene = ret;
                ret.leftScene = fromScene;
                utcs.TrySetResult(ret);
                return ret;
            }
            catch (System.Exception e)
            {
                
                utcs.TrySetException(e);
                throw;
            }
        }
        private void validateCurrentActor(UnvsScene scene)
        {
            var actor = scene.GetActiveActor();

            if (actor == null || UnvsApp.Instance.currentActor == null) return;

            // Safe disable player logic if it exists
            var player = actor.GetComponent<UnvsPlayer>();
            if (player != null) player.enabled = false;

            if (actor.GetType() == UnvsApp.Instance.currentActor.GetType())
            {
                if (actor.name == UnvsApp.Instance.currentActor.name)
                {
                    (actor as MonoBehaviour).enabled = false;
                    (actor as MonoBehaviour).gameObject.SetActive(false);
                    (actor as MonoBehaviour).gameObject.SafeDestroyAsync().Forget();
                    return;
                }
            }
        }
        private async UniTask clearChunkLeftIfExeedeAsync()
        {
            var len = this.chunks.GetComponentsInChildren<UnvsScene>().Length;
            if (len >= UnvsApp.Instance.Settings.ChunLenght)
            {
                var deleteScene = this.chunks.GetComponentsInChildren<UnvsScene>()[0];
                if (deleteScene.rightScene != null)
                {
                    deleteScene.rightScene.TurnOnLeft();
                }
                deleteScene.transform.SetParent(this.bufferDelete.transform);
                await this.bufferDelete.SafeDestroyChildrenAsync();
            }
        }
        private async UniTask clearChunkRightIfExeedeAsync()
        {
            var len = this.chunks.GetComponentsInChildren<UnvsScene>().Length;
            if (len >= UnvsApp.Instance.Settings.ChunLenght)
            {
                var deleteScene = this.chunks.GetComponentsInChildren<UnvsScene>()[UnvsApp.Instance.Settings.ChunLenght - 1];
                if (deleteScene.leftScene != null)
                {
                    deleteScene.leftScene.TurnOnRight();
                }
                deleteScene.transform.SetParent(this.bufferDelete.transform);

                await this.bufferDelete.SafeDestroyChildrenAsync();
            }
        }
        private void clearAllChunksForLoadInterior()
        {
            
            while (this.chunks.GetComponentsInChildren<UnvsScene>().Length > 0)
            {
                this.chunks.GetComponentInChildren<UnvsScene>().transform.SetParent(this.bufferDelete.transform);
            }

            this.bufferDelete.SafeDestroyChildrenAsync().Forget();
        }
        private void clearAllChunks()
        {
            if (lastInteriorScene != null)
            {
                lastInteriorScene.transform.SetParent(this.bufferDelete.transform);
            }
            while( this.chunks.GetComponentsInChildren<UnvsScene>().Length>0)
            {
                this.chunks.GetComponentInChildren<UnvsScene>().transform.SetParent(this.bufferDelete.transform);
            }
            
            this.bufferDelete.SafeDestroyChildrenAsync().Forget();
        }


    }
#if UNITY_EDITOR
    public partial class UnvsSceneLoader : UnvsUIComponentInstance<UnvsSceneLoader>
    {

        [UnvsButton("Generate")]
        public void Generate()
        {
            this.chunks = this.AddChildComponentIfNotExist<Transform>("chunks");
            this.buffer = this.AddChildComponentIfNotExist<Transform>("buffer");
            this.bufferDelete = this.AddChildComponentIfNotExist<Transform>("buffer-delete");
            this.bufferDelete.gameObject.SetActive(false);
            this.interior = this.AddChildComponentIfNotExist<Transform>("interior");
            this.buffer.gameObject.SetActive(false);
            this.actorContainer = this.AddChildComponentIfNotExist<Transform>("Actor-Container");
            this.backupInterior = this.AddChildComponentIfNotExist<Transform>("backup-Interior");
            this.backupInterior.gameObject.SetActive(false);

        }


    }





#endif
}
