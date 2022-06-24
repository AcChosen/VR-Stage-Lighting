Shader "VRSL/Flasher Static/Fixture"
{
    Properties
    {
        //[Header (INSTANCED PROPERITES)]
		 _Sector ("DMX Fixture Number/Sector (Per 13 Channels)", Int) = 0
        [Toggle] _EnableOSC ("Enable Stream OSC/DMX Control", Int) = 0
        [Toggle] _EnableVerticalMode ("Enable Vertical Mode", Int) = 0
         [Toggle] _EnableCompatibilityMode ("Enable Compatibility Mode", Int) = 0
        _FixutreIntensityMultiplier ("Intensity Multipler (For Bloom Scaling)", Range(1,15)) = 1
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _NormalMap ("Normal Map", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _FinalIntensity("Final Intensity", Range(0,1)) = 1
        _GlobalIntensity("Global Intensity", Range(0,1)) = 1
        _UniversalIntensity ("Universal Intensity", Range (0,1)) = 1
        [HDR]_Emission("Light Color Tint", Color) = (1,1,1,1)
        _CurveMod ("Light Intensity Curve Modifier", Range (-3,8)) = 5.0
       // _EmissionMask ("Emission Mask", 2D) = "white" {}
        _FixtureMaxIntensity ("Maximum Light Intensity",Range (0,15)) = 1
		[NoScaleOffset] _OSCGridRenderTextureRAW("OSC Grid Render Texture (RAW Unsmoothed)", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue" = "Geometry+10" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows
        #pragma instancing_options nolightmap

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

      //  sampler2D _EmissionMask, _AlternateEmissionMask;
        sampler2D _NormalMap, _SamplingTexture;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_NormalMap;
            float4 color: Color;
        };
         #include "VRSL-FlasherFixtureMesh-Defines.cginc"
         half _CurveMod, _FixutreIntensityMultiplier, _ScrollIncrement;
         #include "../Shared/VRSL-DMXFunctions.cginc"
        // float GetTextureSampleScrollValue(uint dmx)
        // {
        //     return getValueAtCoords(0.960, standardSampleYAxis, dmx, _OSCGridRenderTextureRAW); 
        // }
            float GetChannelIntensity(uint DMXChannel)
            {
                float value = getValueAtCoords(DMXChannel, _OSCGridRenderTextureRAW);
                value = IF(value <= 0.1, 0.0, value);
                return value;
            }




        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            uint dmx = getDMXChannel();
            float4 OSCcol = float4(0,0,0,0);
            float4 col = IF(isOSC() == 1, GetChannelIntensity(dmx) * getEmissionColor(), getEmissionColor());
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            half4 e = col;
            o.Albedo = c.rgb;
            o.Normal = UnpackNormal (tex2D (_NormalMap, IN.uv_NormalMap));
            //e = IF(isOSC() == 1,lerp(half4(-_CurveMod,-_CurveMod,-_CurveMod,1), e, pow(GetOSCIntensity(dmx, 1.0), 1.0)), e);
            e = clamp(e, half4(0,0,0,1), half4(_FixtureMaxIntensity*2,_FixtureMaxIntensity*2,_FixtureMaxIntensity*2,1));
            float2 maskUVs = float2(IN.uv_MainTex.x + (_Time.y * _ScrollIncrement), IN.uv_MainTex.y);
            float mask = IF(IN.uv_MainTex.y > 0.5, 1, 0);
            e *= floor(mask);
            e*= _FixutreIntensityMultiplier;
            o.Emission = ((e.rgb * _FixtureMaxIntensity) * getGlobalIntensity()) * getFinalIntensity();
            o.Emission = o.Emission * _UniversalIntensity;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
     //CustomEditor "VRSLInspector"
  //  FallBack "Diffuse"
   
}
