Shader "VRSL/Standard Static/Surface Shaders/Opaque"
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
        
        
        _CurveMod ("Light Intensity Multiplier", Range (1,50)) = 5.0
        
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _MetallicSmoothness ("Metallic(R) / Smoothness(A) Map", 2D) = "white" {}
        _NormalMap ("Normal Map", 2D) = "white" {}
        _Metallic ("Metallic Blend", Range(0,1)) = 0.0
        _Glossiness ("Smoothness Blend", Range(0,1)) = 0.5
        [Enum(1CH,0,4CH,1,5CH,2,13CH,3)] _ChannelMode ("Channel Mode", Int) = 2

    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }
        LOD 100

        // Forward Pass
        Pass
        {
            Name "ForwardLit"
            Tags
            {
                "LightMode" = "UniversalForward"
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #define VRSL_DMX
            #define VRSL_SURFACE
            #pragma shader_feature UNIVERSAL_RENDER_PIPELINE
            #pragma shader_feature _ _1CH_MODE _4CH_MODE _5CH_MODE _13CH_MODE
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT

            #if defined(UNIVERSAL_RENDER_PIPELINE)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            #include "../Shared/VRSL-Defines-URP.hlsl"
            #include "../Shared/VRSL-DMXFunctions-URP.hlsl"
            #include "VRSL-StandardSurface-Functions-URP.hlsl"
            #endif

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float2 uv : TEXCOORD0;
                #if defined(UNIVERSAL_RENDER_PIPELINE)
                UNITY_VERTEX_INPUT_INSTANCE_ID
                #endif
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 positionWS : TEXCOORD2;
                float4 tangentWS : TEXCOORD3;
                #if defined(UNIVERSAL_RENDER_PIPELINE)
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
                #endif
            };

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                #if defined(UNIVERSAL_RENDER_PIPELINE)
                // Positions
                VertexPositionInputs posInputs = GetVertexPositionInputs(input.positionOS.xyz);
                output.positionHCS = posInputs.positionCS;
                output.positionWS = posInputs.positionWS;

                // Normals and tangents
                VertexNormalInputs normalInputs = GetVertexNormalInputs(input.normalOS, input.tangentOS);
                output.normalWS = normalInputs.normalWS;

                // Calculate tangent for normal mapping
                real sign = input.tangentOS.w * GetOddNegativeScale();
                output.tangentWS = float4(normalInputs.tangentWS.xyz, sign);

                // UVs
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                #endif
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                #if defined(UNIVERSAL_RENDER_PIPELINE)
                // Sample textures
                half4 baseMap = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                half4 metallicSmoothness = SAMPLE_TEXTURE2D(_MetallicSmoothness, sampler_MetallicSmoothness, input.uv);
                half4 color = baseMap * _Color;

                // Calculate tangent space to world space matrix
                float3 normalTS = UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, input.uv));

                float3 normalWS;
                float3 tangentWS = normalize(input.tangentWS.xyz);
                float3 bitangentWS = normalize(cross(input.normalWS, tangentWS) * input.tangentWS.w);

                // Transform normal from tangent to world space
                float3x3 tangentToWorld = float3x3(tangentWS, bitangentWS, input.normalWS);
                normalWS = normalize(mul(normalTS, tangentToWorld));

                // Surface data
                InputData lightingInput = (InputData)0;
                lightingInput.normalWS = normalWS;
                lightingInput.positionWS = input.positionWS;
                lightingInput.viewDirectionWS = GetWorldSpaceNormalizeViewDir(input.positionWS);
                lightingInput.shadowCoord = TransformWorldToShadowCoord(input.positionWS);

                SurfaceData surfaceData = (SurfaceData)0;
                surfaceData.albedo = color.rgb;
                surfaceData.alpha = color.a;

                // Metallic in R channel, Smoothness in A channel (standard convention)
                surfaceData.metallic = metallicSmoothness.r * _Metallic;
                surfaceData.smoothness = metallicSmoothness.r * _Glossiness;

                // Albedo comes from a texture tinted by color
                float2 emUV = TRANSFORM_TEX(input.uv, _EmissionMask);
                #if _1CH_MODE
                    surfaceData.emission = GetDMXEmission1CH(emUV) * _CurveMod;
                #elif _4CH_MODE
                    surfaceData.emission = GetDMXEmission4CH(emUV) * _CurveMod;
                #elif _5CH_MODE
                    surfaceData.emission = GetDMXEmission5CH(emUV) * _CurveMod;
                #elif _13CH_MODE
                    surfaceData.emission = GetDMXEmission13CH(emUV) * _CurveMod;
                #endif

                surfaceData.normalTS = normalTS;

                // Apply lighting
                return UniversalFragmentPBR(lightingInput, surfaceData);
                #else
                return (0).xxxx;
                #endif
            }
            ENDHLSL
        }

        Pass
        {
            Name "ShadowCaster"
            Tags
            {
                "LightMode" = "ShadowCaster"
            }

            ZWrite On
            ZTest LEqual
            ColorMask 0

            HLSLPROGRAM
            #pragma shader_feature UNIVERSAL_RENDER_PIPELINE
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment

            #if defined(UNIVERSAL_RENDER_PIPELINE)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"

            #include "../Shared/VRSL-Defines-URP.hlsl"
            #endif

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 texcoord : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };

            float3 _LightDirection;

            #if defined(UNIVERSAL_RENDER_PIPELINE)
            float4 GetShadowPositionHClip(Attributes input)
            {
                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                float3 normalWS = TransformObjectToWorldNormal(input.normalOS);

                float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, _LightDirection));

                #if UNITY_REVERSED_Z
                positionCS.z = min(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
                #else
                    positionCS.z = max(positionCS.z, positionCS.w * UNITY_NEAR_CLIP_VALUE);
                #endif

                return positionCS;
            }
            #endif

            Varyings ShadowPassVertex(Attributes input)
            {
                Varyings output = (Varyings)0;
                #if defined(UNIVERSAL_RENDER_PIPELINE)
                output.positionCS = GetShadowPositionHClip(input);
                #endif
                return output;
            }

            half4 ShadowPassFragment(Varyings input) : SV_TARGET
            {
                return 0;
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

        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf StandardDefaultGI
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
        half _CurveMod;
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
            #if _1CH_MODE
                o.Emission = GetDMXEmission1CH(IN.uv_EmissionMask) * _CurveMod;
            #elif _4CH_MODE
                o.Emission = GetDMXEmission4CH(IN.uv_EmissionMask) * _CurveMod;
            #elif _5CH_MODE
                o.Emission = GetDMXEmission5CH(IN.uv_EmissionMask) * _CurveMod;
            #elif _13CH_MODE
                o.Emission = GetDMXEmission13CH(IN.uv_EmissionMask) * _CurveMod;
            #endif

          // o.Emission = DMXcol;
            fixed4 ms = tex2D (_MetallicSmoothness, IN.uv_MetallicSmoothness);

            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic * ms.r;
            o.Smoothness = _Glossiness * ms.a;
            o.Alpha = c.a;
            o.Normal = UnpackNormal (tex2D (_NormalMap, IN.uv_NormalMap));
        }
        ENDCG
    }
    CustomEditor "VRSLInspector"
    FallBack "Diffuse"
}
