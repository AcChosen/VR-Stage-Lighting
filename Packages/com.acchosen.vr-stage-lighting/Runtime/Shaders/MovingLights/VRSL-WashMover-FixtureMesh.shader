Shader "VRSL/Wash Mover/Fixture"
{
	Properties
	{
		//[Header (INSTANCED PROPERITES)]
		 [Enum(Legacy, 0, GGX, 1)]_LightingModel("Lighting Model", Int) = 0
		 [HideInInspector]_DMXChannel ("Starting DMX Channel", Int) = 0
		 [HideInInspector][Toggle] _NineUniverseMode ("Extended Universe Mode", Int) = 0
		 [HideInInspector][Toggle] _PanInvert ("Invert Mover Pan", Int) = 0
		 [HideInInspector][Toggle] _TiltInvert ("Invert Mover Tilt", Int) = 0
		 [Toggle] _EnableStrobe ("Enable Strobe", Int) = 0
		 [Toggle] _EnableVerticalMode ("Enable Vertical Mode", Int) = 0
		 [Toggle] _EnableDMX ("Enable Stream DMX/DMX Control", Int) = 0
		 [Toggle] _EnableCompatibilityMode ("Enable Compatibility Mode", Int) = 0
		 [HideInInspector]_FixtureBaseRotationY("Mover Pan Offset (Blue + Green)", Range(-540,540)) = 0
		 [HideInInspector]_FixtureRotationX("Mover Tilt Offset (Blue)", Range(-180,180)) = 0
		 [HideInInspector]_ProjectionSelection ("GOBO Selection", Range(0,6)) = 0
		 [HideInInspector]_StrobeFreq("Strobe Frequency", Range(0,25)) = 1
		 [HideInInspector][Toggle] _EnableSpin("Enable Auto Spinning", Float) = 0
		_FinalIntensity("Final Intensity", Range(0,1)) = 1
		_GlobalIntensity("Global Intensity", Range(0,1)) = 1
		_GlobalIntensityBlend("Global Intensity Blend", Range(0,1)) = 1
		_UniversalIntensity ("Universal Intensity", Range (0,1)) = 1
		[HDR]_Emission("Light Color Tint", Color) = (1,1,1,1)
		_Saturation("Final Saturation", Range(0,1)) = 1
		_SaturationLength("Final Saturation Length", Range(0,0.2)) = 0.1
		_LensMaxBrightness("Lens Max Brightness", Range(0.00, 200)) = 5
		_ConeWidth("Cone Width", Range(0,5.5)) = 0
		_ConeLength("Cone Length", Range(1,10)) = 1
		_ConeSync ("Cone Scale Sync", Range(0,1)) = 0.2
		_FixutreIntensityMultiplier ("Intensity Multipler (For Bloom Scaling)", Range(1,100)) = 1
		_FixtureRotationOrigin("Fixture Pivot Origin", Float) = (0, 0.014709, -1.02868, 0)
		[Toggle] _UseRawGrid("Use Raw Grid For Light Intensity And Color", Int) = 0
		_MaxMinPanAngle("Max/Min Pan Angle (-x, x)", Float) = 180
		_MaxMinTiltAngle("Max/Min Tilt Angle (-y, y)", Float) = 180
		_FixtureMaxIntensity ("Maximum Cone Intensity",Range (0,0.5)) = 0.5
		[Enum(Unity Default, 0, Non Linear, 1)]_LightProbeMethod("Light Probe Sampling", Int) = 0
		_MainTex("Main Texture", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)

		[Space(16)]
		[Header(NORMALS)]
		_BumpMap("Normal Map", 2D) = "bump" {}
		_BumpScale("Normal Scale", Range(-1,1)) = 1
		[Space(16)]
		[Header(METALLIC)]
		_MetallicGlossMap("Metallic Map", 2D) = "white" {}
		_Metallic("Metallic", Range(0,1)) = 0
		_Glossiness("Smoothness", Range(0,1)) = 0
		_OcclusionMap("Occlusion Map", 2D) = "white" {}
		_OcclusionStrength("Occlusion Strength", Range(0,1)) = 0
		_DecorativeEmissiveMap("Decorative Emissive Map", 2D) = "black" {}
		_DecorativeEmissiveMapStrength("Decorative Emissive Map Strength", Range(0,1)) = 0

	}
    SubShader
    {
        Tags
        {
            "Queue" = "AlphaTest+1" "RenderType" = "Opaque" "RenderingPipeline" = "UniversalPipeline"
        }

        Pass
        {
            Name "UniversalForward"
            Tags
            {
                "LightMode" = "UniversalForward"
            }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #pragma multi_compile_instancing
            #pragma shader_feature_local _LIGHTING_MODEL
            //REMOVE THIS WHEN FINISHED DEBUGGING
            //#pragma target 4.5

            #define GEOMETRY

            #ifndef UNITY_PASS_FORWARDBASE
            #define UNITY_PASS_FORWARDBASE
            #endif

            #define FIXTURE_EMIT
            #define VRSL_DMX
            #define WASH
            //DEBUGGING BUFFER

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float3 normal : NORMAL;
                float3 tangent : TANGENT;
                float4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float3 btn[3] : TEXCOORD3; //TEXCOORD2, TEXCOORD3 | bitangent, tangent, worldNormal
                float3 worldPos : TEXCOORD6;
                #ifdef _LIGHTING_MODEL
			        UNITY_LIGHTING_COORDS(7,8)
			        float4 eyeVec : TEXCOORD12;
			        half4 ambientOrLightmapUV : TEXCOORD13;
                #else
                float3 objPos : TEXCOORD7;
                float3 objNormal : TEXCOORD8;
                float4 _ShadowCoord : TEXCOORD11;
                #endif
                float4 color : COLOR;
                float2 intensityStrobe : TEXCOORD9;
                float4 rgbColor : TEXCOORD10;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            #include "../Shared/VRSL-Defines.cginc"
            #include "../Shared/VRSL-DMXFunctions.cginc"
            #include "../Shared/VRSL-LightingFunctions.cginc"
            #include "../Shared/VRSL-StandardLighting.cginc"
            #include "VRSL-StandardMover-Vertex.cginc"
            ENDCG
        }

        Pass
        {
            Name "ShadowCaster"
            Tags
            {
                "LightMode"="ShadowCaster"
            }

            CGPROGRAM
            #define FIXTURE_SHADOWCAST
            #define VRSL_DMX
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float3 normal : NORMAL;
                float3 tangent : TANGENT;
                float4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 normal: TEXCOORD0;
                float4 color: COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            #include "../Shared/VRSL-Defines.cginc"
            #include "../Shared/VRSL-DMXFunctions.cginc"
            #include "VRSL-StandardMover-Vertex.cginc"
            ENDCG
        }

        // DepthOnly and DepthNormals MUST utilize exact same rotations as the forward pass for vertex rotation to have rendering continuity

        Pass
        {
            Name "DepthOnly"
            Tags
            {
                "LightMode" = "DepthOnly"
            }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #pragma multi_compile_instancing
            #pragma shader_feature_local _LIGHTING_MODEL
            //REMOVE THIS WHEN FINISHED DEBUGGING
            //#pragma target 4.5

            #define GEOMETRY

            #ifndef UNITY_PASS_FORWARDBASE
            #define UNITY_PASS_FORWARDBASE
            #endif

            #define FIXTURE_EMIT
            #define VRSL_DMX
            #define WASH

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float3 normal : NORMAL;
                float3 tangent : TANGENT;
                float4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float3 btn[3] : TEXCOORD3; //TEXCOORD2, TEXCOORD3 | bitangent, tangent, worldNormal
                float3 worldPos : TEXCOORD6;
                #ifdef _LIGHTING_MODEL
			        UNITY_LIGHTING_COORDS(7,8)
			        float4 eyeVec : TEXCOORD12;
			        half4 ambientOrLightmapUV : TEXCOORD13;
                #else
                float3 objPos : TEXCOORD7;
                float3 objNormal : TEXCOORD8;
                float4 _ShadowCoord : TEXCOORD11;
                #endif
                float4 color : COLOR;
                float2 intensityStrobe : TEXCOORD9;
                float4 rgbColor : TEXCOORD10;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            #include "../Shared/VRSL-Defines.cginc"
            #include "../Shared/VRSL-DMXFunctions.cginc"
            #include "../Shared/VRSL-LightingFunctions.cginc"
            #include "../Shared/VRSL-StandardLighting.cginc"
            #include "VRSL-StandardMover-Vertex.cginc"
            ENDCG
        }

        Pass
        {
            Name "DepthNormals"
            Tags
            {
                "LightMode" = "DepthNormals"
            }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #pragma multi_compile_instancing
            #pragma shader_feature_local _LIGHTING_MODEL
            //REMOVE THIS WHEN FINISHED DEBUGGING
            //#pragma target 4.5

            #define GEOMETRY

            #ifndef UNITY_PASS_FORWARDBASE
            #define UNITY_PASS_FORWARDBASE
            #endif

            #define FIXTURE_EMIT
            #define VRSL_DMX
            #define WASH
            //DEBUGGING BUFFER

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float3 normal : NORMAL;
                float3 tangent : TANGENT;
                float4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float2 uv2 : TEXCOORD2;
                float3 btn[3] : TEXCOORD3; //TEXCOORD2, TEXCOORD3 | bitangent, tangent, worldNormal
                float3 worldPos : TEXCOORD6;
                #ifdef _LIGHTING_MODEL
			        UNITY_LIGHTING_COORDS(7,8)
			        float4 eyeVec : TEXCOORD12;
			        half4 ambientOrLightmapUV : TEXCOORD13;
                #else
                float3 objPos : TEXCOORD7;
                float3 objNormal : TEXCOORD8;
                float4 _ShadowCoord : TEXCOORD11;
                #endif
                float4 color : COLOR;
                float2 intensityStrobe : TEXCOORD9;
                float4 rgbColor : TEXCOORD10;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            #include "../Shared/VRSL-Defines.cginc"
            #include "../Shared/VRSL-DMXFunctions.cginc"
            #include "../Shared/VRSL-LightingFunctions.cginc"
            #include "../Shared/VRSL-StandardLighting.cginc"
            #include "VRSL-StandardMover-Vertex.cginc"
            ENDCG
        }

    }
	
		SubShader
	{
		
		Tags{ "Queue" = "AlphaTest+1" "RenderType" = "Opaque" }

		Pass
	{
		Tags{ "LightMode" = "ForwardBase" }
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_fwdbase
#pragma multi_compile_instancing
#pragma shader_feature_local _LIGHTING_MODEL
//REMOVE THIS WHEN FINISHED DEBUGGING
//#pragma target 4.5

#define GEOMETRY

#ifndef UNITY_PASS_FORWARDBASE
#define UNITY_PASS_FORWARDBASE
#endif

#define FIXTURE_EMIT
#define VRSL_DMX
#define WASH
//DEBUGGING BUFFER
RWStructuredBuffer<float> buffer : register(u1);
RWStructuredBuffer<float4> buffer4 : register(u2);

#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

		struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
		float2 uv1 : TEXCOORD1;
		float2 uv2 : TEXCOORD2;
		float3 normal : NORMAL;
		float3 tangent : TANGENT;
		float4 color : COLOR;
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	struct v2f
	{
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
		float2 uv1 : TEXCOORD1;
		float2 uv2 : TEXCOORD2;
		float3 btn[3] : TEXCOORD3; //TEXCOORD2, TEXCOORD3 | bitangent, tangent, worldNormal
		float3 worldPos : TEXCOORD6;
		#ifdef _LIGHTING_MODEL
			UNITY_LIGHTING_COORDS(7,8)
			float4 eyeVec : TEXCOORD12;
			half4 ambientOrLightmapUV : TEXCOORD13;
		#else
			float3 objPos : TEXCOORD7;
			float3 objNormal : TEXCOORD8;
	        float4 _ShadowCoord : TEXCOORD11;
		#endif
		float4 color : COLOR;
		float2 intensityStrobe : TEXCOORD9;
		float4 rgbColor : TEXCOORD10;
		UNITY_VERTEX_INPUT_INSTANCE_ID
		UNITY_VERTEX_OUTPUT_STEREO
		
		
	};

	#include "../Shared/VRSL-Defines.cginc"
	#include "../Shared/VRSL-DMXFunctions.cginc"
	#include "../Shared/VRSL-LightingFunctions.cginc"
	#include "../Shared/VRSL-StandardLighting.cginc"
	#include "VRSL-StandardMover-Vertex.cginc"

	ENDCG
	}
			Pass
	{
		Tags {"LightMode"="ShadowCaster"}

		CGPROGRAM
		#define FIXTURE_SHADOWCAST
		#define VRSL_DMX
		#pragma vertex vert
		#pragma fragment frag
		#pragma multi_compile_shadowcaster
		#pragma multi_compile_instancing
		#include "UnityCG.cginc"
		

		struct appdata
		{
			float4 vertex : POSITION;
			float2 uv : TEXCOORD0;
			float2 uv1 : TEXCOORD1;
			float2 uv2 : TEXCOORD2;
			float3 normal : NORMAL;
			float3 tangent : TANGENT;
			float4 color : COLOR;
			UNITY_VERTEX_INPUT_INSTANCE_ID
		};

		struct v2f
		{
			float4 pos : SV_POSITION;
			float3 normal: TEXCOORD0;
		    float4 color : COLOR;
			UNITY_VERTEX_INPUT_INSTANCE_ID
			UNITY_VERTEX_OUTPUT_STEREO
			//SHADOW_COORDS(11)
		};
		#include "../Shared/VRSL-Defines.cginc"
		#include "../Shared/VRSL-DMXFunctions.cginc"
		#include "VRSL-StandardMover-Vertex.cginc"
	ENDCG
	}


	}
	CustomEditor "VRSLInspector"
}
