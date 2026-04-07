using System;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using unvs.ext;
using unvs.interfaces;

namespace unvs.players{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent (typeof(CapsuleCollider2D))]
    public class PlayerBase:MonoBehaviour{

        public event Action<PlayerBase> OnRuntimeStart;
        public CancellationTokenSource Cts;
        private Collider2D coll;
        [SerializeField]
        public AnimExractor anims;
        [SerializeField]
        public Physical physical;
        [SerializeField]
        public Dialogue dialogue;
        public virtual void CancellationTokenSourceRefresh()
        {
            Cts = Cts.Refresh();
        }

        private void Reset()
        {
            var col=this.AddComponentIfNotExist<CapsuleCollider2D>();
            col.size = new Vector2(8, 20);
            col.offset= new Vector2(0, 10);
        }
        public virtual void Awake()
        {
            
            this.RuntimeInit();
            if (Application.isPlaying)
                InitAllBaseSubPlayers();
        }

        public virtual void InitAllBaseSubPlayers()
        {
            var fields=typeof(PlayerBase).GetFields().Where(p=>p.FieldType.IsAssignableFrom(typeof(BaseSubPlayer)));
            
            foreach (var field in fields)
            {
                var val=field.GetValue(this);
                if(val != null)
                {

                    var mt = field.FieldType.GetMethod("OnPlayerStart");
                    if (mt != null)
                    {
                        mt.Invoke(this, new object[] { val });
                    }
                }
            }
        }

        public virtual void Update()
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();

            var v = mousePos.ToWorld(); 
            this.physical.Direction(v);
            if (Mouse.current.leftButton.IsPressed())
            {
                this.physical.MoveTo(v);
            } else
            {
                this.anims.BaseMotion("idle");
            }
           
        }
        public virtual void RuntimeInit()
        {
            if (!Application.isPlaying) return;
            this.RunTimeSetPosition();
            this.anims.BaseMotion("idle");
            
           
        }

        public virtual void RunTimeSetPosition()
        {
            var _currentScene=this.GetComponentInParent<IScenePrefab>();
            coll=this.GetComponent<Collider2D>();
            if(_currentScene != null )
            {
                _currentScene.StartPos.MoveOtherToMe(this);
                
            }
        }
#if UNITY_EDITOR
        public event Action GismoxDraw;
        public void EditorExtractAllAnim()
        {
            this.anims.EditorExtractAllAnim(this);
        }

        public void EditorPhysicalCalculate()
        {
            this.physical.EditorPhysicalCalculate(this);
        }
        public void EditorGenerateDialogueUI()
        {
            this.dialogue.EditorGenerateDialogueUI(this);
        }
        private void OnDrawGizmos()
        {
            GismoxDraw?.Invoke();
        }
        private void OnValidate()
        {
            GismoxDraw?.Invoke();
        }

        


#endif
    }
}