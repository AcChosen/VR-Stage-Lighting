Shader "VRSL/AudioLink/Standard Static/Fixture"
{
    Properties
    {
      //  [Header (INSTANCED PROPERITES)]
		 [HideInInspector]_Sector ("DMX Fixture Number/Sector (Per 13 Channels)", Int) = 0

		 [Toggle] _EnableStrobe ("Enable Strobe", Int) = 0
		 //[HideInInspector][Toggle] _EnableOSC ("Enable Stream OSC/DMX Control", Int) = 0
		 [HideInInspector]_StrobeFreq("Strobe Frequency", Range(0,25)) = 1
		 [HideInInspector][Toggle] _EnableSpin("Enable Auto Spinning", Float) = 0


        //Color Texture Sampling Properties
		 [Toggle] _EnableColorTextureSample ("Enable Color Texture Sampling", Int) = 0
         [Toggle] _EnableColorChord ("Enable Color Chord Tinting", Int) = 0
		 _SamplingTexture ("Texture To Sample From for Color", 2D) = "white" {}
		 _TextureColorSampleX ("X coordinate to sample the texture from", Range(0,1)) = 0.5
		 _TextureColorSampleY ("Y coordinate to sample the texture from", Range(0,1)) = 0.5
        _RenderTextureMultiplier("Render Texture Multiplier", Range(1,10)) = 1

        [Toggle] _EnableOSC ("Enable Stream OSC/DMX Control", Int) = 0
        _FixutreIntensityMultiplier ("Intensity Multipler (For Bloom Scaling)", Range(1,15)) = 1
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _NormalMap ("Normal Map", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        [Toggle]_UseTraditionalSampling("Use Traditional Texture Sampling", Int) = 0

        [Header(Audio Section)]
         [Toggle]_EnableAudioLink("Enable Audio Link", Float) = 0
         _Band("Band", Float) = 0
         _BandMultiplier("Band Multiplier", Range(1, 15)) = 1
         _Delay("Delay", Float) = 0
         _NumBands("Num Bands", Float) = 4
         _AudioSpectrum("AudioSpectrum", 2D) = "black" {}
        _FinalIntensity("Final Intensity", Range(0,1)) = 1
        _GlobalIntensity("Global Intensity", Range(0,1)) = 1
        _GlobalIntensityBlend("Global Intensity Blend", Range(0,1)) = 1
        _UniversalIntensity ("Universal Intensity", Range (0,1)) = 1
        [HDR]_Emission("Light Color Tint", Color) = (1,1,1,1)
        _CurveMod ("Light Intensity Curve Modifier", Range (-3,8)) = 5.0
        _EmissionMask ("Emission Mask", 2D) = "white" {}
        _FixtureMaxIntensity ("Maximum Light Intensity",Range (0,15)) = 1
        [Toggle] _UseRawGrid("Use Raw Grid For Light Intensity", Int) = 0
		[NoScaleOffset] _OSCGridRenderTextureRAW("OSC Grid Render Texture (RAW Unsmoothed)", 2D) = "white" {}
		[NoScaleOffset] _OSCGridRenderTexture("OSC Grid Render Texture (To Control Lights)", 2D) = "white" {}
		[NoScaleOffset] _OSCGridStrobeTimer ("OSC Grid Render Texture (For Strobe Timings", 2D) = "white" {}
		//[NoScaleOffset] _SceneAlbedo ("Scene Albedo Render Texture", 2D) = "white" {}+
        		[Toggle] _EnableThemeColorSampling ("Enable Theme Color Sampling", Int) = 0
		 _ThemeColorTarget ("Choose Theme Color", Int) = 0
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque" "RenderingPipeline" = "UniversalPipeline"
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

            #pragma target 3.0

            // URP Keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK
            #pragma multi_compile_fog

            #if defined(UNIVERSAL_RENDER_PIPELINE)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            #define STATIC_FIXTURE
            #define VRSL_AUDIOLINK
            #define VRSL_SURFACE

            #include "../../Shared/VRSL-Defines-URP.hlsl"
            #include "../Shared/VRSL-AudioLink-Functions-URP.hlsl"
            #endif

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                #if defined(UNIVERSAL_RENDER_PIPELINE)
                UNITY_VERTEX_INPUT_INSTANCE_ID
                #endif
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float3 normalWS : TEXCOORD2;
                float4 tangentWS : TEXCOORD3;
                float3 viewDirWS : TEXCOORD4;
                half4 fogFactorAndVertexLight : TEXCOORD5;
                #ifdef _MAIN_LIGHT_SHADOWS
                float4 shadowCoord : TEXCOORD6;
                #endif
                #if defined(UNIVERSAL_RENDER_PIPELINE)
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
                #endif
            };

            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;

                #if defined(UNIVERSAL_RENDER_PIPELINE)
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);

                output.positionCS = vertexInput.positionCS;
                output.positionWS = vertexInput.positionWS;
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                output.normalWS = normalInput.normalWS;

                real sign = input.tangentOS.w * GetOddNegativeScale();
                output.tangentWS = float4(normalInput.tangentWS.xyz, sign);
                output.viewDirWS = GetWorldSpaceViewDir(vertexInput.positionWS);

                // Vertex lighting and fog
                half3 vertexLight = VertexLighting(vertexInput.positionWS, normalInput.normalWS);
                half fogFactor = ComputeFogFactor(vertexInput.positionCS.z);
                output.fogFactorAndVertexLight = half4(fogFactor, vertexLight);

                #ifdef _MAIN_LIGHT_SHADOWS
                output.shadowCoord = GetShadowCoord(vertexInput);
                #endif
                #endif

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                #if defined(UNIVERSAL_RENDER_PIPELINE)
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                // Sample textures
                half4 albedoAlpha = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv) * _Color;
                half3 normalTS = UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, input.uv));

                // Setup SurfaceData
                SurfaceData surfaceData = (SurfaceData)0;
                surfaceData.albedo = albedoAlpha.rgb;
                surfaceData.alpha = albedoAlpha.a;
                surfaceData.metallic = _Metallic;
                surfaceData.smoothness = _Glossiness;
                surfaceData.normalTS = normalTS;

                // Convert TBN from TS to WS
                float3 normalWS = TransformTangentToWorld(
                    normalTS,
                    half3x3(input.tangentWS.xyz, cross(input.normalWS, input.tangentWS.xyz) * input.tangentWS.w,
                                          input.normalWS)
                );
                normalWS = normalize(normalWS);

                // Calculate shadow coords
                #ifdef _MAIN_LIGHT_SHADOWS
                float4 shadowCoord = input.shadowCoord;
                #else
                float4 shadowCoord = float4(0, 0, 0, 0);
                #endif

                // Setup InputData
                InputData inputData = (InputData)0;
                inputData.positionWS = input.positionWS;
                inputData.normalWS = normalWS;
                inputData.viewDirectionWS = SafeNormalize(input.viewDirWS);
                inputData.shadowCoord = shadowCoord;
                inputData.fogCoord = input.fogFactorAndVertexLight.x;
                inputData.vertexLighting = input.fogFactorAndVertexLight.yzw;
                inputData.bakedGI = SampleSH(normalWS);

                // Emission calculation
                half4 e = getEmissionColor();
                e = clamp(e, half4(0, 0, 0, 1),
                half4(_FixtureMaxIntensity * 2, _FixtureMaxIntensity * 2, _FixtureMaxIntensity * 2, 1));
                e *= SAMPLE_TEXTURE2D(_EmissionMask, sampler_EmissionMask, input.uv).r;
                e *= _FixutreIntensityMultiplier;
                half3 emission = (e.rgb * _FixtureMaxIntensity) * GetAudioReactAmplitude();
                emission = (emission * getGlobalIntensity()) * getFinalIntensity();
                emission = emission * _UniversalIntensity;

                // Add emission to surface data
                surfaceData.emission = emission;

                // Calculate lighting
                half4 color = UniversalFragmentPBR(inputData, surfaceData);

                // Apply fog
                color.rgb = MixFog(color.rgb, inputData.fogCoord);

                return color;
                #else
                return (0).xxxx;
                #endif
            }
            ENDHLSL
        }

        // URP shadow casting pass
        // ShadowCaster Pass
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

            #include "../../Shared/VRSL-Defines-URP.hlsl"
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

        // URP depth-only pass
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

            #include "../../Shared/VRSL-Defines-URP.hlsl"
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

            #include "../../Shared/VRSL-Defines-URP.hlsl"

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
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _EmissionMask;
        sampler2D _NormalMap;
        #define STATIC_FIXTURE
        #define VRSL_AUDIOLINK
        #define VRSL_SURFACE
        struct Input
        {
            float2 uv_MainTex;
            float2 uv_NormalMap;
        };
         #include "Packages/com.acchosen.vr-stage-lighting/Runtime/Shaders/Shared/VRSL-Defines.cginc"
         half _CurveMod;
         #include "../Shared/VRSL-AudioLink-Functions.cginc"


        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            //hello
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            half4 e = getEmissionColor();
            
            o.Albedo = c.rgb;
            o.Normal = UnpackNormal (tex2D (_NormalMap, IN.uv_NormalMap));
            e = clamp(e, half4(0,0,0,1), half4(_FixtureMaxIntensity*2,_FixtureMaxIntensity*2,_FixtureMaxIntensity*2,1));
            e *= tex2D(_EmissionMask, IN.uv_MainTex).r;
            e*= _FixutreIntensityMultiplier;
            o.Emission = (e.rgb * _FixtureMaxIntensity) * GetAudioReactAmplitude();
            o.Emission = (o.Emission * getGlobalIntensity()) * getFinalIntensity();
            o.Emission = o.Emission * _UniversalIntensity;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
    CustomEditor "VRSLInspector"
}
