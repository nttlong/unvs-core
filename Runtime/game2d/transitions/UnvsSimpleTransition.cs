using UnityEngine;
using DG.Tweening;
using game2d.objects;
namespace unvs.game2d.transitions
{
    public enum TransitionEnum
    {
        Vertical,
        Horizontal
    }
    public class UnvsSimpleTransition : UnvsTransitionBase
    {
        public TransitionEnum Transition;
        public float DutaionTime = 5f;
        public override void Execute(MonoBehaviour source)
        {

            var coll = source.GetComponent<Collider2D>();
            if (coll == null) return; // Kiểm tra tránh lỗi nếu không có collider
            var trans= source.GetComponent<UnvsTransitionable>();
            if(trans !=null)
            {
                if (Transition == TransitionEnum.Vertical)
                {
                    if (!trans.IsOn)
                    {
                        float height = coll.bounds.size.y;
                        source.transform.parent.DOMoveY(-height, DutaionTime)
                            .SetRelative()
                            .SetEase(Ease.OutQuad);
                        
                    } else
                    {
                        float height = coll.bounds.size.y;
                        source.transform.parent.DOMoveY(height, DutaionTime)
                            .SetRelative()
                            .SetEase(Ease.OutQuad);
                    }
                    trans.IsOn = !trans.IsOn;
                }
                if (Transition == TransitionEnum.Horizontal)
                {
                    if (!trans.IsOn)
                    {
                        float width = coll.bounds.size.x;
                        source.transform.parent.DOMoveX(-width, DutaionTime)
                            .SetRelative()
                            .SetEase(Ease.OutQuad);

                    }
                    else
                    {
                        float width = coll.bounds.size.x;
                        source.transform.parent.DOMoveY(width, DutaionTime)
                            .SetRelative()
                            .SetEase(Ease.OutQuad);
                    }
                    trans.IsOn = !trans.IsOn;
                }
            }
            
            // Cách 1: Tính toán vị trí đích (Target Position)
            //float targetY = source.transform.parent.position.y - height;
            //source.transform.parent.DOMoveY(targetY, DutaionTime);
        }
    }
}
