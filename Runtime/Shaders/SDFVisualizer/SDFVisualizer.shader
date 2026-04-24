Shader "Custom/SDF_Perfect_Match"
{
    Properties
    {
        _SDFTex ("SDF Texture (R)", 2D) = "white" {}
        _Progress ("Progress (0=Full Rect -> 1=Shape)", Range(0, 1)) = 0.0
        _MainColor ("Shape Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _SDFTex;
            float _Progress;
            fixed4 _MainColor;

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert (float4 vertex : POSITION, float2 uv : TEXCOORD0) {
                v2f o;
                o.vertex = UnityObjectToClipPos(vertex);
                o.uv = uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
{
    // 1. Đọc giá trị SDF (0.0 ở biên Shape, 1.0 ở biên Rect)
    float sdfValue = tex2D(_SDFTex, i.uv).r;

    // 2. Ngưỡng Threshold:
    // Khi Progress = 0: threshold = 0.0 -> Hiện mọi pixel >= 0 (Full Rect)
    // Khi Progress = 1: threshold = 1.0 -> Chỉ hiện pixel >= 1.0 (Khớp biên Shape)
    // Lưu ý: Tùy vào file SDF bạn lưu là (s/(s+r)) hay (r/(s+r)) mà ta đảo ngược Progress
    float threshold = _Progress;

    // 3. Khử răng cưa
    float aa = fwidth(sdfValue);
    // Những pixel có giá trị SDF lớn hơn Progress sẽ được giữ lại
    float alpha = smoothstep(threshold - aa, threshold + aa, sdfValue);

    // 4. Viền cháy (Glow)
    float edgeWidth = 0.01;
    float edge = smoothstep(aa, -aa, abs(sdfValue - threshold) - edgeWidth);
    
    fixed4 col = _MainColor;
    fixed4 edgeColor = fixed4(1, 0.5, 0, 1) * 2.0; // Màu lửa HDR
    
    col = lerp(col, edgeColor, edge);
    col.a *= saturate(alpha + edge);

    return col;
}
            ENDCG
        }
    }
}