Shader "AudioLink/VRSL Linear Interpolator"
{
    Properties
    {
        
         _MainTex ("Texture", 2D) = "white" {}
        [Header(Band Smoothing Values)]
        _Band0Smoothness ("Band 0 Smoothing (0 to 1, 0 = max)", Range(0,1)) = 0.5
        _Band1Smoothness ("Band 1 Smoothing (0 to 1, 0 = max)", Range(0,1)) = 0.5
        _Band2Smoothness ("Band 2 Smoothing (0 to 1, 0 = max)", Range(0,1)) = 0.5
        _Band3Smoothness ("Band 3 Smoothing (0 to 1, 0 = max)", Range(0,1)) = 0.5
        [Space(5)]
        _LightColorChordSmoothenss ("Color Chord Light Smoothness (0 to 1, 0 = max)", Range(0,1)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #include "UnityCustomRenderTexture.cginc"
            #include "UnityCG.cginc"
            #pragma vertex CustomRenderTextureVertexShader
            #pragma fragment frag
          //  #pragma target 3.0

            float _Band0Smoothness, _Band1Smoothness, _Band2Smoothness, _Band3Smoothness, _LightColorChordSmoothenss;
            // sampler2D   _Tex;
            // Texture2D _MainTex;
            SamplerState sampler_point_repeat;
            #define IF(a, b, c) lerp(b, c, step((fixed) (a), 0));

            // struct appdata
            // {
            //     float4 vertex : POSITION;
            //     float2 uv : TEXCOORD0;
            // };

            // struct v2f
            // {
            //     float2 uv : TEXCOORD0;
            //     UNITY_FOG_COORDS(1)
            //     float4 vertex : SV_POSITION;
            // };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            // v2f vert (appdata v)
            // {
            //     v2f o;
            //     o.vertex = UnityObjectToClipPos(v.vertex);
            //     o.uv = TRANSFORM_TEX(v.uv, _MainTex);
            //     UNITY_TRANSFER_FOG(o,o.vertex);
            //     return o;
            // }

            // fixed4 frag (v2f i) : SV_Target
            // {
            //     // sample the texture
            //     fixed4 col = tex2D(_MainTex, i.uv);
            //     // apply fog
            //     UNITY_APPLY_FOG(i.fogCoord, col);
            //     return col;
            // }

            float4 frag(v2f_customrendertexture IN) : COLOR
            {
                if (_Time.y > 2.0)
                 {
                    float smoothness = _Band0Smoothness;
                    float2 sampleCoords = float2(IN.localTexcoord.x, 0.006729);

                    smoothness = IF(IN.localTexcoord.y > 0.013463, _Band1Smoothness, smoothness);
                    sampleCoords = IF(IN.localTexcoord.y > 0.013463, float2(IN.localTexcoord.x, 0.023666), sampleCoords);

                    smoothness = IF(IN.localTexcoord.y > 0.038536, _Band2Smoothness, smoothness);
                    sampleCoords = IF(IN.localTexcoord.y > 0.038536, float2(IN.localTexcoord.x, 0.039392), sampleCoords);

                    smoothness = IF(IN.localTexcoord.y > 0.054684, _Band3Smoothness, smoothness);
                    sampleCoords = IF(IN.localTexcoord.y > 0.054684, float2(IN.localTexcoord.x, 0.054907), sampleCoords);

                    smoothness = IF(IN.localTexcoord.y > 0.408014 && IN.localTexcoord.y < 0.419178, _LightColorChordSmoothenss, smoothness);
                    sampleCoords = IF(IN.localTexcoord.y > 0.408014 && IN.localTexcoord.y < 0.419178, float2(IN.localTexcoord.x, 0.419178), sampleCoords);

                    float4 previousFrame = tex2D(_SelfTexture2D, IN.localTexcoord.xy);
                    float4 currentFrame = tex2D(_MainTex, sampleCoords);
                    float currentFrameTime = unity_DeltaTime.z; // in seconds
                    if(IN.localTexcoord.y >= 0.066373)
                    {

                        return IF(IN.localTexcoord.y > 0.408014 && IN.localTexcoord.y < 0.419178, lerp(clamp(lerp(previousFrame, currentFrame, clamp(currentFrameTime,0.0,1.0)), 0.0, 400.0), currentFrame, smoothness), tex2D(_MainTex, IN.localTexcoord.xy));
                    }
                    else
                    {
                        return lerp(clamp(lerp(previousFrame, currentFrame,clamp(currentFrameTime,0.0,1.0)), 0.0, 400.0), currentFrame, smoothness);
                    }
                 }
                 else
                 {
                     return tex2D(_MainTex, IN.localTexcoord.xy);
                 }
            }

            ENDCG
        }
    }
}
