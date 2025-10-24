Shader "VRSL/Wash Mover/Volumetric"
{
	Properties
	{
		//[Header (INSTANCED PROPERITES)]
		 [HideInInspector]_DMXChannel ("Starting DMX Channel", Int) = 0
		 [HideInInspector][Toggle] _NineUniverseMode ("Extended Universe Mode", Int) = 0
		 [HideInInspector][Toggle] _PanInvert ("Invert Mover Pan", Int) = 0
		 [HideInInspector][Toggle] _TiltInvert ("Invert Mover Tilt", Int) = 0

		 //[HideInInspector]_FinalStrobeFreq ("Final Strobe Frequency", Float) = 0
		 //[HideInInspector]_NewTimer("New Timer From Udon For Strobe", Float) = 0
		 [Toggle] _EnableExtraChannels ("Enable Extra Channels", Int) = 0
		 [Toggle] _EnableCompatibilityMode ("Enable Compatibility Mode", Int) = 0
		 [Toggle] _EnableVerticalMode ("Enable Vertical Mode", Int) = 0
		 [Toggle] _EnableStrobe ("Enable Strobe", Int) = 0
		 [Toggle] _EnableDMX ("Enable Stream DMX/DMX Control", Int) = 0
		 [HideInInspector]_FixtureBaseRotationY("Mover Pan Offset (Blue + Green)", Range(-540,540)) = 0
		 [HideInInspector]_FixtureRotationX("Mover Tilt Offset (Blue)", Range(-180,180)) = 0
		 [HideInInspector]_ProjectionSelection ("GOBO Selection", Range(0,6)) = 0
		 [HideInInspector]_StrobeFreq("Strobe Frequency", Range(0,25)) = 1
		 [HideInInspector][Toggle] _EnableSpin("Enable Auto Spinning", Float) = 0
		 [Toggle] _LegacyGoboRange ("Enable Legacy GOBO Range", Int) = 0
		 _BlindingStrength("Blinding Strength", Range(0,1)) = 1

		//[Header (BASIC CONTROLS)]
		_FinalIntensity("Final Intensity", Range(0,1)) = 1
		_GlobalIntensity("Global Intensity", Range(0,1)) = 1
		_GlobalIntensityBlend("Global Intensity Blend", Range(0,1)) = 1
		_UniversalIntensity ("Universal Intensity", Range (0,1)) = 1
		[HDR]_Emission("Light Color Tint", Color) = (1,1,1,1)
		_Saturation("Final Saturation", Range(0,1)) = 1
		_SaturationLength("Final Saturation Length", Range(1,20)) = 9
		_LensMaxBrightness("Lens Max Brightness", Range(0.01, 10)) = 5
		_ConeWidth("Cone Width", Range(0,5.5)) = 0
		_ConeLength("Cone Length", Range(1,10)) = 1
		_MaxConeLength("Max Cone Length", Range(1,10)) = 1
		_ConeSync ("Cone Scale Sync", Range(0,1)) = 0.2
		// _BlockLengthX("DMX Block Base Distance X", Float) = 0.019231
		// _BlockLengthY("DMX Block Base Distance Y", Float) = 0
		//[Enum(Off,0,One,1)] _BlendSrc ("Source Blend mode", Float) = 1
		[Enum(Off,0,One,1)] _BlendDst ("Destination Blend mode", Float) = 1
		[Enum(UnityEngine.Rendering.BlendOp)] _BlendOp ("Blend Operation", Float) = 0
		[Enum(HQTransparent,0,Transparent,1,AlphaToCoverage,2)] _RenderMode ("Render Mode", Int) = 1
		[Enum(Off,0,On,1)] _ZWrite ("Z Write", Int) = 0
		[Enum(Off,0,On,1)] _AlphaToCoverage ("Alpha To Coverage", Int) = 0
		//[Space(16)]

		//[Header(MOVER CONTROLS)]


		_FixtureRotationOrigin("Fixture Pivot Origin", Float) = (0, 0.014709, -1.02868, 0)
		[Toggle] _UseRawGrid("Use Raw Grid For Light Intensity And Color", Int) = 0
		// [NoScaleOffset] _Udon_DMXGridRenderTexture("DMX Grid Render Texture (RAW Unsmoothed)", 2D) = "white" {}
		// [NoScaleOffset] _Udon_DMXGridRenderTextureMovement("DMX Grid Render Texture (To Control Lights)", 2D) = "white" {}
		// [NoScaleOffset] _Udon_DMXGridStrobeTimer("DMX Grid Render Texture (For Strobe Timings", 2D) = "white" {}
		// [NoScaleOffset] _Udon_DMXGridSpinTimer("DMX Grid Render Texture (For GOBO Spin Timings", 2D) = "white" {}
		_MaxMinPanAngle("Max/Min Pan Angle (-x, x)", Float) = 180
		_MaxMinTiltAngle("Max/Min Tilt Angle (-y, y)", Float) = 180

		//[Header(VOLUMETRIC LIGHTING CONTROLS)]
		_LightMainTex ("Light Texture", 2D) = "white" {}
		_NoiseTex ("NoiseTex", 2D) = "white" {}
		_NoiseTexHigh ("NoiseTexHigh", 2D) = "white" {}
		_NoisePower("Noise Strength", Range(0, 1)) = 1
		_NoiseSeed ("Noise Seed", float) = 0
		[Toggle]_MAGIC_NOISE_ON_HIGH ("Toggle Magic Noise", Int) = 1
		[Toggle]_MAGIC_NOISE_ON_MED ("Toggle Magic Noise", Int) = 1
		[Toggle]_2D_NOISE_ON ("Toggle 2D Noise", Int) = 1
		_Noise2Stretch ("Outside Magic Noise Scale", Range(-10, 10)) = 1
		_Noise2StretchInside ("Inside Magic Noise Scale", Range(-10, 10)) = 1
		_Noise2X ("Magic Noise X Scroll", Range(-10, 10)) = 1
		_Noise2Y ("Magic Noise Y Scroll", Range(-10, 10)) = 1
		_Noise2Z ("Magic Noise Y Scroll", Range(-10, 10)) = 1
		_Noise2Power("Magic Noise Strength", Range(0, 1)) = 1


		_Noise2StretchDefault ("Outside Magic Noise Scale", Range(-10, 10)) = 1
		_Noise2StretchInsideDefault ("Inside Magic Noise Scale", Range(-10, 10)) = 1
		_Noise2XDefault ("Magic Noise X Scroll", Range(-10, 10)) = 1
		_Noise2YDefault ("Magic Noise Y Scroll", Range(-10, 10)) = 1
		_Noise2ZDefault ("Magic Noise Y Scroll", Range(-10, 10)) = 1
		_Noise2PowerDefault("Magic Noise Strength", Range(0, 1)) = 1

		_Noise2StretchPotato ("Outside Magic Noise Scale", Range(-10, 10)) = 1
		_Noise2StretchInsidePotato ("Inside Magic Noise Scale", Range(-10, 10)) = 1
		_Noise2XPotato ("Magic Noise X Scroll", Range(-10, 10)) = 1
		_Noise2YPotato ("Magic Noise Y Scroll", Range(-10, 10)) = 1
		_Noise2ZPotato ("Magic Noise Y Scroll", Range(-10, 10)) = 1
		_Noise2PowerPotato("Magic Noise Strength", Range(0, 1)) = 1



		//[NoScaleOffset]_InsideConeNormalMap("Inside Cone Normal Map", 2D) = "bump" {}

		_FixtureMaxIntensity ("Maximum Cone Intensity",Range (0,5)) = 1
		_PulseSpeed("Pulse Speed", Range(0,2)) = 0
		_FadeStrength("Edge Fade", Range(0.00001,20)) = 1
		_InnerFadeStrength("Inner Fade Strength", Range(0.00001,10)) = 0
		_InnerIntensityCurve("Inner Intensity Curve", Range(0.00001,20)) = 1
		_DistFade("Distance Fade", Range(0,1)) = 0.1
		_FadeAmt("Depth Blending", Range(0, 100)) = 0.1
		_BlindingAngleMod("Blinding Angle Modification", Range(0, 1)) = 1
		//_IntensityCutoff("Intensity Minimum Cut Off", Range (0, 1)) = 0.2
		[Toggle]_GoboBeamSplitEnable("Enable Splitting the beam on Gobos 2-6", Int) = 0
		_StripeSplit ("Stripe Split GOBO2", Range(0, 30)) = 0
		_StripeSplitStrength ("Stripe Split Strength G0B02", Range(0, 1)) = 0
		_StripeSplit2 ("Stripe Split GOBO3", Range(0, 30)) = 0
		_StripeSplitStrength2 ("Stripe Split Strength G0B03", Range(0, 1)) = 0
		_StripeSplit3 ("Stripe Split GOBO4", Range(0, 30)) = 0
		_StripeSplitStrength3 ("Stripe Split Strength G0B04", Range(0, 1)) = 0
		_StripeSplit4 ("Stripe Split GOBO5", Range(0, 30)) = 0
		_StripeSplitStrength4 ("Stripe Split Strength G0B05", Range(0, 1)) = 0
		_StripeSplit5 ("Stripe Split GOBO6", Range(0, 30)) = 0
		_StripeSplitStrength5 ("Stripe Split Strength G0B06", Range(0, 1)) = 0
		_StripeSplit6 ("Stripe Split GOBO7", Range(0, 30)) = 0
		_StripeSplitStrength6 ("Stripe Split Strength G0B07", Range(0, 1)) = 0
		_StripeSplit7 ("Stripe Split GOBO8", Range(0, 30)) = 0
		_StripeSplitStrength7 ("Stripe Split Strength G0B08", Range(0, 1)) = 0

		_MinimumBeamRadius ("Minimum Beam Radius", Range(0.001,1)) = 1


		[Toggle] _EnableSpin("Enable Auto Spinning", Float) = 0
		_SpinSpeed ("Auto Spin Speed", Range(0, 10)) = 0
		_GradientMod ("Gradient Modifier", Range(1, 4)) = 2.25
		_GradientModGOBO ("Gradient Modifier GOBO", Range(1, 4)) = 2.25

		[Toggle]_UseDepthLight("Toggle The Requirement of the depth light to function.", Int) = 1
		[Toggle]_PotatoMode("Reduces the overhead on the fragment shader by removing both noise components to extra texture sampling", Int) = 0
		[Toggle]_HQMode("A higher quality volumetric mode (Experimental)", Int) = 0



		//[Space(16)]



		//[Toggle] _UseWorldNorm("Use World Normal vs View Normal", Float) = 0
		//[KeywordEnum(None, UseDNTexture)] _DNEnabler ("Enable Depth Normal Texture", Float) = 0

	}
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent+2" "IgnoreProjector"="True" "RenderType" = "Transparent" "RenderingPipeline" = "UniversalPipeline"
        }
        //Volumetric Pass

        Pass
        {
            AlphaToMask [_AlphaToCoverage]
            Blend One [_BlendDst]
            Cull Off
            ZWrite [_ZWrite]
            Lighting Off
            Tags
            {
                "LightMode" = "UniversalForward"
            }
            Stencil
            {
                Ref 142
                Comp NotEqual
                Pass Keep
            }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            //#pragma multi_compile_fog
            #pragma multi_compile_instancing
            #pragma instancing_options assumeuniformscaling
            #pragma multi_compile_local _ _MAGIC_NOISE_ON_HIGH
            #pragma multi_compile_local _ _MAGIC_NOISE_ON_MED
            #pragma multi_compile_local _ _USE_DEPTH_LIGHT
            #pragma multi_compile_local _ _POTATO_MODE_ON
            #pragma multi_compile_local _ _HQ_MODE
            #pragma multi_compile_local _ _2D_NOISE_ON
            #pragma multi_compile_local _ _ALPHATEST_ON
            #define VOLUMETRIC_YES //To identify the pass in the vert/frag
            #define WASH
            #define VRSL_DMX
            #include "UnityCG.cginc"
            #include "../Shared/VRSL-Defines.cginc" //Property Defines are here
            float3 thisIsAChange;
            #include "../Shared/VRSL-DMXFunctions.cginc" //Custom Functions

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
                //float4 color : TEXCOORD3;
                float3 currentForward : TEXCOORD3;
                float2 camAngleCamfade : TEXCOORD5;
                float4 screenPos : TEXCOORD6;
                float4 pos : SV_POSITION;
                //float3 objPos : TEXCOORD7;
                centroid float3 objNormal : TEXCOORD8;
                float2 stripeInfo : TEXCOORD9;
                float coneWidth : TEXCOORD10;
                //float2 uvClone : TEXCOORD10;
                //float3 norm : TEXCOORD11;
                float2 uv2 : TEXCOORD13;
                float4 worldDirection : TEXCOORD14;
                float4 intensityStrobeGOBOSpinSpeed : TEXCOORD15;
                float4 rgbColor : TEXCOORD16;

                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            #include "VRSL-StandardMover-VolumetricFrag.cginc" //Fragment Shader is here
            #include "VRSL-StandardMover-Vertex.cginc" //Vertex Shader is here
            ENDCG
        }
    }

		SubShader
	{
		
		Tags{ "Queue" = "Transparent+2" "IgnoreProjector"="True" "RenderType" = "Transparent" }
		//Volumetric Pass

	Pass
		{
			AlphaToMask [_AlphaToCoverage]
			Blend One [_BlendDst]
			Cull Off
			ZWrite [_ZWrite]
			Lighting Off
			Tags{ "LightMode" = "Always" }
			Stencil
			{
				Ref 142
				Comp NotEqual
				Pass Keep
			}
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma multi_compile_fog
			#pragma multi_compile_instancing
			#pragma instancing_options assumeuniformscaling
			#pragma multi_compile_local _ _MAGIC_NOISE_ON_HIGH
			#pragma multi_compile_local _ _MAGIC_NOISE_ON_MED
			#pragma multi_compile_local _ _USE_DEPTH_LIGHT
			#pragma multi_compile_local _ _POTATO_MODE_ON
			#pragma multi_compile_local _ _HQ_MODE
			#pragma multi_compile_local _ _2D_NOISE_ON
			#pragma multi_compile_local _ _ALPHATEST_ON
			#define VOLUMETRIC_YES //To identify the pass in the vert/frag
			#define WASH
			#define VRSL_DMX
			#include "UnityCG.cginc"
			#include "../Shared/VRSL-Defines.cginc" //Property Defines are here
			float3 thisIsAChange;
			#include "../Shared/VRSL-DMXFunctions.cginc" //Custom Functions

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
				//float4 color : TEXCOORD3;
				float3 currentForward : TEXCOORD3;
				float2 camAngleCamfade : TEXCOORD5;
				float4 screenPos : TEXCOORD6;
				float4 pos : SV_POSITION;
				//float3 objPos : TEXCOORD7;
				centroid float3 objNormal : TEXCOORD8;
				float2 stripeInfo : TEXCOORD9;
				float coneWidth : TEXCOORD10;
				//float2 uvClone : TEXCOORD10;
				//float3 norm : TEXCOORD11;
				float2 uv2 : TEXCOORD13;
				float4 worldDirection : TEXCOORD14;
				float4 intensityStrobeGOBOSpinSpeed : TEXCOORD15;
				float4 rgbColor : TEXCOORD16;

				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO 
			};
			#include "VRSL-StandardMover-VolumetricFrag.cginc" //Fragment Shader is here
			#include "VRSL-StandardMover-Vertex.cginc" //Vertex Shader is here

			
			ENDCG
		}

	}
	CustomEditor "VRSLInspector"
	//CustomEditor "MoverProjectionLightCustomGUI"
}
