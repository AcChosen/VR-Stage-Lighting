Shader "VRSL/Standard Static/Surface Shaders/AlphaCutout"
{
    Properties
    {
         [Toggle] _EnableDMX ("Enable Stream DMX/DMX Control", Int) = 0
        _DMXChannel ("DMX Fixture Number/Sector (Per 13 Channels)", Int) = 0 
        [HideInInspector][Toggle] _NineUniverseMode ("Extended Universe Mode", Int) = 0
        [Toggle] _UseRawGrid("Use Raw Grid For Light Intensity", Int) = 0
		//[NoScaleOffset] _Udon_DMXGridRenderTexture("DMX Grid Render Texture (RAW Unsmoothed)", 2D) = "white" {}
		//[NoScaleOffset] _Udon_DMXGridRenderTextureMovement("DMX Grid Render Texture (To Control Lights)", 2D) = "white" {}
		//[NoScaleOffset] _Udon_DMXGridStrobeTimer("DMX Grid Render Texture (For Strobe Timings", 2D) = "white" {}
        [Toggle] _EnableCompatibilityMode ("Enable Compatibility Mode", Int) = 0
        [Toggle] _EnableVerticalMode ("Enable Vertical Mode", Int) = 0
       
		[Toggle] _EnableStrobe ("Enable Strobe", Int) = 0
		 //[HideInInspector][Toggle] _EnableDMX ("Enable Stream DMX/DMX Control", Int) = 0
		[HideInInspector]_StrobeFreq("Strobe Frequency", Range(0,25)) = 1
		[HideInInspector][Toggle] _EnableSpin("Enable Auto Spinning", Float) = 0  

        [HDR]_Emission("Light Color Tint", Color) = (1,1,1,1)     
        _EmissionMask ("Emission Mask", 2D) = "white" {}   
        _FixtureMaxIntensity ("Maximum Light Intensity",Range (0,55)) = 1
        _Saturation ("Color Saturation", Range (0,1)) = 0.95
        _GlobalIntensity("Global Intensity", Range(0,1)) = 1
        _GlobalIntensityBlend("Global Intensity Blend", Range(0,1)) = 1
        _FinalIntensity("Final Intensity", Range(0,1)) = 1
        _UniversalIntensity ("Universal Intensity", Range (0,1)) = 1
        _AlphaIntensity("Alpha Intensity", Range(0,1)) = 1
        [Toggle]_EnableAlphaDMX("Control Alpha With DMX", Range(0,1)) = 0
        
        _CurveMod ("Light Intensity Multiplier", Range (1,50)) = 5.0
        
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _MetallicSmoothness ("Metallic(R) / Smoothness(A) Map", 2D) = "white" {}
        _NormalMap ("Normal Map", 2D) = "white" {}
        _Metallic ("Metallic Blend", Range(0,1)) = 0.0
        _Glossiness ("Smoothness Blend", Range(0,1)) = 0.5
        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5
        [Enum(1CH,0,4CH,1,5CH,2,13CH,3)] _ChannelMode ("Channel Mode", Int) = 2

    }
    SubShader
    {
        Tags
        {
            "Queue" = "AlphaTest" "RenderType"="TransparentCutout" "RenderPipeline" = "UniversalPipeline"
        }
        LOD 200

        Pass
        {
            Name "ForwardLit"
            Tags
            {
                "LightMode" = "UniversalForward"
            }

            HLSLPROGRAM
            #pragma shader_feature UNIVERSAL_RENDER_PIPELINE
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile_fog
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma shader_feature _1CH_MODE _4CH_MODE _5CH_MODE _13CH_MODE

            #define VRSL_DMX

            // Use shader model 3.5 target, to get nicer looking lighting
            #pragma target 3.5

            #if UNIVERSAL_RENDER_PIPELINE
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.acchosen.vr-stage-lighting/Runtime/Shaders/Shared/VRSL-Defines-URP.hlsl"
            #include "../Shared/VRSL-DMXFunctions-URP.hlsl"
            #include "VRSL-StandardSurface-Functions-URP.hlsl"
            #endif

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float2 uv : TEXCOORD0;
                #if UNIVERSAL_RENDER_PIPELINE
                UNITY_VERTEX_INPUT_INSTANCE_ID
                #endif
            };

            struct Varyings
            {
                float2 uv_MainTex : TEXCOORD0;
                float2 uv_EmissionMask : TEXCOORD1;
                float2 uv_NormalMap : TEXCOORD2;
                float2 uv_MetallicSmoothness : TEXCOORD3;
                float3 normalWS : TEXCOORD4;
                float3 tangentWS : TEXCOORD5;
                float3 bitangentWS : TEXCOORD6;
                float3 positionWS : TEXCOORD7;
                float4 positionCS : SV_POSITION;
                float fogCoord : TEXCOORD8;
                #if UNIVERSAL_RENDER_PIPELINE
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
                #endif
            };

            #if UNIVERSAL_RENDER_PIPELINE
            TEXTURE2D(_EmissionMask);
            SAMPLER(sampler_EmissionMask);
            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);
            TEXTURE2D(_MetallicSmoothness);
            SAMPLER(sampler_MetallicSmoothness);

            CBUFFER_START(UnityPerMaterial)
                float4 _EmissionMask_ST;
                float4 _NormalMap_ST;
                float4 _MetallicSmoothness_ST;
                half _CurveMod;
                half _AlphaIntensity;
                half _EnableAlphaDMX;
                half _Cutoff;
            CBUFFER_END
            #endif

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                #if UNIVERSAL_RENDER_PIPELINE
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                // Transform positions and normals
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);

                // Copy values to output
                output.positionWS = vertexInput.positionWS;
                output.positionCS = vertexInput.positionCS;
                output.normalWS = normalInput.normalWS;
                output.tangentWS = normalInput.tangentWS;
                output.bitangentWS = normalInput.bitangentWS;

                // UVs
                output.uv_MainTex = TRANSFORM_TEX(input.uv, _MainTex);
                output.uv_EmissionMask = TRANSFORM_TEX(input.uv, _EmissionMask);
                output.uv_NormalMap = TRANSFORM_TEX(input.uv, _NormalMap);
                output.uv_MetallicSmoothness = TRANSFORM_TEX(input.uv, _MetallicSmoothness);

                // Fog
                output.fogCoord = ComputeFogFactor(vertexInput.positionCS.z);
                #endif
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                #if UNIVERSAL_RENDER_PIPELINE
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                // Sample textures
                half4 mainTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv_MainTex) * _Color;
                half4 ms = SAMPLE_TEXTURE2D(_MetallicSmoothness, sampler_MetallicSmoothness,
                                                                             input.uv_MetallicSmoothness);
                half3 normalTS = UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, input.uv_NormalMap));

                // Alpha calculation based on DMX mode
                half alpha = mainTex.a;
                half3 emission = half3(0, 0, 0);

                #if _1CH_MODE
                emission = GetDMXEmission1CH(input.uv_EmissionMask) * _CurveMod;
                alpha *= lerp(1.0, 0.0, GetDMXAlpha(getDMXChannel() + 1, _EnableAlphaDMX)) * _AlphaIntensity;
                #elif _4CH_MODE
                emission = GetDMXEmission4CH(input.uv_EmissionMask) * _CurveMod;
                alpha *= lerp(1.0, 0.0, GetDMXAlpha(getDMXChannel()+4, _EnableAlphaDMX)) * _AlphaIntensity;
                #elif _5CH_MODE
                emission = GetDMXEmission5CH(input.uv_EmissionMask) * _CurveMod;
                alpha *= lerp(1.0, 0.0, GetDMXAlpha(getDMXChannel()+5, _EnableAlphaDMX)) * _AlphaIntensity;
                #elif _13CH_MODE
                emission = GetDMXEmission13CH(input.uv_EmissionMask) * _CurveMod;
                alpha *= lerp(1.0, 0.0, GetDMXAlpha(getDMXChannel()+10, _EnableAlphaDMX)) * _AlphaIntensity;
                #endif

                // Alpha cutout
                clip(alpha - _Cutoff);

                // Transform normal from tangent to world space
                float3x3 tangentToWorld = float3x3(
                    normalize(input.tangentWS),
                    normalize(input.bitangentWS),
                    normalize(input.normalWS)
                );

                float3 normalWS = TransformTangentToWorld(normalTS, tangentToWorld);

                // PBR inputs
                InputData lightingInput = (InputData)0;
                lightingInput.positionWS = input.positionWS;
                lightingInput.normalWS = normalize(normalWS);
                lightingInput.viewDirectionWS = GetWorldSpaceNormalizeViewDir(input.positionWS);
                lightingInput.shadowCoord = TransformWorldToShadowCoord(input.positionWS);
                lightingInput.fogCoord = input.fogCoord;
                lightingInput.vertexLighting = half3(0, 0, 0);
                lightingInput.bakedGI = half3(0, 0, 0); // If using baked lighting, sample SH here
                lightingInput.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);
                lightingInput.shadowMask = half4(1, 1, 1, 1);

                // Surface data
                SurfaceData surfaceData = (SurfaceData)0;
                surfaceData.albedo = mainTex.rgb;
                surfaceData.metallic = _Metallic * ms.r;
                surfaceData.specular = half3(0, 0, 0);
                surfaceData.smoothness = _Glossiness * ms.a;
                surfaceData.occlusion = 1.0;
                surfaceData.emission = emission;
                surfaceData.alpha = alpha;
                surfaceData.clearCoatMask = 0;
                surfaceData.clearCoatSmoothness = 0;
                surfaceData.normalTS = normalTS;

                // Calculate final lighting
                half4 finalColor = UniversalFragmentPBR(lightingInput, surfaceData);

                // Apply fog
                finalColor.rgb = MixFog(finalColor.rgb, input.fogCoord);

                return finalColor;
                #else
            return (0).xxxx;
                #endif
            }
            ENDHLSL
        }

        // DepthOnly Pass, Required for depth priming
        Pass
        {
            Name "DepthOnly"
            Tags
            {
                "LightMode" = "DepthOnly"
            }

            ZWrite On
            ColorMask 0

            HLSLPROGRAM
            #pragma shader_feature UNIVERSAL_RENDER_PIPELINE
            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            #if defined(UNIVERSAL_RENDER_PIPELINE)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            #include "../Shared/VRSL-Defines-URP.hlsl"
            #endif

            struct Attributes
            {
                float4 position : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };

            Varyings DepthOnlyVertex(Attributes input)
            {
                Varyings output = (Varyings)0;
                #if defined(UNIVERSAL_RENDER_PIPELINE)
                output.positionCS = TransformObjectToHClip(input.position.xyz);
                #endif
                return output;
            }

            half4 DepthOnlyFragment(Varyings input) : SV_TARGET
            {
                return 0;
            }
            ENDHLSL
        }
        // DepthNormals Pass, Required for depth priming
        Pass
        {
            Name "DepthNormals"
            Tags
            {
                "LightMode" = "DepthNormals"
            }

            ZWrite On

            HLSLPROGRAM
            #pragma shader_feature UNIVERSAL_RENDER_PIPELINE
            #pragma vertex DepthNormalsVertex
            #pragma fragment DepthNormalsFragment

            #if defined(UNIVERSAL_RENDER_PIPELINE)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            #include "../Shared/VRSL-Defines-URP.hlsl"

            TEXTURE2D(_NormalMap);
            SAMPLER(sampler_NormalMap);
            #endif

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float2 texcoord : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float4 tangentWS : TEXCOORD2;
            };

            Varyings DepthNormalsVertex(Attributes input)
            {
                Varyings output = (Varyings)0;
                #if defined(UNIVERSAL_RENDER_PIPELINE)
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = TRANSFORM_TEX(input.texcoord, _MainTex);

                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
                output.normalWS = normalInput.normalWS;

                real sign = input.tangentOS.w * GetOddNegativeScale();
                output.tangentWS = float4(normalInput.tangentWS.xyz, sign);
                #endif
                return output;
            }

            half4 DepthNormalsFragment(Varyings input) : SV_TARGET
            {
                #if defined(UNIVERSAL_RENDER_PIPELINE)
                // Read normal map
                float3 normalTS = UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, input.uv));

                // Calculate tangent space to world space matrix
                float3 tangentWS = normalize(input.tangentWS.xyz);
                float3 bitangentWS = normalize(cross(input.normalWS, tangentWS) * input.tangentWS.w);

                // Transform normal from tangent to world space
                float3x3 tangentToWorld = float3x3(tangentWS, bitangentWS, input.normalWS);
                float3 normalWS = normalize(mul(normalTS, tangentToWorld));

                // Convert normal from [-1,1] to [0,1] range
                float3 normalViewSpace = TransformWorldToViewDir(normalWS) * 0.5 + 0.5;

                return half4(normalViewSpace, 0);
                #else
                return (0).xxxx;
                #endif
            }
            ENDHLSL
        }
    }

    SubShader
    {

        Tags {"Queue" = "AlphaTest" "RenderType"="TransparentCutout"  }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf StandardDefaultGI addshadow
        #define VRSL_DMX
        #define VRSL_SURFACE
        #pragma shader_feature _1CH_MODE _4CH_MODE _5CH_MODE _13CH_MODE
        #include "UnityPBSLighting.cginc"

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.5
        struct Input
        {
            float2 uv_MainTex;
            float2 uv_EmissionMask;
            float2 uv_NormalMap;
            float2 uv_MetallicSmoothness;

        };
        #include "Packages/com.acchosen.vr-stage-lighting/Runtime/Shaders/Shared/VRSL-Defines.cginc"
        half _CurveMod, _AlphaIntensity, _EnableAlphaDMX, _Cutoff;
        sampler2D _EmissionMask, _NormalMap, _MetallicSmoothness;
         #include "../Shared/VRSL-DMXFunctions.cginc"

        //half _Glossiness;
        //half _Metallic;
        //fixed4 _Color;

        inline half4 LightingStandardDefaultGI(SurfaceOutputStandard s, half3 viewDir, UnityGI gi)
        {
            return LightingStandard(s, viewDir, gi);
        }
        inline void LightingStandardDefaultGI_GI(SurfaceOutputStandard s, UnityGIInput data, inout UnityGI gi)
        {
            LightingStandard_GI(s, data, gi);
        }

        #include "VRSL-StandardSurface-Functions.cginc"

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color


          // o.Emission = DMXcol;
            fixed4 ms = tex2D (_MetallicSmoothness, IN.uv_MetallicSmoothness);

            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;

            #if _1CH_MODE
                o.Emission = GetDMXEmission1CH(IN.uv_EmissionMask) * _CurveMod;
                o.Alpha = c.a * lerp(1.0,0.0,GetDMXAlpha(getDMXChannel()+1, _EnableAlphaDMX)) * _AlphaIntensity;
            #elif _4CH_MODE
                o.Emission = GetDMXEmission4CH(IN.uv_EmissionMask) * _CurveMod;
                o.Alpha = c.a * lerp(1.0,0.0,GetDMXAlpha(getDMXChannel()+4, _EnableAlphaDMX)) * _AlphaIntensity;
            #elif _5CH_MODE
                o.Emission = GetDMXEmission5CH(IN.uv_EmissionMask) * _CurveMod;
                o.Alpha = c.a * lerp(1.0,0.0,GetDMXAlpha(getDMXChannel()+5, _EnableAlphaDMX)) * _AlphaIntensity;
            #elif _13CH_MODE
                o.Emission = GetDMXEmission13CH(IN.uv_EmissionMask) * _CurveMod;
                o.Alpha = c.a * lerp(1.0,0.0,GetDMXAlpha(getDMXChannel()+10, _EnableAlphaDMX)) * _AlphaIntensity;
            #endif
            
            if(o.Alpha < _Cutoff)
            {
                discard;
            }

            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic * ms.r;
            o.Smoothness = _Glossiness * ms.a;
            
            o.Normal = UnpackNormal (tex2D (_NormalMap, IN.uv_NormalMap));
        }
        ENDCG
    }
    CustomEditor "VRSLInspector"
   // FallBack "Diffuse"
}
