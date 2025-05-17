Shader "VRSL/Other/Discoball"
 {
     Properties
     {
         [HideInInspector]_DMXChannel ("DMX Fixture Number/Sector (Per 13 Channels)", Int) = 0
         [HideInInspector][Toggle] _NineUniverseMode ("Extended Universe Mode", Int) = 0

         [Toggle] _EnableCompatibilityMode ("Enable Compatibility Mode", Int) = 0
         [Toggle] _EnableVerticalMode ("Enable Vertical Mode", Int) = 0
         [Toggle] _EnableDMX ("Enable Stream DMX/DMX Control", Int) = 0
        //  [NoScaleOffset] _Udon_DMXGridRenderTexture("DMX Grid Render Texture (RAW Unsmoothed)", 2D) = "white" {}
         _GlobalIntensity("Global Intensity", Range(0,1)) = 1
         _FinalIntensity("Final Intensity", Range(0,1)) = 1
         _UniversalIntensity ("Universal Intensity", Range (0,1)) = 1
         [HDR]_Emission ("Color", Color) = (1.0, 1.0, 1.0, .2)
         _Cube ("Projection Map", Cube) = "" {}
         [Toggle] _UseWorldNorm("Use World Normal vs View Normal", Float) = 0
         _RotationSpeed ("Rotation Speed", Range (-180,180)) = 8.2
         _Multiplier("Brightness Multiplier", Range(0, 10)) = 1
         [Enum(Transparent,1,AlphaToCoverage,2)] _RenderMode ("Render Mode", Int) = 1
        [Enum(Off,0,On,1)] _ZWrite ("Z Write", Int) = 0
		[Enum(Off,0,On,1)] _AlphaToCoverage ("Alpha To Coverage", Int) = 0
        [Enum(Off,0,One,1)] _BlendDst ("Destination Blend mode", Float) = 1
		[Enum(UnityEngine.Rendering.BlendOp)] _BlendOp ("Blend Operation", Float) = 0
        _ClippingThreshold ("Clipping Threshold", Range (0,1)) = 0.5
        _GlobalIntensityBlend("Global Intensity Blend", Range(0,1)) = 1

     }
     SubShader
     {
         Tags { "Queue" = "Transparent+1"
             "ForceNoShadowCasting"="True"
             "IgnoreProjector"="True"
             "RenderType" = "Transparent"
             "RenderingPipeline"="UniversalPipeline"
         }
         Offset -1, -5
         Stencil
         {
            Ref 142
            Comp NotEqual
            Pass Keep
         }

         Pass
         {
            AlphaToMask [_AlphaToCoverage]
            Cull Front
            Ztest Greater
            ZWrite  Off
            Blend DstColor [_BlendDst]
            Lighting Off
		    SeparateSpecular Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_local _ _ALPHATEST_ON
            #define VRSL_DMX
            uniform samplerCUBE _Cube;
            float4 _Cube_ST;
            float _RotationSpeed;
            #include "UnityCG.cginc"
             struct appdata
             {
                 float4 vertex : POSITION;
                 float2 uv : TEXCOORD0;
                 float3 texcoord : TEXCOORD1;
                 UNITY_VERTEX_INPUT_INSTANCE_ID
             };
             struct v2f
             {
                 float4 vertex : SV_POSITION;
                 float2 uv : TEXCOORD0;
                 float3 ray : TEXCOORD2;
                 float4 screenPos : TEXCOORD4;
                 float4 worldDirection : TEXCOORD5;
                 float4 worldPos : TEXCOORD6;
                 float2 dmxIntensity: TEXCOORD7;
                 UNITY_VERTEX_OUTPUT_STEREO
             };
            #include "../Shared/VRSL-Defines.cginc"
            half _Multiplier;
			#include "../Shared/VRSL-DMXFunctions.cginc"

             float4 Rotation(float4 vertPos)
             {
	            //CALCULATE BASE ROTATION. MORE FUN MATH. THIS IS FOR PAN.
                float angleY = radians(_Time.y * _RotationSpeed);
                float c = cos(angleY);
                float s = sin(angleY);
                float4x4 rotateYMatrix = float4x4(c, 0, s, 0,
                    0, 1, 0, 0,
                    -s, 0, c, 0,
                    0, 0, 0, 1);
                return mul(rotateYMatrix, vertPos);
             }

             inline float4 CalculateFrustumCorrection()
             {
                 float x1 = -UNITY_MATRIX_P._31/(UNITY_MATRIX_P._11*UNITY_MATRIX_P._34);
                 float x2 = -UNITY_MATRIX_P._32/(UNITY_MATRIX_P._22*UNITY_MATRIX_P._34);
                 return float4(x1, x2, 0, UNITY_MATRIX_P._33/UNITY_MATRIX_P._34 + x1*UNITY_MATRIX_P._13 + x2*UNITY_MATRIX_P._23);
             }

             //CREDIT TO DJ LUKIS FOR MIRROR DEPTH CORRECTION
             inline float CorrectedLinearEyeDepth(float z, float B)
             {
                 return 1.0 / (z/UNITY_MATRIX_P._34 + B);
             }

             v2f vert(appdata v)
             {
                 v2f o;
                 UNITY_SETUP_INSTANCE_ID(v);
                 UNITY_INITIALIZE_OUTPUT(v2f, o); //DON'T INITIALIZE OR IT WILL BREAK PROJECTION
                 UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                 // UNITY_TRANSFER_INSTANCE_ID(v, o);
                 o.vertex = UnityObjectToClipPos(v.vertex);
                 o.ray = UnityObjectToViewPos(v.vertex).xyz;
                 o.ray = o.ray.xyz * float3(-1,-1,1);
                 o.ray = lerp(o.ray, v.texcoord, v.texcoord.z != 0);
                 o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                 o.screenPos = ComputeScreenPos(o.vertex);
                 o.worldDirection.xyz = o.worldPos.xyz - _WorldSpaceCameraPos;
                 // pack correction factor into direction w component to save space
                 o.worldDirection.w = dot(o.vertex, CalculateFrustumCorrection());
                 uint dmx = getDMXChannel();
                 o.dmxIntensity = IF(_EnableCompatibilityMode == 1, float2(dmx, getValueAtCoords(dmx, _Udon_DMXGridRenderTexture)), float2(dmx, getValueAtCoords(dmx, _Udon_DMXGridRenderTexture)));
                 if(o.dmxIntensity.y <= 0.05 && _EnableDMX == 1)
                 {
                     v.vertex = float4(0,0,0,0);
                     o.vertex = UnityObjectToClipPos(v.vertex);

                 }
                 return o;
             }

             #define IF(a, b, c) lerp(b, c, step((fixed) (a), 0));

             fixed4 frag(v2f i) : SV_Target
             {
                 UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                 if(i.dmxIntensity.y <= 0.05 && _EnableDMX == 1)
                 {
                     return half4(0,0,0,0);
                 }
                 #if _ALPHATEST_ON
                     float2 pos = i.screenPos.xy / i.screenPos.w;
                     pos *= _ScreenParams.xy;
                     float DITHER_THRESHOLDS[16] =
                     {
                         1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
                         13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
                         4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
                         16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
                     };
                     int index = (int)((uint(pos.x) % 4) * 4 + uint(pos.y) % 4);
		         #endif
                 float4 depthdirect = i.worldDirection * (1.0f / i.vertex.w);
                 float sceneZ = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.screenPos.xy / i.screenPos.w);
                 #if UNITY_REVERSED_Z
                     if (sceneZ == 0) return half4(0,0,0,0);
                 #else
                     sceneZ = lerp(UNITY_NEAR_CLIP_VALUE, 1, sceneZ);
                     if (sceneZ == 1) return half4(0,0,0,0);
                 #endif

                 float depth = CorrectedLinearEyeDepth(sceneZ, depthdirect.w);
                 i.ray = i.ray * (_ProjectionParams.z / i.ray.z);
                 depth = Linear01Depth((1.0 - (depth * _ZBufferParams.w)) / (depth * _ZBufferParams.z));
                 float3 wpos = mul(unity_CameraToWorld, float4(i.ray * depth, 1)).xyz;
                 float UVscale = pow(abs(distance( mul(unity_ObjectToWorld, float4(0.0,0.0,0.0,1.0) ).xyz,wpos)),-1);
                 float3 projPos = (mul(unity_WorldToObject,float4(wpos, 1)));
                 if(0.0 < abs(projPos.x) < 0.1)
                 {
                     return half4(0,0,0,0);
                 }
                 projPos = Rotation(float4(projPos, 0)).xyz;
                 float4 col = (texCUBE (_Cube, projPos));
                 col = col *(_Emission * (4*UVscale));
                 col = IF(_EnableDMX == 1, col * i.dmxIntensity.y, col);
                 col = (col * _Multiplier)*((col * getGlobalIntensity()) * getFinalIntensity());
                 col = col * _UniversalIntensity;

                 #ifdef _ALPHATEST_ON
                     clip(col.a - DITHER_THRESHOLDS[index]);
                     clip((((col.r + col.g + col.b)/3) * (_ClippingThreshold)) - DITHER_THRESHOLDS[index]);
                     return col;
                 #else
                         return col;
                 #endif
             }
             ENDCG
         }//end color pass
     }
     SubShader
     {
         Tags{ "Queue" = "Transparent+1" "ForceNoShadowCasting"="True" "IgnoreProjector"="True" "RenderType" = "Transparent" }
         Offset -1, -5
        	Stencil
			{
				Ref 142
				Comp NotEqual
				Pass Keep
			}

         Pass
         {
            AlphaToMask [_AlphaToCoverage]
            Cull Front
            Ztest Greater
            ZWrite  Off
            Blend DstColor [_BlendDst]
            Lighting Off
		    SeparateSpecular Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_local _ _ALPHATEST_ON
            #define VRSL_DMX
            uniform samplerCUBE _Cube;
            float4 _Cube_ST;
            float _RotationSpeed;
            #include "UnityCG.cginc"
             struct appdata
             {
                 float4 vertex : POSITION;
                 float2 uv : TEXCOORD0;
                 float3 texcoord : TEXCOORD1;
             };
             struct v2f
             {
                 float4 vertex : SV_POSITION;
                 float2 uv : TEXCOORD0;
                 float3 ray : TEXCOORD2;
                 float4 screenPos : TEXCOORD4;
                 float4 worldDirection : TEXCOORD5;
                 float4 worldPos : TEXCOORD6;
                 float2 dmxIntensity: TEXCOORD7;
                 UNITY_VERTEX_OUTPUT_STEREO 
             };
            #include "../Shared/VRSL-Defines.cginc"
            half _Multiplier;
			#include "../Shared/VRSL-DMXFunctions.cginc"

             float4 Rotation(float4 vertPos)
             {
                 
	            //CALCULATE BASE ROTATION. MORE FUN MATH. THIS IS FOR PAN.
                float angleY = radians(_Time.y * _RotationSpeed);
                float c = cos(angleY);
                float s = sin(angleY);
                float4x4 rotateYMatrix = float4x4(c, 0, s, 0,
                    0, 1, 0, 0,
                    -s, 0, c, 0,
                    0, 0, 0, 1);

                return mul(rotateYMatrix, vertPos);
             }
            inline float4 CalculateFrustumCorrection()
            {
                float x1 = -UNITY_MATRIX_P._31/(UNITY_MATRIX_P._11*UNITY_MATRIX_P._34);
                float x2 = -UNITY_MATRIX_P._32/(UNITY_MATRIX_P._22*UNITY_MATRIX_P._34);
                return float4(x1, x2, 0, UNITY_MATRIX_P._33/UNITY_MATRIX_P._34 + x1*UNITY_MATRIX_P._13 + x2*UNITY_MATRIX_P._23);
            }

             
            //CREDIT TO DJ LUKIS FOR MIRROR DEPTH CORRECTION
            inline float CorrectedLinearEyeDepth(float z, float B)
            {
                return 1.0 / (z/UNITY_MATRIX_P._34 + B);
            }


             v2f vert(appdata v)
             {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o); //DON'T INITIALIZE OR IT WILL BREAK PROJECTION
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
               // UNITY_TRANSFER_INSTANCE_ID(v, o);
                        
    
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.ray = UnityObjectToViewPos(v.vertex).xyz;
                o.ray = o.ray.xyz * float3(-1,-1,1);
                o.ray = lerp(o.ray, v.texcoord, v.texcoord.z != 0);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.screenPos = ComputeScreenPos(o.vertex);
                o.worldDirection.xyz = o.worldPos.xyz - _WorldSpaceCameraPos;
                // pack correction factor into direction w component to save space
                o.worldDirection.w = dot(o.vertex, CalculateFrustumCorrection());
                uint dmx = getDMXChannel();
                o.dmxIntensity = IF(_EnableCompatibilityMode == 1, float2(dmx, getValueAtCoords(dmx, _Udon_DMXGridRenderTexture)), float2(dmx, getValueAtCoords(dmx, _Udon_DMXGridRenderTexture)));
                if(o.dmxIntensity.y <= 0.05 && _EnableDMX == 1)
                {
                    v.vertex = float4(0,0,0,0);
                    o.vertex = UnityObjectToClipPos(v.vertex);

                }
                return o;
             }

             #define IF(a, b, c) lerp(b, c, step((fixed) (a), 0));


             fixed4 frag(v2f i) : SV_Target
             {
                if(i.dmxIntensity.y <= 0.05 && _EnableDMX == 1)
                {
                    return half4(0,0,0,0);
                }
                #if _ALPHATEST_ON
                    float2 pos = i.screenPos.xy / i.screenPos.w;
                    pos *= _ScreenParams.xy;
                    float DITHER_THRESHOLDS[16] =
                    {
                        1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
                        13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
                        4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
                        16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
                    };
                    int index = (int)((uint(pos.x) % 4) * 4 + uint(pos.y) % 4);
		        #endif
                float4 depthdirect = i.worldDirection * (1.0f / i.vertex.w);
                float sceneZ = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.screenPos.xy / i.screenPos.w);
                #if UNITY_REVERSED_Z
                    if (sceneZ == 0)
                #else
                    if (sceneZ == 1)
                #endif
                        return half4(0,0,0,0);
                        
                float depth = CorrectedLinearEyeDepth(sceneZ, depthdirect.w);
                i.ray = i.ray * (_ProjectionParams.z / i.ray.z); 
                depth = Linear01Depth((1.0 - (depth * _ZBufferParams.w)) / (depth * _ZBufferParams.z));
                float3 wpos = mul(unity_CameraToWorld, float4(i.ray * depth, 1)).xyz;
                float UVscale = pow(abs(distance( mul(unity_ObjectToWorld, float4(0.0,0.0,0.0,1.0) ).xyz,wpos)),-1);
                float3 projPos = (mul(unity_WorldToObject,float4(wpos, 1)));
                if(0.0 < abs(projPos.x) < 0.1)
                {
                    return half4(0,0,0,0);
                }
                projPos = Rotation(float4(projPos, 0)).xyz;
                float4 col = (texCUBE (_Cube, projPos));
                col = col *(_Emission * (4*UVscale));
                col = IF(_EnableDMX == 1, col * i.dmxIntensity.y, col);
                col = (col * _Multiplier)*((col * getGlobalIntensity()) * getFinalIntensity());
                col = col * _UniversalIntensity;

                
                #ifdef _ALPHATEST_ON
                    clip(col.a - DITHER_THRESHOLDS[index]);
                    clip((((col.r + col.g + col.b)/3) * (_ClippingThreshold)) - DITHER_THRESHOLDS[index]);
                    return col;
                #else
                        return col;
                #endif
             }
             ENDCG
         }//end color pass
     }
     CustomEditor "VRSLInspector"
 }