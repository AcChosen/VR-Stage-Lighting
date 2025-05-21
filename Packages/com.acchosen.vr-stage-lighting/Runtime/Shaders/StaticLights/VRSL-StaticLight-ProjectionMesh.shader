Shader "VRSL/Standard Static/Projection"
{
	
	Properties
	{
		
		//[Header (INSTANCED PROPERITES)]
		 [HideInInspector]_DMXChannel ("DMX Fixture Number/Sector (Per 13 Channels)", Int) = 0
		  [Toggle] _NineUniverseMode ("Extended Universe Mode", Int) = 0

		 [HideInInspector][Toggle] _EnableStrobe ("Enable Strobe", Int) = 0

		 [Toggle] _EnableCompatibilityMode ("Enable Compatibility Mode", Int) = 0
		 [Toggle] _EnableVerticalMode ("Enable Vertical Mode", Int) = 0
		 [Toggle] _EnableDMX ("Enable Stream DMX/DMX Control", Int) = 0
		 [Enum(UnityEngine.Rendering.BlendMode)] _BlendSrc ("Source Blend mode", Float) = 2
		 //[Enum(UnityEngine.Rendering.BlendMode)] _BlendDst ("Destination Blend mode", Float) = 1
		 [Enum(UnityEngine.Rendering.BlendOp)] _BlendOp ("Blend Operation", Float) = 0
		// _BlockLengthX("DMX Block Base Distance X", Float) = 0.019231
		// _BlockLengthY("DMX Block Base Distance Y", Float) = 0
		 [HideInInspector]_StrobeFreq("Strobe Frequency", Range(0,25)) = 1

		//[Header(LIGHTING CONTROLS)]
		_GlobalIntensity("Global Intensity", Range(0,1)) = 1
		_GlobalIntensityBlend("Global Intensity Blend", Range(0,1)) = 1
		_FinalIntensity("Final Intensity", Range(0,1)) = 1
		_UniversalIntensity ("Universal Intensity", Range (0,1)) = 1
		[HDR]_Emission("Light Color Tint", Color) = (1,1,1,1)
		_FixtureMaxIntensity ("Maximum Light Intensity",Range (0,6)) = 1
		
		[Toggle] _UseRawGrid("Use Raw Grid For Light Intensity", Int) = 0
		// [NoScaleOffset] _Udon_DMXGridRenderTexture("DMX Grid Render Texture (RAW Unsmoothed)", 2D) = "white" {}
		// [NoScaleOffset] _Udon_DMXGridRenderTextureMovement("DMX Grid Render Texture (To Control Lights)", 2D) = "white" {}
		// [NoScaleOffset] _Udon_DMXGridStrobeTimer("DMX Grid Render Texture (For Strobe Timings", 2D) = "white" {}
		//[NoScaleOffset] _SceneAlbedo ("Scene Albedo Render Texture", 2D) = "white" {}



		//[Header(PROJECTION SETTINGS)]

		[NoScaleOffset] _ProjectionMainTex ("Projection Texture GOBO 1", 2D) = "white"{}
		_ProjectionMaxIntensity ("Maximum Projection Intensity", Range (0,50)) = 1
		
		_XOffset ("Projection Offset X", Range(-6, 6)) = 0
		_YOffset ("Projection Offset Y", Range(-6, 6)) = 0
		
		
		_ConeWidth("Specular Strength or whatever", Range(0,5)) = 0
		_ProjectionRange ("Projection Drawing Range", Range(0,20)) = 0
		_ProjectionRangeOrigin ("Projection Drawing Range Scale Origin", Float) = (0, -0.07535, 0.12387, 0)

		//[Space(12)]
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

		[Enum(Transparent,1,AlphaToCoverage,2)] _RenderMode ("Render Mode", Int) = 1
        [Enum(Off,0,On,1)] _ZWrite ("Z Write", Int) = 0
		[Enum(Off,0,On,1)] _AlphaToCoverage ("Alpha To Coverage", Int) = 0
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
            AlphaToMask [_AlphaToCoverage]
            Cull Front
            Ztest GEqual
            ZWrite Off
            Blend DstColor [_BlendDst]
            BlendOp Add
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
            #pragma multi_compile_local _ _ALPHATEST_ON
            #pragma shader_feature_local _CHANNEL_MODE
            #pragma shader_feature_local _MULTISAMPLEDEPTH
            //#pragma multi_compile_fog
            #pragma multi_compile_instancing

            #define PROJECTION_YES
            #define VRSL_DMX

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
                float2 dmx: TEXCOORD10;
                float4 projectionorigin : TEXCOORD5;
                float4 worldDirection : TEXCOORD6;
                float4 worldPos : TEXCOORD7;
                float2 intensityStrobe : TEXCOORD11;
                float4 rgbColor : TEXCOORD12;
                float4 emissionColor : TEXCOORD13;
                float2 globalFinalIntensity : TEXCOORD14;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            #include "../Shared/VRSL-Defines.cginc"
            #include "../Shared/VRSL-DMXFunctions.cginc"
            #include "VRSL-StaticLight-ProjectionFrag.cginc"
            //float _AlphaProjectionIntensity;

            #define IF(a, b, c) lerp(b, c, step((fixed) (a), 0));

            float4 CalculateProjectionScaleRange(appdata v, float4 input, float scalar)
            {
                float4 oldinput = input;
                float4x4 scaleMatrix = float4x4(
                    scalar, 0, 0, 0,
                    0, scalar, 0, 0,
                    0, 0, scalar, 0,
                    0, 0, 0, 1.0
                );
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
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                uint dmx = getDMXChannel();
                #ifdef _CHANNEL_MODE
			        o.intensityStrobe = float2(getValueAtCoords(dmx, _Udon_DMXGridRenderTexture),GetStrobeOutputFiveCH(dmx));
			        o.rgbColor = float4(getValueAtCoords(dmx+1, _Udon_DMXGridRenderTexture), getValueAtCoords(dmx+2, _Udon_DMXGridRenderTexture), getValueAtCoords(dmx+3, _Udon_DMXGridRenderTexture), 1);
			        o.rgbColor *= o.intensityStrobe.x;
			        o.emissionColor = getEmissionColor();
                #else
                    o.intensityStrobe = float2(GetDMXIntensity(dmx, 1.0), GetStrobeOutput(dmx));
                    o.rgbColor = GetDMXColor(dmx);
                    o.emissionColor = getEmissionColor();
                #endif
                o.globalFinalIntensity.x = getGlobalIntensity();
                o.globalFinalIntensity.y = getFinalIntensity();

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
                if (((all(o.rgbColor <= float4(0.05, 0.05, 0.05, 1)) || o.intensityStrobe.x <= 0.05) && isDMX() == 1) ||
                    o.globalFinalIntensity.x <= 0.005 || o.globalFinalIntensity.y <= 0.005 || all(
                        o.emissionColor <= float4(0.005, 0.005, 0.005, 1.0)))
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
			AlphaToMask [_AlphaToCoverage]
            Cull Front
            Ztest GEqual
            ZWrite Off
            Blend  DstColor [_BlendDst]	
            BlendOp Add
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
			#pragma multi_compile_local _ _ALPHATEST_ON
			#pragma shader_feature_local _CHANNEL_MODE
			#pragma shader_feature_local _MULTISAMPLEDEPTH
			//#pragma multi_compile_fog
			#pragma multi_compile_instancing

			#define PROJECTION_YES
			#define VRSL_DMX

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
				 float2 dmx: TEXCOORD10;
				 float4 projectionorigin : TEXCOORD5;
				 float4 worldDirection : TEXCOORD6;
				 float4 worldPos : TEXCOORD7;
				 float2 intensityStrobe : TEXCOORD11;
				 float4 rgbColor : TEXCOORD12;
				 float4 emissionColor : TEXCOORD13;
				 float2 globalFinalIntensity : TEXCOORD14;
				 UNITY_VERTEX_INPUT_INSTANCE_ID
				 UNITY_VERTEX_OUTPUT_STEREO
             };
			#include "../Shared/VRSL-Defines.cginc"
			#include "../Shared/VRSL-DMXFunctions.cginc"
			#include "VRSL-StaticLight-ProjectionFrag.cginc"
			//float _AlphaProjectionIntensity;

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
		UNITY_SETUP_INSTANCE_ID(v);
		UNITY_INITIALIZE_OUTPUT(v2f, o);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
		UNITY_TRANSFER_INSTANCE_ID(v, o);
		uint dmx = getDMXChannel();
		#ifdef _CHANNEL_MODE
			o.intensityStrobe = float2(getValueAtCoords(dmx, _Udon_DMXGridRenderTexture),GetStrobeOutputFiveCH(dmx));
			o.rgbColor = float4(getValueAtCoords(dmx+1, _Udon_DMXGridRenderTexture), getValueAtCoords(dmx+2, _Udon_DMXGridRenderTexture), getValueAtCoords(dmx+3, _Udon_DMXGridRenderTexture), 1);
			o.rgbColor *= o.intensityStrobe.x;
			o.emissionColor = getEmissionColor();
		#else
			o.intensityStrobe = float2(GetDMXIntensity(dmx, 1.0),GetStrobeOutput(dmx));
			o.rgbColor = GetDMXColor(dmx);
			o.emissionColor = getEmissionColor();
		#endif
		o.globalFinalIntensity.x = getGlobalIntensity();
		o.globalFinalIntensity.y = getFinalIntensity();

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
		if(((all(o.rgbColor <= float4(0.05,0.05,0.05,1)) || o.intensityStrobe.x <= 0.05) && isDMX() == 1) || o.globalFinalIntensity.x <= 0.005 || o.globalFinalIntensity.y <= 0.005 || all(o.emissionColor <= float4(0.005, 0.005, 0.005, 1.0)))
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
