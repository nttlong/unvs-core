#if UNITY_EDITOR
using UnityEngine;
using System;

public static class Texture_ConvertTo_SDF_JFA
{
    private const int NO_SEED = -1;
    public static Texture2D ConvertToMultiShapeSDF(Texture2D inputTex)
    {
        int width = inputTex.width;
        int height = inputTex.height;

        float[] distToShape = GetDistanceMap(inputTex);

        Texture2D sdfTex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Color32[] outputPixels = new Color32[width * height];

        // 1. THU HẸP PHẠM VI (SPREAD)
        // Thay vì dùng tỉ lệ màn hình lớn, ta dùng một con số nhỏ cố định.
        // Ví dụ: 16 hoặc 32 pixel để vùng đen không bị loang ra quá xa.
        float maxSpread = 20.0f;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int idx = y * width + x;
                float s = distToShape[idx];

                // 2. CHUẨN HÓA KHOẢNG CÁCH
                float normalizedValue = Mathf.Clamp01(s / maxSpread);

                // 3. ĐIỀU CHỈNH ĐỘ CONG (CURVE)
                // Dùng hàm Pow với số mũ thấp (0.5 hoặc 0.7) sẽ khiến vùng đen 
                // biến mất nhanh hơn khi ra xa biên, giúp hạt trông nhỏ lại.
                normalizedValue = Mathf.Pow(normalizedValue, 0.6f);

                // 4. ĐẢO NGƯỢC (Tùy chọn)
                // Nếu bạn muốn hạt màu trắng trên nền đen: 1.0f - normalizedValue
                // Hiện tại theo ảnh của bạn là hạt đen nền trắng:
                byte byteVal = (byte)(normalizedValue * 255);

                outputPixels[idx] = new Color32(byteVal, byteVal, byteVal, 255);
            }
        }

        sdfTex.SetPixels32(outputPixels);
        sdfTex.Apply();
        return sdfTex;
    }
    public static Texture2D ConvertToMultiShapeSDF004(Texture2D inputTex)
    {
        int width = inputTex.width;
        int height = inputTex.height;

        float[] distToShape = GetDistanceMap(inputTex);

        Texture2D sdfTex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Color32[] outputPixels = new Color32[width * height];

        // 1. GIẢM PHẠM VI LOANG (SPREAD)
        // Thay vì 0.25f, ta dùng 0.1f hoặc một con số nhỏ hơn để thu hẹp vùng đen
        float maxSpread = Mathf.Min(width, height) * 0.1f;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int idx = y * width + x;
                float s = distToShape[idx];

                // 2. NỘI SUY TUYẾN TÍNH
                float normalizedValue = Mathf.Clamp01(s / maxSpread);

                // 3. LÀM NHỎ VÙNG ĐEN BẰNG HÀM MŨ (POWER)
                // Càng tăng số mũ (ví dụ 2.0 hoặc 3.0), vùng màu đen sẽ càng co lại sát biên vật thể
                normalizedValue = Mathf.Pow(normalizedValue, 0.5f);
                // Lưu ý: Nếu muốn vùng đen nhỏ lại, dùng Pow < 1 (ví dụ 0.5). 
                // Nếu muốn vùng đen rộng ra nhưng mờ dần, dùng Pow > 1.

                byte byteVal = (byte)(normalizedValue * 255);

                outputPixels[idx] = new Color32(byteVal, byteVal, byteVal, 255);
            }
        }

        sdfTex.SetPixels32(outputPixels);
        sdfTex.Apply();
        return sdfTex;
    }
    public static Texture2D ConvertToMultiShapeSDF0002(Texture2D inputTex)
    {
        int width = inputTex.width;
        int height = inputTex.height;

        // 1. Lấy Distance Map từ JFA
        float[] distToShape = GetDistanceMap(inputTex);

        Texture2D sdfTex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Color32[] outputPixels = new Color32[width * height];

        // TÍNH TOÁN ĐỘ RỘNG GRADIENT
        // Thay vì dùng biên ảnh, ta xác định dải loang rộng bao nhiêu pixel.
        // Thử dùng 1/4 cạnh nhỏ nhất của ảnh làm độ loang tối đa.
        float maxSpread = Mathf.Min(width, height) * 0.25f;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int idx = y * width + x;
                float s = distToShape[idx];

                // CÔNG THỨC MỚI: Standard SDF Gradient
                // s = 0 (tại biên vật thể) -> 0
                // s >= maxSpread -> 1
                float normalizedValue = Mathf.Clamp01(s / maxSpread);

                // Nếu bạn muốn dùng SmoothStep để làm dải loang "mềm" hơn nữa ở hai đầu:
                // float normalizedValue = Mathf.SmoothStep(0, 1, s / maxSpread);

                // Chuyển về byte (0 = Đen, 255 = Trắng)
                byte byteVal = (byte)(normalizedValue * 255);

                outputPixels[idx] = new Color32(byteVal, byteVal, byteVal, 255);
            }
        }

        sdfTex.SetPixels32(outputPixels);
        sdfTex.Apply();
        return sdfTex;
    }
    public static Texture2D ConvertToMultiShapeSDFOld(Texture2D inputTex)
    {
        int width = inputTex.width;
        int height = inputTex.height;

        // BƯỚC 1: Lấy Distance Map (Sửa lỗi logic Seed)
        float[] distToShape = GetDistanceMap(inputTex);

        Texture2D sdfTex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Color32[] outputPixels = new Color32[width * height];

        //for (int y = 0; y < height; y++)
        //{
        //    for (int x = 0; x < width; x++)
        //    {
        //        int idx = y * width + x;

        //        // BƯỚC 2: Tính khoảng cách tới biên Rect
        //        float dLeft = x;
        //        float dRight = width - 1 - x;
        //        float dTop = y;
        //        float dBottom = height - 1 - y;
        //        float distToRect = Mathf.Min(Mathf.Min(dLeft, dRight), Mathf.Min(dTop, dBottom));

        //        // BƯỚC 3: Công thức nội suy của bạn
        //        float s = distToShape[idx];
        //        float r = distToRect;

        //        // Tránh chia cho 0 và tính toán giá trị
        //        float normalizedValue = s / (s + r + 0.00001f);
        //        byte byteVal = (byte)(Mathf.Clamp01(normalizedValue) * 255);

        //        // Gán màu (R, G, B giống nhau, Alpha 255)
        //        outputPixels[idx] = new Color32(byteVal, byteVal, byteVal, 255);
        //    }
        //}
        // Thiết lập một khoảng cách tối đa để gradient trông mượt hơn
        // Bạn có thể cho phép người dùng nhập giá trị này hoặc lấy theo tỉ lệ ảnh
        float maxDistance = Mathf.Min(width, height) * 0.5f;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int idx = y * width + x;
                float s = distToShape[idx];

                // Cách 1: Gradient dựa trên khoảng cách tuyệt đối (Standard SDF)
                // Càng xa vật thể càng trắng, tối đa tại maxDistance
                float normalizedValue = Mathf.Clamp01(s / maxDistance);

                // Cách 2: Kết hợp với distToRect nhưng dùng SmoothStep để làm mềm biên
                // float normalizedValue = Mathf.SmoothStep(0, 1, s / (s + distToRect + 0.001f));

                byte byteVal = (byte)(normalizedValue * 255);
                outputPixels[idx] = new Color32(byteVal, byteVal, byteVal, 255);
            }
        }
        sdfTex.SetPixels32(outputPixels);
        sdfTex.Apply();
        return sdfTex;
    }

    private static float[] GetDistanceMap(Texture2D inputTex)
    {
        int width = inputTex.width;
        int height = inputTex.height;
        int count = width * height;

        // Đọc pixel bằng Color32 (Nhanh và chính xác nhất trong Unity)
        Color32[] inputPixels = inputTex.GetPixels32();
        int[] closest = new int[count];

        // KHỞI TẠO HẠT GIỐNG: Sửa lỗi nhận diện Alpha
        for (int i = 0; i < count; i++)
        {
            int x = i % width;
            int y = i / width;

            // Trong Unity, alpha > 128 là điểm nằm trong Shape
            // Nếu ảnh bị đen, hãy thử đổi điều kiện này thành (inputPixels[i].r > 128) 
            // nếu bạn dùng ảnh trắng đen làm mask.
            closest[i] = (inputPixels[i].a > 128) ? (x << 16) | y : NO_SEED;
        }

        // CHẠY JFA (Loang khoảng cách)
        int maxJump = Mathf.Max(width, height);
        for (int jump = Mathf.NextPowerOfTwo(maxJump) / 2; jump >= 1; jump /= 2)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int idx = y * width + x;
                    int curSeed = closest[idx];
                    long bestDistSq = (curSeed == NO_SEED) ? long.MaxValue : GetDistSq(x, y, curSeed);

                    for (int dy = -1; dy <= 1; dy++)
                    {
                        for (int dx = -1; dx <= 1; dx++)
                        {
                            if (dx == 0 && dy == 0) continue;

                            int nx = x + dx * jump;
                            int ny = y + dy * jump;

                            if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                            {
                                int neighborSeed = closest[ny * width + nx];
                                if (neighborSeed != NO_SEED)
                                {
                                    long dsq = GetDistSq(x, y, neighborSeed);
                                    if (dsq < bestDistSq)
                                    {
                                        bestDistSq = dsq;
                                        closest[idx] = neighborSeed;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        // CHUYỂN SANG MẢNG FLOAT
        float[] distMap = new float[count];
        for (int i = 0; i < count; i++)
        {
            int x = i % width;
            int y = i / width;
            int seed = closest[i];
            distMap[i] = (seed == NO_SEED) ? 1000000f : Mathf.Sqrt(GetDistSq(x, y, seed));
        }
        return distMap;
    }

    private static long GetDistSq(int x, int y, int packedSeed)
    {
        int sx = packedSeed >> 16;
        int sy = packedSeed & 0xFFFF;
        long dx = x - sx;
        long dy = y - sy;
        return dx * dx + dy * dy;
    }
}
#endif