using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using unvs.shares;

namespace unvs.ext
{
    public static class SpriteRendererExtension
    {
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
}