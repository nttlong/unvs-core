using Cysharp.Threading.Tasks;

using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using unvs.shares;

namespace unvs.ext
{
    public static class SpriteRendererExtension
    {
        public static SpriteRenderer ApplyDefaultBox(this SpriteRenderer sp)
        {
            if (sp == null) return sp;
            if(sp.sprite==null)
            {
                sp.sprite = Commons.LoadAsset<Sprite>(Commons.DefaultResources.Box);
            }
            return sp;
        }
        internal static void ApplyTexture(this SpriteRenderer sprite, string path)
        {
            var t=Commons.LoadAsset<Texture2D>(path);
            sprite.ApplyTexture(t);
        }
        public static Sprite ApplyTexture(this SpriteRenderer sr, Texture2D texT)
        {
            Sprite newSprite = Sprite.Create(texT,
                                      new Rect(0, 0, texT.width, texT.height),
                                      new Vector2(0.5f, 0.5f),
                                      100.0f);

            // Gán vào Renderer
            sr.sprite = newSprite;
            return newSprite;
        }
        public static Sprite ApplyTextureIfEmptySprite(this SpriteRenderer sr, Texture2D texT)
        {
            if (sr != null && texT!=null)
            {
                if (sr.sprite == null)
                {
                    Sprite newSprite = Sprite.Create(texT,
                                      new Rect(0, 0, texT.width, texT.height),
                                      new Vector2(0.5f, 0.5f),
                                      100.0f);

                    // Gán vào Renderer
                    sr.sprite = newSprite;
                    return newSprite;
                }
                return sr.sprite;
            }
            return null;
        }
        public static void LoadImage(this SpriteRenderer targetRenderer, string address)
        {
            // Bắt đầu load nhưng không bắt CPU phải đứng đợi
            var handle = Addressables.LoadAssetAsync<Sprite>(address);

            // Đăng ký một "lời hẹn": Khi nào xong thì chạy hàm này
            handle.Completed += (op) => {
                if (op.Status == AsyncOperationStatus.Succeeded)
                {
                    targetRenderer.sprite = op.Result;
                }
                else
                {
                    Debug.LogError($"Không tìm thấy Sprite tại: {address}");
                    // Giải phóng handle ngay nếu lỗi
                    Addressables.Release(op);
                }
            };
        }
        public static async UniTask LoadImageAsync(this SpriteRenderer targetRenderer, string address)
        {
            // 1. Tạo handle
            AsyncOperationHandle<Sprite> handle = Addressables.LoadAssetAsync<Sprite>(address);

            // 2. Sử dụng UniTask để await trực tiếp handle
            // ToUniTask() giúp tích hợp sâu với vòng đời của Unity (PlayerLoop)
            await handle.ToUniTask();

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                // Kiểm tra xem targetRenderer có còn tồn tại không trước khi gán
                // (Đề phòng trường hợp object bị destroy trong lúc đang load)
                if (targetRenderer != null)
                {
                    targetRenderer.sprite = handle.Result;
                }
                else
                {
                    // Nếu Renderer đã bị hủy, phải giải phóng ngay asset vừa load
                    Addressables.Release(handle);
                }
            }
            else
            {
                Debug.LogError($"[Emotion] Load Sprite thất bại tại: {address}");
                // Chỉ Release khi thất bại để tránh rò rỉ handle lỗi
                Addressables.Release(handle);
            }
        }

        public static void FixCollider2DSize(this SpriteRenderer sp, BoxCollider2D coll)
        {
            float width = sp.sprite.rect.width / sp.sprite.pixelsPerUnit;
            float height = sp.sprite.rect.height / sp.sprite.pixelsPerUnit;
            coll.size = new Vector2(width, height);// sp.transform.localScale;
        }
        
        public static void FixCollider2DSize(this Sprite sp, BoxCollider2D coll)
        {
            float width = sp.rect.width / sp.pixelsPerUnit;
            float height = sp.rect.height / sp.pixelsPerUnit;
            coll.size = new Vector2(width, height);// sp.transform.localScale;
           
        }
    }
#if UNITY_EDITOR
    public static class SpriteRendererExtensionExitor
    {
        /// <summary>
        /// Use if SpriteRenderer is in same level with BoxCollider2D
        /// </summary>
        /// <param name="sr"></param>
        /// <param name="coll"></param>
        /// <returns></returns>
        public static SpriteRenderer EditorSyncSize(this SpriteRenderer sr,BoxCollider2D coll, Transform transform)
        {
            if (sr == null ||coll == null || transform == null)
            {
                var i = 0;
                foreach(object obj  in new object[] {sr,coll, transform })
                {
                    if (obj == null)
                    {
                        Debug.LogError($"param {i} is null");
                    }
                    i++;
                }
            }
                SpriteRendererExtension.FixCollider2DSize(sr, coll);
            float width = sr.sprite.rect.width / sr.sprite.pixelsPerUnit;
            float height = sr.sprite.rect.height / sr.sprite.pixelsPerUnit;
            coll.size = new Vector2(width, height);// sp.transform.localScale;
            coll.offset = Vector2.zero;
            // coll.offset =new ( sp.transform.position.x/2,sp.transform.position.y);//= coll.offset;
            coll.transform.rotation = transform.rotation;// = coll.transform.rotation;
            return sr;
        }
        /// <summary>
        /// Use if SpriteRenderer is in child level with BoxCollider2D
        /// </summary>
        /// <param name="sr"></param>
        /// <param name="coll"></param>
        /// <returns></returns>
        public static SpriteRenderer EditorSyncSize(this SpriteRenderer sr, BoxCollider2D coll)
        {
            if (sr == null || coll == null)
            {
                var i = 0;
                foreach (object obj in new object[] { sr, coll })
                {
                    if (obj == null)
                    {
                        Debug.LogError($"param {i} is null");
                    }
                    i++;
                }
            }
            if (sr != null && coll != null)
            {
                sr.transform.localScale = coll.size;
                sr.transform.localPosition = coll.offset;
                // coll.offset =new ( sp.transform.position.x/2,sp.transform.position.y);//= coll.offset;
                coll.transform.rotation = sr.transform.rotation;// = coll.transform.rotation;
            }
            return sr;
        }
    } 
#endif
}