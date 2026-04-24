#ifndef SDF_BURN_EDGE_ONLY_INCLUDED
#define SDF_BURN_EDGE_ONLY_INCLUDED

void GetBurnEdgeOnly_float(
    float sdfValue, 
    float progress, 
    float edgeWidth,
    out float OutAlpha)
{
    // Ngưỡng cháy (0 = Rect, 1 = Shape)
    float threshold = progress;
    
    // Độ mịn biên để tránh răng cưa
    float aa = fwidth(sdfValue) + 0.0001;

    // Khoảng cách từ vị trí hiện tại đến biên đang cháy
    float dist = abs(sdfValue - threshold);

    // CHÌA KHÓA: Chỉ lấy vùng nằm trong khoảng cách edgeWidth
    // Kết quả này trả về 1 tại biên và 0 ở mọi nơi khác
    OutAlpha = smoothstep(edgeWidth + aa, edgeWidth - aa, dist);
}

// Chỉ tính toán Alpha của viền lửa dựa trên SDF
void GetSDFEdgeMask_float(
    float sdfValue, 
    float progress, 
    float edgeWidth,
    out float OutAlpha)
{
    // 1. Ngưỡng hiện tại (0 = Ngoài cùng, 1 = Sát Shape)
    float threshold = progress;

    // 2. Độ mịn khử răng cưa (Antialiasing)
    // fwidth giúp viền luôn mượt dù bạn zoom xa hay gần
    float aa = fwidth(sdfValue) + 0.0001;

    // 3. Tính khoảng cách từ pixel tới "đường biên" đang cháy
    float dist = abs(sdfValue - threshold);

    // 4. Tạo mặt nạ viền (Edge Mask)
    // Nếu dist < edgeWidth thì OutAlpha tiến về 1
    // Nếu dist > edgeWidth thì OutAlpha tiến về 0
    OutAlpha = smoothstep(edgeWidth + aa, edgeWidth - aa, dist);
}
void GetFillAndEdgeMask_float(
    float sdfValue, 
    float progress, 
    float edgeWidth,
    out float OutAlpha,    // Mặt nạ vùng lấp đầy (Trắng)
    out float EdgeAlpha    // Mặt nạ chỉ riêng cái viền (Lửa)
)
{
    float threshold = progress;
    float aa = fwidth(sdfValue) + 0.00001;

    // 1. FILL MASK: Lấp đầy vùng chưa cháy
    // Nếu sdfValue > threshold thì bằng 1, ngược lại bằng 0
    // Smoothstep ở đây giúp biên vùng lấp đầy mượt mà không răng cưa
    OutAlpha = smoothstep(threshold - aa, threshold + aa, sdfValue);

    // 2. EDGE MASK: Chỉ lấy cái viền tại điểm tiếp giáp
    // Dùng abs để tạo dải xung quanh threshold như bạn đã làm
    float dist = abs(sdfValue - threshold);
    EdgeAlpha = smoothstep(edgeWidth + aa, edgeWidth - aa, dist);
}
#endif