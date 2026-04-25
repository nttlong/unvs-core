#if UNITY_EDITOR
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using unvs.game2d.objects.editor;
using unvs.game2d.scenes;

namespace unvs.editor.components 
{
    [Serializable]
    public class unvs_psd_export : unvs.types.UnvsEditableProperty
    {
        [SerializeField] public Transform[] items;
        public UnvsScene Owner;
        public AssetReference OwnerRef;
        public string folderPath;

        [UnvsButton("Create psd")]
        public async UniTask EditorCreatePsdFile()
        {
            this.folderPath = unvs.editor.utils.UnvsEditorUtils.GetAbsFolderPathOfGameObject(Owner.gameObject);
            var file_ath = System.IO.Path.Join(this.folderPath, $"{Owner.gameObject.name}.psd");
            var data = unvs.editor.utils.PsdFile.Createlayers(file_ath);
            foreach (var item in items)
            {
                data.AddBox(item);
            }
            await UniTask.Yield();
            await unvs.editor.utils.UnvsPythonCall.Call("UnvsPsd", "create_dumny_actor_psd", data);
        }
    }
} 
#endif