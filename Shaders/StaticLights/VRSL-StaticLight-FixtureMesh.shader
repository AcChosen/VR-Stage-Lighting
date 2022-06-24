Shader "VRSL/Standard Static/Fixture"
{
    Properties
    {
        //[Header (INSTANCED PROPERITES)]
		 [HideInInspector]_DMXChannel ("DMX Fixture Number/Sector (Per 13 Channels)", Int) = 0

		 [Toggle] _EnableStrobe ("Enable Strobe", Int) = 0
		 //[HideInInspector][Toggle] _EnableOSC ("Enable Stream OSC/DMX Control", Int) = 0
		 [HideInInspector]_StrobeFreq("Strobe Frequency", Range(0,25)) = 1
		 [HideInInspector][Toggle] _EnableSpin("Enable Auto Spinning", Float) = 0

         [Toggle] _EnableCompatibilityMode ("Enable Compatibility Mode", Int) = 0
         [Toggle] _EnableVerticalMode ("Enable Vertical Mode", Int) = 0
        [Toggle] _EnableOSC ("Enable Stream OSC/DMX Control", Int) = 0
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
        _EmissionMask ("Emission Mask", 2D) = "white" {}
        _FixtureMaxIntensity ("Maximum Light Intensity",Range (0,15)) = 1
        [Toggle] _UseRawGrid("Use Raw Grid For Light Intensity", Int) = 0
		[NoScaleOffset] _OSCGridRenderTextureRAW("OSC Grid Render Texture (RAW Unsmoothed)", 2D) = "white" {}
		[NoScaleOffset] _OSCGridRenderTexture("OSC Grid Render Texture (To Control Lights)", 2D) = "white" {}
		[NoScaleOffset] _OSCGridStrobeTimer ("OSC Grid Render Texture (For Strobe Timings", 2D) = "white" {}
		//[NoScaleOffset] _SceneAlbedo ("Scene Albedo Render Texture", 2D) = "white" {}
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

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_NormalMap;
        };
         #include "VRSL-StaticLight-FixtureMesh-Defines.cginc"
         half _CurveMod, _FixutreIntensityMultiplier;
         #include "../Shared/VRSL-DMXFunctions.cginc"


        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float3 worldCam;
            worldCam.x = unity_CameraToWorld[0][3];
            worldCam.y = unity_CameraToWorld[1][3];
            worldCam.z = unity_CameraToWorld[2][3];
            float3 objCamPos = mul(unity_WorldToObject, float4(worldCam, 1)).xyz;
            float len = length(objCamPos.xy);
            len *= len;
            float blindingEffect = clamp(0.6/len,1.0,16.0);

            uint dmx = getDMXChannel();
            float strobe = IF(isStrobe() == 1, GetStrobeOutput(dmx), 1);
            float4 OSCcol = getEmissionColor();
            OSCcol *= GetOSCColor(dmx);
            float4 col = IF(isOSC() == 1, OSCcol, getEmissionColor());
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            half4 e = col * strobe;
            
            o.Albedo = c.rgb;
            o.Normal = UnpackNormal (tex2D (_NormalMap, IN.uv_NormalMap));
            e = IF(isOSC() == 1,lerp(half4(-_CurveMod,-_CurveMod,-_CurveMod,1), e, pow(GetOSCIntensity(dmx, 1.0), 1.0)), e);
            e = clamp(e, half4(0,0,0,1), half4(_FixtureMaxIntensity*2,_FixtureMaxIntensity*2,_FixtureMaxIntensity*2,1));
            e *= tex2D(_EmissionMask, IN.uv_MainTex).r;
            e*= _FixutreIntensityMultiplier;
            o.Emission = ((e.rgb * _FixtureMaxIntensity) * getGlobalIntensity()) * getFinalIntensity();
            o.Emission *= blindingEffect;
            o.Emission = o.Emission * _UniversalIntensity;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
     CustomEditor "VRSLInspector"
    FallBack "Diffuse"
   
}
