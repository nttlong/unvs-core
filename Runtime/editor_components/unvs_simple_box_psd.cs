/*
This component just use in editor mode,It will use 
purpose: if any mono behavior use this component
a button with creat psd file which include
1. create an a psd file (which pixel format is 8bit rgba)  by image file that input from script in runtime
2. psd file will generatr by call python api server
*/
#if UNITY_EDITOR
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.WSA;
using unvs.game2d.objects.components;
using unvs.game2d.objects.editor;
using unvs.game2d.scenes;
namespace unvs.editor.components     {
    
    public class UnvsSimpleBoxPsd : UnvsBaseComponent
    {
        public string imagePath;
        public string psdPath;
        [SerializeField]
        public SceneInfoResut Folder;

        [UnvsButton("Create PSD File")]
        public async UniTask CreatePSDFile()
        {
            var spr=this.GetComponent<SpriteRenderer>();
            if(spr ==null)
            {
                unvs.core.editorlibs.Dialogs.Show("Please,add SpriteRenderer");
                return;
            }
            if (!await unvs.core.editorlibs.UnvsPythonCall.HealthCheck())
            {
                return;
            }
            var points = new Vector2[] { 
                new Vector2 (0,0),
                new Vector2 (spr.bounds.size.x,0),
                new Vector2(spr.bounds.size.x,spr.bounds.size.y),
                new Vector2(0,spr.bounds.size.y)
            };
            
            var data = new
            {
                psd_file = psdPath,
                points = points.Select(p=> new
                {
                    x=(int)p.x,
                    y=(int)p.y,
                }).ToArray(),
                width= spr.bounds.size.x,
                hideFlags= spr.bounds.size.y

            };
            
            var result = await unvs.core.editorlibs.UnvsPythonCall.Call("UnvsPsd", "create_single_psd", data);
           
        }
        private void OnDrawGizmos()
        {

            Folder = unvs.core.editorlibs.EditorTools.GetFolderOfGameObjectByScene(gameObject);
          
            psdPath = System.IO.Path.Join(Folder.FolderPath, $"{name}.psd");
        }
    }
}
#endif