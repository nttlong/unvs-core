using UnityEngine;
using unvs.ext;
using unvs.interfaces;
using unvs.shares;
namespace unvs.actors
{

    public class ActorAnimationEvents : MonoBehaviour
    {
        
        public void OnAnimationEvent(string clipName)
        {
            var actor=this.GetComponentInParent<IActorObject>();
            if (actor != null)
            {
                var speaker = actor.Speaker;
                var terrantLayerMask = LayerMask.GetMask(Constants.Layers.TERRANT,Constants.Layers.SURFACE);
                // Lấy điểm thấp nhất của Collider để bắn tia xuống
                var collider = (actor as MonoBehaviour).GetComponent<Collider2D>();
                Vector2 rayStart = new Vector2(collider.bounds.center.x, collider.bounds.min.y + 0.1f);

                float rayDistance = 0.5f; // Khoảng cách ngắn đủ để chạm đất

                // VẼ TIA ĐỂ DEBUG (Rất quan trọng để xem tia bay đi đâu)
                Debug.DrawRay(rayStart, Vector2.down * rayDistance, Color.red, 1.0f);

                // PHẢI dùng Physics2D cho Collider2D
                RaycastHit2D hit = Physics2D.Raycast(rayStart, Vector2.down, rayDistance, terrantLayerMask);
                var audibleObject = (actor as MonoBehaviour).GetComponent<IAudibleStep>();
                audibleObject?.StepSound.Play(audibleObject?.Volume);
                if (hit.collider != null)
                {

                    var terrant = hit.collider.GetComponent<ITerrant>();
                    if (terrant != null)
                    {
                        AudioClip sound = terrant.Sound;

                        // Tìm AudioSource trên Actor
                        AudioSource audioSource = (actor as MonoBehaviour).GetComponent<AudioSource>();

                        // Nếu không tìm thấy, code sẽ tự "kéo thả" bằng cách thêm mới luôn
                        if (audioSource == null)
                        {
                            audioSource = (actor as MonoBehaviour).gameObject.AddComponent<AudioSource>();
                            // Tùy chỉnh nhanh một vài thông số nếu muốn
                            audioSource.playOnAwake = false;
                            audioSource.spatialBlend = 1f; // 1 là âm thanh 3D, 0 là 2D
                        }

                        if (sound != null)
                        {
                            audioSource.PlayOneShot(sound);
                        }
                    }
                    else
                    {

                    }

                }
                else
                {
                    // Debug.Log("Không bắn trúng gì cả. Kiểm tra lại Layer của mặt đất!");
                }
            }
        }

        //public void Start()
        //{
        //    var actor = GetComponentInParent<IActor>(true);
        //    if (actor != null)
        //    {
        //        Actor = (actor as MonoBehaviour).transform;
        //    }
        //}
    }
}
