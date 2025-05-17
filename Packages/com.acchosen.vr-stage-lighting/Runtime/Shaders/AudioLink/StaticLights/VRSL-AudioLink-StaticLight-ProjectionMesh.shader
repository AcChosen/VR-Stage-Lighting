Shader "VRSL/AudioLink/Standard Static/Projection"
{
	
	Properties
	{
		
		//[Header (INSTANCED PROPERITES)]
		 //[Enum(UnityEngine.Rendering.BlendMode)] _BlendDst ("Destination Blend mode", Float) = 1
		// _BlockLengthX("OSC Block Base Distance X", Float) = 0.019231
		// _BlockLengthY("OSC Block Base Distance Y", Float) = 0

		[Header(Audio Section)]
         [Toggle]_EnableAudioLink("Enable Audio Link", Float) = 0
		 [Toggle] _EnableColorChord ("Enable Color Chord Tinting", Int) = 0
         _Band("Band", Float) = 0
         _BandMultiplier("Band Multiplier", Range(1, 15)) = 1
         _Delay("Delay", Float) = 0
         _NumBands("Num Bands", Float) = 4
         _AudioSpectrum("AudioSpectrum", 2D) = "black" {}

		//[Header(LIGHTING CONTROLS)]
		_GlobalIntensity("Global Intensity", Range(0,1)) = 1
		_GlobalIntensityBlend("Global Intensity Blend", Range(0,1)) = 1
		_FinalIntensity("Final Intensity", Range(0,1)) = 1
		_UniversalIntensity ("Universal Intensity", Range (0,1)) = 1
		[HDR]_Emission("Light Color Tint", Color) = (1,1,1,1)
		_FixtureMaxIntensity ("Maximum Light Intensity",Range (0,6)) = 1
		//[NoScaleOffset] _SceneAlbedo ("Scene Albedo Render Texture", 2D) = "white" {}
		_RenderTextureMultiplier("Render Texture Multiplier", Range(1,10)) = 1
		[Toggle]_UseTraditionalSampling("Use Traditional Texture Sampling", Int) = 0
		//Color Texture Sampling Properties
		 [Toggle] _EnableColorTextureSample ("Enable Color Texture Sampling", Int) = 0
		 _SamplingTexture ("Texture To Sample From for Color", 2D) = "white" {}
		 _TextureColorSampleX ("X coordinate to sample the texture from", Range(0,1)) = 0.5
		 _TextureColorSampleY ("Y coordinate to sample the texture from", Range(0,1)) = 0.5

		//[Header(PROJECTION SETTINGS)]

		[NoScaleOffset] _ProjectionMainTex ("Projection Texture GOBO 1", 2D) = "white"{}
		_ProjectionMaxIntensity ("Maximum Projection Intensity", Range (0,50)) = 1
		
		_XOffset ("Projection Offset X", Range(-6, 6)) = 0
		_YOffset ("Projection Offset Y", Range(-6, 6)) = 0
		
		
		_ConeWidth("Specular Strength or whatever", Range(0,5)) = 0
		_ProjectionRange ("Projection Drawing Range", Range(0,10)) = 0
		_ProjectionRangeOrigin ("Projection Drawing Range Scale Origin", Float) = (0, -0.07535, 0.12387, 0)

	//	[Space(12)]
		_ProjectionDistanceFallOff("Attenuation Constant", Range(0,1)) = 1
		_ProjectionUVMod ("Projection UV Scale Modifier ", Range(0,1)) = 0
		_Fade ("Light Range", Range(0, 25)) = 1
		_FeatherOffset ("Attenuation Quadratic", Range(0,1)) = 1
		//[Space(12)]

		[Toggle] _UseWorldNorm("Use World Normal vs View Normal", Float) = 0
		_ModX ("Projection UV X Stretch", Range(-2, 2)) = 1
		_ModY ("Projection UV Y Stretch", Range(-2, 2)) = 1
		_ProjectionRotation("Projection UV Rotation", Range(-180, 180)) = 0
		[Toggle] _EnableSpin("Enable Auto Spinning", Float) = 0
		_SpinSpeed ("Auto Spin Speed", Range(0, 10)) = 0

		//[Space(8)]	

		_RedMultiplier ("Red Channel Multiplier", Range(1, 5)) = 1
		_GreenMultiplier ("Green Channel Multiplier", Range(1, 5)) = 1
		_BlueMultiplier ("Blue Channel Multiplier", Range(1,5)) = 1

		



	// 	[Header(MAIN)]
	// [Enum(Unity Default, 0, Non Linear, 1)]_LightProbeMethod("Light Probe Sampling", Int) = 0
	// 	[Enum(UVs, 0, Triplanar World, 1, Triplanar Object, 2)]_TextureSampleMode("Texture Mode", Int) = 0
	// 	_TriplanarFalloff("Triplanar Blend", Range(0.5,1)) = 1
	// 	_MainTex("Main Texture", 2D) = "white" {}
	// _Color("Color", Color) = (1,1,1,1)

	// 	[Space(16)]
	// [Header(NORMALS)]
	// _BumpMap("Normal Map", 2D) = "bump" {}
	// _BumpScale("Normal Scale", Range(-1,1)) = 1

	// 	[Space(16)]
	// [Header(METALLIC)]
	// _MetallicGlossMap("Metallic Map", 2D) = "white" {}
	// _Metallic("Metallic", Range(0,1)) = 0
	// 	_Glossiness("Smoothness", Range(0,1)) = 0

	// 	[Space(16)]
	// [Header(LIGHTMAPPING HACKS)]
	// _SpecularLMOcclusion("Specular Occlusion", Range(0,1)) = 0
	// 	_SpecLMOcclusionAdjust("Spec Occlusion Sensitiviy", Range(0,1)) = 0.2
	// 	_LMStrength("Lightmap Strength", Range(0,1)) = 1
	// 	_RTLMStrength("Realtime Lightmap Strength", Range(0,1)) = 1

			[Toggle] _EnableThemeColorSampling ("Enable Theme Color Sampling", Int) = 0
		 _ThemeColorTarget ("Choose Theme Color", Int) = 0
		[Enum(Transparent,1,AlphaToCoverage,2)] _RenderMode ("Render Mode", Int) = 1
        [Enum(Off,0,On,1)] _ZWrite ("Z Write", Int) = 0
		[Enum(Off,0,On,1)] _AlphaToCoverage ("Alpha To Coverage", Int) = 0
        [Enum(UnityEngine.Rendering.BlendMode)] _BlendSrc ("Source Blend mode", Float) = 2
        [Enum(Off,0,One,1)] _BlendDst ("Destination Blend mode", Float) = 1
		[Enum(UnityEngine.Rendering.BlendOp)] _BlendOp ("Blend Operation", Float) = 0
        _ClippingThreshold ("Clipping Threshold", Range (0,1)) = 0.5
		_AlphaProjectionIntensity ("Alpha Projection Intesnity", Range (0,1)) = 0.5
		[Enum(13CH,0,5CH,1)] _ChannelMode ("Channel Mode", Int) = 0

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

            Tags
            {
                "ForceNoShadowCasting"="True" "IgnoreProjector"="True" "LightMode" = "UniversalForward"
            }
            Cull Front
            Ztest GEqual
            ZWrite Off
            Blend [_BlendSrc] [_BlendDst]
            BlendOp [_BlendOp]
            Lighting Off
            //SeparateSpecular Off

            Stencil
            {
                Ref 142
                Comp NotEqual
            }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #pragma multi_compile_local _ _ALPHATEST_ON
            #pragma shader_feature_local _MULTISAMPLEDEPTH
            #define PROJECTION_YES
            #define VRSL_AUDIOLINK

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 texcoord : TEXCOORD1;
                float4 color : COLOR;
                float3 normal : TEXCOORD3;
                float3 tangent : TANGENT;
                float4 projectionorigin : TEXCOORD2;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 ray : TEXCOORD2;
                float4 screenPos : TEXCOORD4;
                float4 color : COLOR;
                float3 normal : TEXCOORD3;
                float4 projectionorigin : TEXCOORD5;
                float4 worldDirection : TEXCOORD6;
                float4 worldPos : TEXCOORD7;
                float4 emissionColor : TEXCOORD8;
                float3 audioGlobalFinalIntensity: TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            #include "Packages/com.acchosen.vr-stage-lighting/Runtime/Shaders/Shared/VRSL-Defines.cginc"
            #include "../Shared/VRSL-AudioLink-Functions.cginc"
            #include "Packages/com.acchosen.vr-stage-lighting/Runtime/Shaders/StaticLights/VRSL-StaticLight-ProjectionFrag.cginc"

            #define IF(a, b, c) lerp(b, c, step((fixed) (a), 0));

            float4 CalculateProjectionScaleRange(appdata v, float4 input, float scalar)
            {
                float4 oldinput = input;
                float4x4 scaleMatrix = float4x4(scalar, 0, 0, 0,
                                               0, scalar, 0, 0,
                                               0, 0, scalar, 0,
                                               0, 0, 0, 1.0);
                float4 newOrigin = input.w * _ProjectionRangeOrigin;
                input.xyz = input.xyz - newOrigin;
                //Do stretch
                float4 newProjectionScale = mul(scaleMatrix, input);
                input.xyz = newProjectionScale;
                input.xyz = input.xyz + newOrigin;
                input.xyz = IF(v.color.g != 0, input.xyz, oldinput);
                return input;
            }

            inline float4 CalculateFrustumCorrection()
            {
                float x1 = -UNITY_MATRIX_P._31 / (UNITY_MATRIX_P._11 * UNITY_MATRIX_P._34);
                float x2 = -UNITY_MATRIX_P._32 / (UNITY_MATRIX_P._22 * UNITY_MATRIX_P._34);
                return float4(
                    x1, x2, 0,
                    UNITY_MATRIX_P._33 / UNITY_MATRIX_P._34 + x1 * UNITY_MATRIX_P._13 + x2 * UNITY_MATRIX_P._23);
            }

            //VERTEX SHADER
            v2f vert(appdata v)
            {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                o.audioGlobalFinalIntensity.x = GetAudioReactAmplitude();
                o.audioGlobalFinalIntensity.y = getGlobalIntensity();
                o.audioGlobalFinalIntensity.z = getFinalIntensity();
                o.emissionColor = getEmissionColor();
                v.vertex = CalculateProjectionScaleRange(v, v.vertex, _ProjectionRange);
                o.projectionorigin = CalculateProjectionScaleRange(v, _ProjectionRangeOrigin, _ProjectionRange);
                //move verts to clip space
                o.pos = UnityObjectToClipPos(v.vertex);

                //get screen space position of verts
                o.screenPos = ComputeScreenPos(o.pos);
                //Putting in the vertex position before the transformation seems to somewhat move the projection correctly, but is still incorrect...?
                o.ray = UnityObjectToViewPos(v.vertex).xyz;
                //invert z axis so that it projects from camera properly
                o.ray *= float3(1, 1, -1);
                //saving vertex color incase needing to perform rotation calculation in fragment shader
                o.color = v.color;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                //For Mirror Depth Correction
                o.worldDirection.xyz = o.worldPos.xyz - _WorldSpaceCameraPos;
                // pack correction factor into direction w component to save space
                o.worldDirection.w = dot(o.pos, CalculateFrustumCorrection());
                if (o.audioGlobalFinalIntensity.x <= 0.005 || o.audioGlobalFinalIntensity.y <= 0.005 || o.
                    audioGlobalFinalIntensity.z <= 0.005 ||
                    all(o.emissionColor.xyz <= float4(0.005, 0.005, 0.005, 1.0)))
                {
                    v.vertex = float4(0, 0, 0, 0);
                    o.pos = UnityObjectToClipPos(v.vertex);
                }
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                //UNITY_SETUP_INSTANCE_ID(i);
                return ProjectionFrag(i);
            }
            ENDCG

        }

        // Used for handling Depth Buffer (DBuffer) and Depth Priming
        UsePass "Universal Render Pipeline/Lit/DepthOnly"
        UsePass "Universal Render Pipeline/Lit/DepthNormals"
    }
		SubShader
	{
		//UNITY_REQUIRE_ADVANDED_BLEND(all_equations)
		Tags{ "Queue" = "Transparent+1" "IgnoreProjector"="True" "RenderType" = "Transparent" }
		Pass
         {

			Tags{ "ForceNoShadowCasting"="True" "IgnoreProjector"="True" "LightMode" = "Always"}
            Cull Front
            Ztest GEqual
            ZWrite Off
            Blend  [_BlendSrc] [_BlendDst]	
            BlendOp [_BlendOp]
            Lighting Off
		    //SeparateSpecular Off
			
			Stencil
			{
				Ref 142
				Comp NotEqual
			}
            CGPROGRAM
			
            #pragma vertex vert
            #pragma fragment frag
			#pragma multi_compile_fog
			#pragma multi_compile_instancing
			#pragma multi_compile_local _ _ALPHATEST_ON
			#pragma shader_feature_local _MULTISAMPLEDEPTH
			#define PROJECTION_YES
			#define VRSL_AUDIOLINK

            #include "UnityCG.cginc"

			

             struct appdata
             {
                 float4 vertex : POSITION;
                 float2 uv : TEXCOORD0;
                 float3 texcoord : TEXCOORD1;
				 float4 color : COLOR;
				 float3 normal : TEXCOORD3;
				 float3 tangent : TANGENT;
				 float4 projectionorigin : TEXCOORD2;
				 UNITY_VERTEX_INPUT_INSTANCE_ID
             };
             struct v2f
             {
                 float4 pos : SV_POSITION;
                 float2 uv : TEXCOORD0;
                 float3 ray : TEXCOORD2;
                 float4 screenPos : TEXCOORD4;
				 float4 color : COLOR;
				 float3 normal : TEXCOORD3;	
				 float4 projectionorigin : TEXCOORD5;
				 float4 worldDirection : TEXCOORD6;
				 float4 worldPos : TEXCOORD7;
				 float4 emissionColor : TEXCOORD8;
				 float3 audioGlobalFinalIntensity: TEXCOORD1;
				 UNITY_VERTEX_INPUT_INSTANCE_ID
             };
			#include "Packages/com.acchosen.vr-stage-lighting/Runtime/Shaders/Shared/VRSL-Defines.cginc"
			#include "../Shared/VRSL-AudioLink-Functions.cginc"
			#include "Packages/com.acchosen.vr-stage-lighting/Runtime/Shaders/StaticLights/VRSL-StaticLight-ProjectionFrag.cginc"

	#define IF(a, b, c) lerp(b, c, step((fixed) (a), 0));


	float4 CalculateProjectionScaleRange(appdata v, float4 input, float scalar)
	{
	float4 oldinput = input;
	float4x4 scaleMatrix = float4x4(scalar, 0, 0, 0,
	0, scalar, 0, 0,
	0, 0, scalar, 0,
	0, 0, 0, 1.0);
	float4 newOrigin = input.w * _ProjectionRangeOrigin;
	input.xyz = input.xyz - newOrigin;
	//Do stretch
	float4 newProjectionScale = mul(scaleMatrix, input);
	input.xyz = newProjectionScale;
	input.xyz = input.xyz + newOrigin;
	input.xyz = IF(v.color.g != 0, input.xyz, oldinput);
	return input;
	}

	
	inline float4 CalculateFrustumCorrection()
	{
		float x1 = -UNITY_MATRIX_P._31/(UNITY_MATRIX_P._11*UNITY_MATRIX_P._34);
		float x2 = -UNITY_MATRIX_P._32/(UNITY_MATRIX_P._22*UNITY_MATRIX_P._34);
		return float4(x1, x2, 0, UNITY_MATRIX_P._33/UNITY_MATRIX_P._34 + x1*UNITY_MATRIX_P._13 + x2*UNITY_MATRIX_P._23);
	}


	//VERTEX SHADER
	v2f vert (appdata v)
	{
		v2f o;
		UNITY_INITIALIZE_OUTPUT(v2f, o);
		UNITY_SETUP_INSTANCE_ID(v);
		//
		UNITY_TRANSFER_INSTANCE_ID(v, o);
		o.audioGlobalFinalIntensity.x = GetAudioReactAmplitude();
		o.audioGlobalFinalIntensity.y = getGlobalIntensity();
		o.audioGlobalFinalIntensity.z = getFinalIntensity();
		o.emissionColor = getEmissionColor();
		v.vertex = CalculateProjectionScaleRange(v, v.vertex, _ProjectionRange);
		o.projectionorigin = CalculateProjectionScaleRange(v, _ProjectionRangeOrigin, _ProjectionRange);
		//move verts to clip space
		o.pos = UnityObjectToClipPos(v.vertex);

		//get screen space position of verts
		o.screenPos = ComputeScreenPos(o.pos);
		//Putting in the vertex position before the transformation seems to somewhat move the projection correctly, but is still incorrect...?
		o.ray = UnityObjectToViewPos(v.vertex).xyz;
		//invert z axis so that it projects from camera properly
		o.ray *= float3(1,1,-1);
		//saving vertex color incase needing to perform rotation calculation in fragment shader
		o.color = v.color;
		o.worldPos = mul(unity_ObjectToWorld, v.vertex);
		//For Mirror Depth Correction
		o.worldDirection.xyz = o.worldPos.xyz - _WorldSpaceCameraPos;
		// pack correction factor into direction w component to save space
		o.worldDirection.w = dot(o.pos, CalculateFrustumCorrection());
		if(o.audioGlobalFinalIntensity.x <= 0.005 || o.audioGlobalFinalIntensity.y <= 0.005 || o.audioGlobalFinalIntensity.z <= 0.005 || all(o.emissionColor.xyz <= float4(0.005, 0.005, 0.005, 1.0)))
		{
			v.vertex = float4(0,0,0,0);
			o.pos = UnityObjectToClipPos(v.vertex);
		}
		return o;
	}
				
	fixed4 frag (v2f i) : SV_Target
	{
		//UNITY_SETUP_INSTANCE_ID(i);
		return ProjectionFrag(i);
	}
			 ENDCG

		 }


	}
	CustomEditor "VRSLInspector"
}
