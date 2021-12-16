Shader "VRSL/Non-DMX/Standard Mover/Volumetric"
{
	Properties
	{
		[Header (INSTANCED PROPERITES)]

		 [HideInInspector][Toggle] _PanInvert ("Invert Mover Pan", Int) = 0
		 [HideInInspector][Toggle] _TiltInvert ("Invert Mover Tilt", Int) = 0

		//Color Texture Sampling Properties
		 [Toggle] _EnableColorTextureSample ("Enable Color Texture Sampling", Int) = 0
		 _SamplingTexture ("Texture To Sample From for Color", 2D) = "white" {}
		 _TextureColorSampleX ("X coordinate to sample the texture from", Range(0,1)) = 0.5
		 _TextureColorSampleY ("Y coordinate to sample the texture from", Range(0,1)) = 0.5

		 //[HideInInspector]_FinalStrobeFreq ("Final Strobe Frequency", Float) = 0
		 //[HideInInspector]_NewTimer("New Timer From Udon For Strobe", Float) = 0

		 [HideInInspector]_FixtureBaseRotationY("Mover Pan Offset (Blue + Green)", Range(-540,540)) = 0
		 [HideInInspector]_FixtureRotationX("Mover Tilt Offset (Blue)", Range(-180,180)) = 0
		 [HideInInspector]_ProjectionSelection ("GOBO Selection", Range(0,6)) = 0
		 [HideInInspector][Toggle] _EnableSpin("Enable Auto Spinning", Float) = 0

		//[Header (BASIC CONTROLS)]
		_FinalIntensity("Final Intensity", Range(0,1)) = 1
		_GlobalIntensity("Global Intensity", Range(0,1)) = 1
		_UniversalIntensity ("Universal Intensity", Range (0,1)) = 1
		[HDR]_Emission("Light Color Tint", Color) = (1,1,1,1)
		_Saturation("Final Saturation", Range(0,1)) = 1
		_SaturationLength("Final Saturation Length", Range(0,0.2)) = 0.1
		_LensMaxBrightness("Lens Max Brightness", Range(0.01, 10)) = 5
		_ConeWidth("Cone Width", Range(0,5.5)) = 0
		_ConeLength("Cone Length", Range(1,10)) = 1
		_MaxConeLength("Max Cone Length", Range(1,10)) = 1
		_ConeSync ("Cone Scale Sync", Range(0,1)) = 0.2
		// _BlockLengthX("OSC Block Base Distance X", Float) = 0.019231
		// _BlockLengthY("OSC Block Base Distance Y", Float) = 0
		[Enum(UnityEngine.Rendering.BlendMode)] _BlendSrc ("Source Blend mode", Float) = 2
		[Enum(UnityEngine.Rendering.BlendMode)] _BlendDst ("Destination Blend mode", Float) = 1
		[Enum(UnityEngine.Rendering.BlendOp)] _BlendOp ("Blend Operation", Float) = 0
		//[Space(16)]

		//[Header(MOVER CONTROLS)]


		_FixtureRotationOrigin("Fixture Pivot Origin", Float) = (0, 0.014709, -1.02868, 0)
		_MaxMinPanAngle("Max/Min Pan Angle (-x, x)", Float) = 180
		_MaxMinTiltAngle("Max/Min Tilt Angle (-y, y)", Float) = 180

		//[Header(VOLUMETRIC LIGHTING CONTROLS)]
		_LightMainTex ("Light Texture", 2D) = "white" {}
		_NoiseTex ("NoiseTex", 2D) = "white" {}
		_NoisePower("Noise Strength", Range(0, 1)) = 1
		_NoiseSeed ("Noise Seed", float) = 0
		[NoScaleOffset]_InsideConeNormalMap("Inside Cone Normal Map", 2D) = "bump" {}

		_FixtureMaxIntensity ("Maximum Cone Intensity",Range (0,0.5)) = 0.5
		_PulseSpeed("Pulse Speed", Range(0,2)) = 0
		_FadeStrength("Edge Fade", Range(1,10)) = 1
		_InnerFadeStrength("Inner Fade Strength", Range(-5,5)) = 0
		_InnerIntensityCurve("Inner Intensity Curve", Range(-2, 2)) = 1
		_DistFade("Distance Fade", Range(0,3)) = 0.7
		_FadeAmt("Depth Blending", Range(0, 1)) = 0.1
		//_IntensityCutoff("Intensity Minimum Cut Off", Range (0, 1)) = 0.2
		[Toggle]_GoboBeamSplitEnable("Enable Splitting the beam on Gobos 2-6", Int) = 0
		_StripeSplit ("Stripe Split", Range(0, 30)) = 0
		_StripeSplitStrength ("Stripe Split Strength", Range(0, 1)) = 0
		[Toggle] _EnableSpin("Enable Auto Spinning", Float) = 0
		_SpinSpeed ("Auto Spin Speed", Range(0, 10)) = 0
		//_FixtureLensOrigin("Center Of Fixture Lens (For Blinding Effect)", Float) = (0,-0.068819, 0.123191, 0)



		//[Space(16)]



		//[Toggle] _UseWorldNorm("Use World Normal vs View Normal", Float) = 0
		//[KeywordEnum(None, UseDNTexture)] _DNEnabler ("Enable Depth Normal Texture", Float) = 0

	}
		SubShader
	{
		
		Tags{ "Queue" = "Transparent+2" "IgnoreProjector"="True" "RenderType" = "Transparent" }
		//Volumetric Pass

	Pass
		{
			Blend One One
			Cull Back
			ZWrite Off
			Lighting Off
			Tags{ "LightMode" = "Always" }
			Stencil
			{
				Ref 148
				Comp NotEqual
				Pass Keep
			}
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma multi_compile_fog
			#pragma multi_compile_instancing
			#pragma instancing_options assumeuniformscaling
			#define VOLUMETRIC_YES //To identify the pass in the vert/frag
			#define RAW

			#include "UnityCG.cginc"
			#include "../Shared/VRSLNonDMX-Defines.cginc" //Property Defines are here
			float3 thisIsAChange;
			#include "../Shared/VRSLNonDMX-Functions.cginc" //Custom Functions

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 color : COLOR;
				float3 normal : NORMAL;
				float3 tangent : TANGENT;
				float2 uv2 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				centroid float2 uv : TEXCOORD0;
				float blindingEffect : TEXCOORD1;
				float4 worldPos : TEXCOORD2;
				float4 color : TEXCOORD3;
				float2 globalFinalIntensity : TEXCOORD4;
				float3 viewDir : TEXCOORD5;
				float4 screenPos : TEXCOORD6;
				float4 pos : SV_POSITION;
				float3 objPos : TEXCOORD7;
				float3 objNormal : TEXCOORD8;
				float3 bitan : TEXCOORD9;
				float3 tan : TEXCOORD10;
				float3 norm : TEXCOORD11;
				float2 uv2 : TEXCOORD13;
				float4 worldDirection : TEXCOORD14;
				float4 emissionColor : TEXCOORD15;

				//float3 intensityStrobeGOBOSpinSpeed : TEXCOORD15;
				//float4 rgbColor : TEXCOORD16;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			#include "VRSLNonDMX-StandardMover-VolumetricFrag.cginc" //Fragment Shader is here
			#include "VRSLNonDMX-StandardMover-Vertex.cginc" //Vertex Shader is here

			
			ENDCG
		}

	}
	CustomEditor "VRSLInspector"
	//CustomEditor "MoverProjectionLightCustomGUI"
}
