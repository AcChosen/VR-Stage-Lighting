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
