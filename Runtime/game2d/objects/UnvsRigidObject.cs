using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using unvs.ext;
using unvs.game2d.objects.components;
using unvs.game2d.scenes;
using unvs.game2d.transitions;
using unvs.shares;
using static Unity.VisualScripting.Member;

namespace unvs.game2d.objects
{
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(ShadowCaster2D))]
   
    public partial class UnvsRigidObject:UnvsBaseComponent
    {
        public BoxCollider2D coll;
        public virtual void Awake()
        {
            if (Application.isPlaying)
            {
                InitRuntime();
            }
        }
        [SerializeField]
        public Vector2 Velocity;
        private Vector2 lastPosition;
        public Action OnBeginTransition;
        public Action OnInTransition;
        public Action OnCompleteTransition;
        private bool _ToggleHeight;
        public float DutaionTime;

        public virtual UnvsRigidObject ExecTransition(float distance, TrasitionEnum transition,float duration)
        {
            var rb = GetComponent<Rigidbody2D>();
            Vector2 targetPosition = rb.position;
            
            // 1. Tính toán tọa độ đích dựa trên Enum
            switch (transition)
            {
                case TrasitionEnum.Up: targetPosition += Vector2.up * distance; break;
                case TrasitionEnum.Down: targetPosition += Vector2.down * distance; break;
                case TrasitionEnum.Left: targetPosition += Vector2.left * distance; break;
                case TrasitionEnum.Right: targetPosition += Vector2.right * distance; break;
            }
            OnBeginTransition?.Invoke();
            // 2. Thực hiện Tween
            // Sử dụng DOMove của Rigidbody thay vì Transform để Unity xử lý vật lý mượt hơn
            rb.DOMove(targetPosition, duration) // 1f là thời gian di chuyển, bạn có thể biến nó thành tham số
              .SetEase(Ease.InOutQuad)     // Khởi đầu và kết thúc mượt mà (rất hợp với game Horror)
              .SetUpdate(UpdateType.Normal) // QUAN TRỌNG: Buộc Tween chạy trong FixedUpdate để đồng bộ với vật lý
              .OnUpdate(() => {
                  Velocity = ((Vector2)transform.position - lastPosition) / Time.fixedDeltaTime;
                  lastPosition = transform.position;
                  OnInTransition?.Invoke();
              }).OnComplete(() =>
              {
                  OnCompleteTransition?.Invoke();
              });

            return this;
        }

        
        public virtual void InitRuntime()
        {
            coll = this.GetComponent<BoxCollider2D>();
        }

        public void ToggleHeight()
        {
            _ToggleHeight = !_ToggleHeight;
            float height = _ToggleHeight?-coll.bounds.size.y: coll.bounds.size.y;
            transform.parent.DOMoveY(height, DutaionTime)
                .SetRelative()
                .SetEase(Ease.OutQuad).SetUpdate(UpdateType.Fixed) // QUAN TRỌNG: Buộc Tween chạy trong FixedUpdate để đồng bộ với vật lý
              .OnUpdate(() => {
                  Velocity = ((Vector2)transform.position - lastPosition) / Time.fixedDeltaTime;
                  lastPosition = transform.position;
                  OnInTransition?.Invoke();
              }).OnComplete(() =>
              {
                  OnCompleteTransition?.Invoke();
              }); ;
        }
    }
#if UNITY_EDITOR
    public partial class UnvsRigidObject : UnvsBaseComponent
    {
        

        public virtual void OnValidate()
        {
            this.SetMeOnLayer(Constants.Layers.WORLD_GROUND);
        }
        public virtual void OnDrawGizmos()
        {
            var sp = GetComponent<SpriteRenderer>();
            if(sp != null)
            {
                if (sp.sprite == null)
                {
                    sp.ApplyDefaultBox();
                }
            }
            if (coll == null)
                coll = this.GetComponent<BoxCollider2D>();
           
            
            if (sp != null)
            {
                sp.EditorSyncSize(coll, transform);
               
            }
            else
            {
                sp = this.GetComponentInChildren<SpriteRenderer>();
                sp.EditorSyncSize(coll);
                
            }

        }
    }
#endif
}