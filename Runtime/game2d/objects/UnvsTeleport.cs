using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.AddressableAssets;
using unvs.actions;
using unvs.ext;
using unvs.game2d.scenes;
using unvs.game2d.actors;
using unvs.shares;


#if UNITY_EDITOR
using unvs.game2d.objects.editor;
using unvs.editor.utils;
using UnityEditor;
#endif

namespace unvs.game2d.objects
{
    [RequireComponent(typeof(UnvsSpawnPoint))]
    [RequireComponent(typeof(SpriteRenderer))]
    public partial class UnvsTeleport : UnvsInteractObject
    {
        [Header("Teleport info",order =0)]
        public AssetReference Target;
        public string TargetPath;
        public Transform otherSpawnPoints;
        public UnvsScene TarggetScene;
        public bool IsNew;
        public string SpawnName;
        
        [Header("Feedback audio", order = 2)]
        [SerializeField]
        public AudioInfo OpenSound;
        [SerializeField] public AudioInfo CloseSound;
       
        
        

        public override void InitRuntime()
        {

        }

    }

#if UNITY_EDITOR
    public partial class UnvsTeleport : UnvsInteractObject
    {
        [Header("Spawn List", order = 1)]
        [SerializeField] public SpawnPointInfo[] SpawnList;
        public void OnValidate()
        {



            if (Target != null)
            {
                this.TargetPath = Target.EditorGetAddressPath();
            }
            if (GetComponent<SpriteRenderer>().sprite == null)
                GetComponent<SpriteRenderer>().sprite = Commons.LoadAsset<Sprite>("Packages/com.unvs.core/Runtime/Sprites/Square.png");
            if (Target != null)
            {
                this.TargetPath = Target.EditorGetAddressPath();
                if (SpawnList != null)
                {
                    var selected = SpawnList.FirstOrDefault(p => p.IsSelected);
                    if (selected.IsSelected)
                    {
                        this.SpawnName = selected.name;
                    }
                }

            }

        }
        [UnvsButton("Read all Spawn points")]
        public void EditorReadAllSpawnPoints()
        {
            if (Target != null)
            {
                this.TargetPath = Target.EditorGetAddressPath();
                if (Target.editorAsset != null)
                {

                    GameObject prefab = Target.editorAsset as GameObject;

                    if (prefab != null)
                    {
                        var tr = prefab.GetComponentInChildrenByName<Transform>("Spawn-Points");
                        if (tr != null)
                        {
                            SpawnList = tr.GetComponentsInChildren<UnvsSpawnPoint>().Select(p => new SpawnPointInfo
                            {

                                name = p.gameObject.name,
                                Target = p.gameObject
                            }).ToArray();
                        }
                        this.otherSpawnPoints = tr;



                    }
                }

            }

        }
        [UnvsButton("Apply data")]
        public void EditorApplyData()
        {
            if (Target != null)
            {
                this.TargetPath = Target.EditorGetAddressPath();
            }
        }
    }
#endif

}