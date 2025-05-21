Shader "Unlit/GenericUnlitTexture-Linear"
{
    Properties
    {
        _EmissionMap ("Texture", 2D) = "white" {}
        [Toggle(_)]_IsAVProInput("Is AV Pro Input", Int) = 0
        _TargetAspectRatio("Target Aspect Ratio", Float) = 1.7777777
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque" "RenderingPipeline"="UniversalPipeline"
        }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _EmissionMap;
            float4 _EmissionMap_ST;
            int _IsAVProInput;
            float _TargetAspectRatio;
            float4 _EmissionMap_TexelSize;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _EmissionMap);
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float2 emissionRes = _EmissionMap_TexelSize.zw;

                float currentAspectRatio = emissionRes.x / emissionRes.y;

                float visibility = 1.0;

                // If the aspect ratio does not match the target ratio, then we fit the UVs to maintain the aspect ratio while fitting the range 0-1
                if (abs(currentAspectRatio - _TargetAspectRatio) > 0.001)
                {
                    float2 normalizedVideoRes = float2(emissionRes.x / _TargetAspectRatio, emissionRes.y);
                    float2 correctiveScale;

                    // Find which axis is greater, we will clamp to that
                    if (normalizedVideoRes.x > normalizedVideoRes.y)
                        correctiveScale = float2(1, normalizedVideoRes.y / normalizedVideoRes.x);
                    else
                        correctiveScale = float2(normalizedVideoRes.x / normalizedVideoRes.y, 1);

                    uv = ((uv - 0.5) / correctiveScale) + 0.5;

                    // Antialiasing on UV clipping
                    float2 uvPadding = (1 / emissionRes) * 0.1;
                    float2 uvfwidth = fwidth(uv.xy);
                    float2 maxFactor = smoothstep(uvfwidth + uvPadding + 1, uvPadding + 1, uv.xy);
                    float2 minFactor = smoothstep(-uvfwidth - uvPadding, -uvPadding, uv.xy);

                    visibility = maxFactor.x * maxFactor.y * minFactor.x * minFactor.y;

                    //if (any(uv <= 0) || any(uv >= 1))
                    //    return float3(0, 0, 0);
                }
                // sample the texture
                float3 texColor = tex2D(_EmissionMap, _IsAVProInput ? float2(uv.x, 1 - uv.y) : uv).rgb;

                if (!_IsAVProInput)
                {
                    texColor = LinearToGammaSpace(texColor);
                }
                // if (_IsAVProInput)
                //     texColor = pow(texColor, 2.2f);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, float4(texColor,1));
                return float4(texColor, 1);
            }
            ENDCG
        }

        // Used for handling Depth Buffer (DBuffer) and Depth Priming
        UsePass "Universal Render Pipeline/Unlit/DepthOnly"
        UsePass "Universal Render Pipeline/Unlit/DepthNormals"
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _EmissionMap;
            float4 _EmissionMap_ST;
            int         _IsAVProInput;
            float _TargetAspectRatio;
            float4 _EmissionMap_TexelSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _EmissionMap);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float2 emissionRes = _EmissionMap_TexelSize.zw;

                float currentAspectRatio = emissionRes.x / emissionRes.y;

                float visibility = 1.0;

                // If the aspect ratio does not match the target ratio, then we fit the UVs to maintain the aspect ratio while fitting the range 0-1
                if (abs(currentAspectRatio - _TargetAspectRatio) > 0.001)
                {
                    float2 normalizedVideoRes = float2(emissionRes.x / _TargetAspectRatio, emissionRes.y);
                    float2 correctiveScale;
                    
                    // Find which axis is greater, we will clamp to that
                    if (normalizedVideoRes.x > normalizedVideoRes.y)
                        correctiveScale = float2(1, normalizedVideoRes.y / normalizedVideoRes.x);
                    else
                        correctiveScale = float2(normalizedVideoRes.x / normalizedVideoRes.y, 1);

                    uv = ((uv - 0.5) / correctiveScale) + 0.5;

                    // Antialiasing on UV clipping
                    float2 uvPadding = (1 / emissionRes) * 0.1;
                    float2 uvfwidth = fwidth(uv.xy);
                    float2 maxFactor = smoothstep(uvfwidth + uvPadding + 1, uvPadding + 1, uv.xy);
                    float2 minFactor = smoothstep(-uvfwidth - uvPadding, -uvPadding, uv.xy);

                    visibility = maxFactor.x * maxFactor.y * minFactor.x * minFactor.y;

                    //if (any(uv <= 0) || any(uv >= 1))
                    //    return float3(0, 0, 0);
                }
                // sample the texture
                float3 texColor = tex2D(_EmissionMap, _IsAVProInput ? float2(uv.x, 1 - uv.y) : uv).rgb;
                
                if (!_IsAVProInput)
                {
                    texColor = LinearToGammaSpace(texColor);
                }
                // if (_IsAVProInput)
                //     texColor = pow(texColor, 2.2f);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, float4(texColor,1));
                return float4(texColor,1);
            }
            ENDCG
        }
    }
}
