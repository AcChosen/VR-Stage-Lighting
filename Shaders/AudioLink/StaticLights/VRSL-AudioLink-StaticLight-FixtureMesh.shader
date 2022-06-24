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


        [Toggle] _EnableOSC ("Enable Stream OSC/DMX Control", Int) = 0
        _FixutreIntensityMultiplier ("Intensity Multipler (For Bloom Scaling)", Range(1,15)) = 1
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _NormalMap ("Normal Map", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

        [Header(Audio Section)]
         [Toggle]_EnableAudioLink("Enable Audio Link", Float) = 0
         _Band("Band", Float) = 0
         _BandMultiplier("Band Multiplier", Range(1, 15)) = 1
         _Delay("Delay", Float) = 0
         _NumBands("Num Bands", Float) = 4
         _AudioSpectrum("AudioSpectrum", 2D) = "black" {}
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
        #define STATIC_FIXTURE
        struct Input
        {
            float2 uv_MainTex;
            float2 uv_NormalMap;
        };
         #include "../Shared/VRSL-AudioLink-Defines.cginc" //Property Defines are here
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
