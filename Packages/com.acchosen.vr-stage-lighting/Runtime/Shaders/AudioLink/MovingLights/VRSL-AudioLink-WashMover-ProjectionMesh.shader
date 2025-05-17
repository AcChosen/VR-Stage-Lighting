Shader "VRSL/AudioLink/Wash Mover/Projection"
{

	Properties
	{
		//[Header (INSTANCED PROPERITES)]
		 [HideInInspector][Toggle] _PanInvert ("Invert Mover Pan", Int) = 0
		 [HideInInspector][Toggle] _TiltInvert ("Invert Mover Tilt", Int) = 0
		_RenderTextureMultiplier("Render Texture Multiplier", Range(1,10)) = 1
		 //[HideInInspector]_FinalStrobeFreq ("Final Strobe Frequency", Float) = 0
		 //[HideInInspector]_NewTimer("New Timer From Udon For Strobe", Float) = 0
		 [HideInInspector]_FixtureBaseRotationY("Mover Pan Offset (Blue + Green)", Range(-540,540)) = 0
		 [HideInInspector]_FixtureRotationX("Mover Tilt Offset (Blue)", Range(-180,180)) = 0
		 [HideInInspector]_ProjectionSelection ("GOBO Selection", Range(1,6)) = 1
		 //[HideInInspector]_StrobeFreq("Strobe Frequency", Range(0,25)) = 1
		 [HideInInspector][Toggle] _EnableSpin("Enable Auto Spinning", Float) = 0

		[Header(Audio Section)]
         [Toggle]_EnableAudioLink("Enable Audio Link", Float) = 0
		 [Toggle] _EnableColorChord ("Enable Color Chord Tinting", Int) = 0
         _Band("Band", Float) = 0
         _BandMultiplier("Band Multiplier", Range(1, 15)) = 1
         _Delay("Delay", Float) = 0
         _NumBands("Num Bands", Float) = 4
         _AudioSpectrum("AudioSpectrum", 2D) = "black" {}
		 [Toggle]_UseTraditionalSampling("Use Traditional Texture Sampling", Int) = 0

		//[Header (BASIC CONTROLS)]
		_FinalIntensity("Final Intensity", Range(0,1)) = 1
		_GlobalIntensity("Global Intensity", Range(0,1)) = 1
		_GlobalIntensityBlend("Global Intensity Blend", Range(0,1)) = 1
		_UniversalIntensity ("Universal Intensity", Range (0,1)) = 1
		[HDR]_Emission("Light Color Tint", Color) = (1,1,1,1)
		[HDR]_StaticEmission("Static Light Color Tint", Color) = (1,1,1,1)
		[Toggle] _EnableStaticEmissionColor("Enable Static Emission Color", Int) = 0
		_Saturation("Final Saturation", Range(0,1)) = 1
		_SaturationLength("Final Saturation Length", Range(0,0.2)) = 0.1
		_LensMaxBrightness("Lens Max Brightness", Range(0.01, 10)) = 5
		_ConeWidth("Cone Width", Range(0,5.5)) = 0
		_ConeLength("Cone Length", Range(1,10)) = 1
		_ConeSync ("Cone Scale Sync", Range(1,2)) = 1
		// _BlockLengthX("OSC Block Base Distance X", Float) = 0.019231
		// _BlockLengthY("OSC Block Base Distance Y", Float) = 0

		[Enum(UnityEngine.Rendering.BlendMode)] _BlendSrc ("Source Blend mode", Float) = 2
		//[Enum(UnityEngine.Rendering.BlendMode)] _BlendDst ("Destination Blend mode", Float) = 1
		[Enum(UnityEngine.Rendering.BlendOp)] _BlendOp ("Blend Operation", Float) = 0
		//[Space(16)]

		//Color Texture Sampling Properties
		 [Toggle] _EnableColorTextureSample ("Enable Color Texture Sampling", Int) = 0
		 _SamplingTexture ("Texture To Sample From for Color", 2D) = "white" {}
		 _TextureColorSampleX ("X coordinate to sample the texture from", Range(0,1)) = 0.5
		 _TextureColorSampleY ("Y coordinate to sample the texture from", Range(0,1)) = 0.5
		//[Header(MOVER CONTROLS)]


		_FixtureRotationOrigin("Fixture Pivot Origin", Float) = (0, 0.014709, -1.02868, 0)
		// [Toggle] _UseRawGrid("Use Raw Grid For Light Intensity And Color", Int) = 0
		// [NoScaleOffset] _OSCGridRenderTextureRAW("OSC Grid Render Texture (RAW Unsmoothed)", 2D) = "white" {}
		// [NoScaleOffset] _OSCGridRenderTexture("OSC Grid Render Texture (To Control Lights)", 2D) = "white" {}
		// [NoScaleOffset] _OSCGridStrobeTimer ("OSC Grid Render Texture (For Strobe Timings", 2D) = "white" {}
		//[Toggle] _EnableAudioReact ("Enable AudioLink Audio React", Int) = 0
		//[NoScaleOffset] _AudioSpectrum("AudioSpectrum", 2D) = "black" {}
		_MaxMinPanAngle("Max/Min Pan Angle (-x, x)", Float) = 180
		_MaxMinTiltAngle("Max/Min Tilt Angle (-y, y)", Float) = 180

		_FixtureMaxIntensity ("Maximum Cone Intensity",Range (0,0.5)) = 0.5

		// [Header(Divide)]
		// _Divide ("Divide", Range(0, 30)) = 0
		// _DividePower ("Divide Strength", Range(0, 1)) = 0

		//[Space(24)]
		//[Header(PROJECTION GOBO SETTINGS)]
		_ProjectionRotation("Static Projection UV Rotation", Range(-180, 180)) = 0
		_SpinSpeed ("Auto Spin Speed", Range(0, 10)) = 0
		_ProjectionIntensity ("Projection Intensity", Range(0,20)) = 0
		_ProjectionFade("Projection Edge Fade", Range(0,10)) = 0
		_ProjectionFadeCurve("Projection Edge Fade Harshness", Range(0, 10)) = 1
		_ProjectionDistanceFallOff("Projection Distance Fallof Strength", Range(0.001,0.5)) = 0.05
		_ProjectionRange ("Projection Drawing Range", Range(0,10)) = 0
		_ProjectionRangeOrigin ("Projection Drawing Range Scale Origin", Float) = (0, -0.07535, 0.12387, 0)
		_ProjectionShadowHarshness("Projection Shadow Harshness", Range(0,1)) = 0


		[NoScaleOffset] _ProjectionMainTex ("Projection Texture GOBO 1", 2D) = "white"{}
		_ProjectionUVMod ("Projection UV Scale Modifier ", Range(0.01,10)) = 0
		//[NoScaleOffset] _ProjectionTex2 ("Projection Texture GOBO 2", 2D) = "white"{}
		_ProjectionUVMod2 ("Projection UV Scale Modifier ", Range(0.01,10)) = 0
		//[NoScaleOffset] _ProjectionTex3 ("Projection Texture GOBO 3", 2D) = "white"{}
		_ProjectionUVMod3 ("Projection UV Scale Modifier ", Range(0.01,10)) = 0
		//[NoScaleOffset] _ProjectionTex4 ("Projection Texture GOBO 4", 2D) = "white"{}
		_ProjectionUVMod4 ("Projection UV Scale Modifier ", Range(0.01,10)) = 0
		//[NoScaleOffset] _ProjectionTex5 ("Projection Texture GOBO 5", 2D) = "white"{}
		_ProjectionUVMod5 ("Projection UV Scale Modifier ", Range(0.01,10)) = 0
		// [NoScaleOffset] _ProjectionTex6 ("Projection Texture GOBO 6", 2D) = "white"{}
		_ProjectionUVMod6 ("Projection UV Scale Modifier ", Range(0.01,10)) = 0	
		_ProjectionUVMod7 ("Projection UV Scale Modifier ", Range(0.01,10)) = 0	
		_ProjectionUVMod8 ("Projection UV Scale Modifier ", Range(0.01,10)) = 0	
		//[Space(8)]	

		_RedMultiplier ("Red Channel Multiplier", Range(0, 5)) = 1
		_GreenMultiplier ("Green Channel Multiplier", Range(0, 5)) = 1
		_BlueMultiplier ("Blue Channel Multiplier", Range(0,5)) = 1

		_ProjectionCutoff("Projection Cutoff Point", Range(0,1)) = 0.25
		_ProjectionOriginCutoff("Projection Origin Cutoff Point", Range(0,3)) = 0.25


		[Toggle] _EnableThemeColorSampling ("Enable Theme Color Sampling", Int) = 0
		 _ThemeColorTarget ("Choose Theme Color", Int) = 0

		 		[Enum(Transparent,1,AlphaToCoverage,2)] _RenderMode ("Render Mode", Int) = 1
        [Enum(Off,0,On,1)] _ZWrite ("Z Write", Int) = 0
		[Enum(Off,0,On,1)] _AlphaToCoverage ("Alpha To Coverage", Int) = 0
        [Enum(Off,0,One,1)] _BlendDst ("Destination Blend mode", Float) = 1
		[Enum(UnityEngine.Rendering.BlendOp)] _BlendOp ("Blend Operation", Float) = 0
        _ClippingThreshold ("Clipping Threshold", Range (0,1)) = 0.5
		_MinimumBeamRadius ("Minimum Beam Radius", Range(0.001,1)) = 1

		[Enum(Off,0,On,1)] _MultiSampleDepth ("Multi Sample Depth", Int) = 1


	}
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent+1" "IgnoreProjector"="True" "RenderType" = "Transparent" "RenderingPipeline" = "UniversalPipeline"
        }

        Pass
        {
            AlphaToMask [_AlphaToCoverage]
            Cull Front
            Ztest GEqual
            ZWrite Off
            Blend DstColor [_BlendDst]
            BlendOp Add
            Offset -1, -1
            Lighting Off

            Stencil
            {
                Ref 142
                Comp NotEqual
                Pass Keep
            }
            Tags
            {
                "LightMode" = "UniversalForward"
            }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_local _ _ALPHATEST_ON
            #pragma shader_feature_local _MULTISAMPLEDEPTH
            //#pragma multi_compile_fog
            #pragma multi_compile_instancing
            #pragma instancing_options assumeuniformscaling
            //#pragma multi_compile _DNENABLER_NONE _DNENABLER_USEDNTEXTURE
            #define PROJECTION_YES //To identify the pass in the vert/frag shaders
            #define PROJECTION_MOVER
            #define WASH
            #define VRSL_AUDIOLINK

            #include "UnityCG.cginc"
            #include "Packages/com.acchosen.vr-stage-lighting/Runtime/Shaders/Shared/VRSL-Defines.cginc" //Property Defines are here


            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 texcoord : TEXCOORD1;
                float4 color : COLOR;
                float3 normal : NORMAL;
                float3 tangent : TANGENT;
                float4 projectionorigin : TEXCOORD2;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 audioGlobalFinalConeIntensity : TEXCOORD1;
                float3 ray : TEXCOORD2;
                float4 screenPos : TEXCOORD4;
                float4 color : COLOR;
                float3 normal : TEXCOORD3;
                //float2 sector: TEXCOORD10;
                float4 projectionorigin : TEXCOORD5;
                float4 worldDirection : TEXCOORD6;
                float4 worldPos : TEXCOORD7;
                float3 viewDir : TEXCOORD8;
                float4 emissionColor : TEXCOORD9;
                //float3 intensityStrobeWidth : TEXCOORD9;
                //float4 goboPlusSpinPanTilt : TEXCOORD11;
                //float4 rgbColor : TEXCOORD12;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            #include "../Shared/VRSL-AudioLink-Functions.cginc" //Custom Functions
            #include "Packages/com.acchosen.vr-stage-lighting/Runtime/Shaders/MovingLights/VRSL-StandardMover-ProjectionFrag.cginc" //Fragment Shader is here

            #include "Packages/com.acchosen.vr-stage-lighting/Runtime/Shaders/MovingLights/VRSL-StandardMover-Vertex.cginc" //Vertex Shader is here

            ENDCG
        }

        // Used for handling Depth Buffer (DBuffer) and Depth Priming
        UsePass "Universal Render Pipeline/Lit/DepthOnly"
        UsePass "Universal Render Pipeline/Lit/DepthNormals"
    }

		SubShader
	{
		
		Tags{ "Queue" = "Transparent+1" "IgnoreProjector"="True" "RenderType" = "Transparent" }

		Pass
         {
			AlphaToMask [_AlphaToCoverage] 
            Cull Front
            Ztest GEqual
            ZWrite Off
            Blend  DstColor [_BlendDst]
            BlendOp Add	
			Offset -1, -1
			Lighting Off

			Stencil
			{
				Ref 142
				Comp NotEqual
				Pass Keep
			}
			Tags{ "LightMode" = "Always" }
            CGPROGRAM
			
            #pragma vertex vert
            #pragma fragment frag
			#pragma multi_compile_local _ _ALPHATEST_ON
			#pragma shader_feature_local _MULTISAMPLEDEPTH
			//#pragma multi_compile_fog
			#pragma multi_compile_instancing
			#pragma instancing_options assumeuniformscaling
			//#pragma multi_compile _DNENABLER_NONE _DNENABLER_USEDNTEXTURE
			#define PROJECTION_YES //To identify the pass in the vert/frag shaders
			#define PROJECTION_MOVER
			#define WASH
			#define VRSL_AUDIOLINK

            #include "UnityCG.cginc"
			#include "Packages/com.acchosen.vr-stage-lighting/Runtime/Shaders/Shared/VRSL-Defines.cginc" //Property Defines are here
			

             struct appdata
             {
                 float4 vertex : POSITION;
                 float2 uv : TEXCOORD0;
                 float3 texcoord : TEXCOORD1;
				 float4 color : COLOR;
				 float3 normal : NORMAL;
				 float3 tangent : TANGENT;
				 float4 projectionorigin : TEXCOORD2;
				 UNITY_VERTEX_INPUT_INSTANCE_ID
             };
             struct v2f
             {
                 float4 pos : SV_POSITION;
                 float2 uv : TEXCOORD0;
				 float4 audioGlobalFinalConeIntensity : TEXCOORD1;
                 float3 ray : TEXCOORD2;
                 float4 screenPos : TEXCOORD4;
				 float4 color : COLOR;
				 float3 normal : TEXCOORD3;	
				 //float2 sector: TEXCOORD10;
				 float4 projectionorigin : TEXCOORD5;
				 float4 worldDirection : TEXCOORD6;
				 float4 worldPos : TEXCOORD7;
				 float3 viewDir : TEXCOORD8;
				 float4 emissionColor : TEXCOORD9;
				 //float3 intensityStrobeWidth : TEXCOORD9;
				 //float4 goboPlusSpinPanTilt : TEXCOORD11;
				 //float4 rgbColor : TEXCOORD12;
				 UNITY_VERTEX_INPUT_INSTANCE_ID
				 UNITY_VERTEX_OUTPUT_STEREO
             };
			#include "../Shared/VRSL-AudioLink-Functions.cginc" //Custom Functions
			#include "Packages/com.acchosen.vr-stage-lighting/Runtime/Shaders/MovingLights/VRSL-StandardMover-ProjectionFrag.cginc" //Fragment Shader is here
			#include "Packages/com.acchosen.vr-stage-lighting/Runtime/Shaders/MovingLights/VRSL-StandardMover-Vertex.cginc" //Vertex Shader is here

			 ENDCG

		 }


	}
	CustomEditor "VRSLInspector"
	//CustomEditor "MoverProjectionLightCustomGUI"
	//CustomEditor "MoverProjectionLightCustomGUI"
}
