// Upgrade NOTE: upgraded instancing buffer 'VRSLAmplifyVRSLAmplifySurface' to new syntax.

// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "VRSL/Amplify/VRSL-AmplifySurface"
{
	Properties
	{
		[Normal][SingleLineTexture]_NormalMap("Normal Map", 2D) = "bump" {}
		[SingleLineTexture]_AlbedoMap("Albedo Map", 2D) = "white" {}
		_DMXGrid("DMX Grid", 2D) = "white" {}
		[SingleLineTexture]_MetallicSmoothnessMap("Metallic Smoothness Map", 2D) = "white" {}
		_DMXStrobeTimer("DMX Strobe Timer", 2D) = "white" {}
		[SingleLineTexture]_AmbientOcclusionMap("Ambient Occlusion Map", 2D) = "white" {}
		[SingleLineTexture]_EmissionMask("Emission Mask", 2D) = "white" {}
		_DMXChannel("DMX Channel", Int) = 0
		[Toggle]_LegacyMode("Legacy Mode?", Int) = 0
		[Toggle]_EnableStrobe("Can Strobe?", Int) = 1
		_UniversalIntensity("Universal Intensity", Range( 0 , 1)) = 1
		_GlobalIntensity("Global Intensity", Range( 0 , 1)) = 1
		_FinalIntensity("Final Intensity", Range( 0 , 1)) = 1
		_EmissionStrengthMultiplier("Emission Strength Multiplier", Range( 1 , 200)) = 1
		_MetallicStrength("Metallic Strength", Range( 0 , 1)) = 0
		_SmoothnessStrength("Smoothness Strength", Range( 0 , 1)) = 0
		_AmbientOcclusionStrength("Ambient Occlusion Strength", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma multi_compile_instancing
		#define ASE_USING_SAMPLING_MACROS 1
		#if defined(SHADER_API_D3D11) || defined(SHADER_API_XBOXONE) || defined(UNITY_COMPILER_HLSLCC) || defined(SHADER_API_PSSL) || (defined(SHADER_TARGET_SURFACE_ANALYSIS) && !defined(SHADER_TARGET_SURFACE_ANALYSIS_MOJOSHADER))//ASE Sampler Macros
		#define SAMPLE_TEXTURE2D(tex,samplerTex,coord) tex.Sample(samplerTex,coord)
		#define SAMPLE_TEXTURE2D_LOD(tex,samplerTex,coord,lod) tex.SampleLevel(samplerTex,coord, lod)
		#define SAMPLE_TEXTURE2D_BIAS(tex,samplerTex,coord,bias) tex.SampleBias(samplerTex,coord,bias)
		#define SAMPLE_TEXTURE2D_GRAD(tex,samplerTex,coord,ddx,ddy) tex.SampleGrad(samplerTex,coord,ddx,ddy)
		#else//ASE Sampling Macros
		#define SAMPLE_TEXTURE2D(tex,samplerTex,coord) tex2D(tex,coord)
		#define SAMPLE_TEXTURE2D_LOD(tex,samplerTex,coord,lod) tex2Dlod(tex,float4(coord,0,lod))
		#define SAMPLE_TEXTURE2D_BIAS(tex,samplerTex,coord,bias) tex2Dbias(tex,float4(coord,0,bias))
		#define SAMPLE_TEXTURE2D_GRAD(tex,samplerTex,coord,ddx,ddy) tex2Dgrad(tex,coord,ddx,ddy)
		#endif//ASE Sampling Macros

		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		UNITY_DECLARE_TEX2D_NOSAMPLER(_NormalMap);
		SamplerState sampler_linear_repeat;
		UNITY_DECLARE_TEX2D_NOSAMPLER(_AlbedoMap);
		UNITY_DECLARE_TEX2D_NOSAMPLER(_EmissionMask);
		UNITY_DECLARE_TEX2D_NOSAMPLER(_DMXStrobeTimer);
		uniform int _LegacyMode;
		float4 _DMXStrobeTimer_TexelSize;
		SamplerState sampler_DMXStrobeTimer;
		UNITY_DECLARE_TEX2D_NOSAMPLER(_DMXGrid);
		float4 _DMXGrid_TexelSize;
		SamplerState sampler_DMXGrid;
		uniform float _UniversalIntensity;
		uniform float _EmissionStrengthMultiplier;
		UNITY_DECLARE_TEX2D_NOSAMPLER(_MetallicSmoothnessMap);
		uniform float _MetallicStrength;
		uniform float _SmoothnessStrength;
		UNITY_DECLARE_TEX2D_NOSAMPLER(_AmbientOcclusionMap);
		uniform float _AmbientOcclusionStrength;

		UNITY_INSTANCING_BUFFER_START(VRSLAmplifyVRSLAmplifySurface)
			UNITY_DEFINE_INSTANCED_PROP(float4, _NormalMap_ST)
#define _NormalMap_ST_arr VRSLAmplifyVRSLAmplifySurface
			UNITY_DEFINE_INSTANCED_PROP(float4, _AlbedoMap_ST)
#define _AlbedoMap_ST_arr VRSLAmplifyVRSLAmplifySurface
			UNITY_DEFINE_INSTANCED_PROP(float4, _EmissionMask_ST)
#define _EmissionMask_ST_arr VRSLAmplifyVRSLAmplifySurface
			UNITY_DEFINE_INSTANCED_PROP(float4, _MetallicSmoothnessMap_ST)
#define _MetallicSmoothnessMap_ST_arr VRSLAmplifyVRSLAmplifySurface
			UNITY_DEFINE_INSTANCED_PROP(float4, _AmbientOcclusionMap_ST)
#define _AmbientOcclusionMap_ST_arr VRSLAmplifyVRSLAmplifySurface
			UNITY_DEFINE_INSTANCED_PROP(int, _DMXChannel)
#define _DMXChannel_arr VRSLAmplifyVRSLAmplifySurface
			UNITY_DEFINE_INSTANCED_PROP(int, _EnableStrobe)
#define _EnableStrobe_arr VRSLAmplifyVRSLAmplifySurface
			UNITY_DEFINE_INSTANCED_PROP(float, _FinalIntensity)
#define _FinalIntensity_arr VRSLAmplifyVRSLAmplifySurface
			UNITY_DEFINE_INSTANCED_PROP(float, _GlobalIntensity)
#define _GlobalIntensity_arr VRSLAmplifyVRSLAmplifySurface
		UNITY_INSTANCING_BUFFER_END(VRSLAmplifyVRSLAmplifySurface)


		float2 BaseRead( int DMXChannel )
		{
			uint ch = abs(DMXChannel);
			uint x = ch % 13;
			x = x == 0.0 ? 13.0 : x;
			float y = DMXChannel / 13.0;
			y = frac(y)== 0.00000 ? y - 1 : y;
			if(x == 13.0)
			{
			    y = DMXChannel >= 90 && DMXChannel <= 404 ? y - 1 : y;
			    y = DMXChannel >= 676 && DMXChannel <= 819 ? y - 1 : y;
			    y = DMXChannel >= 1339 ? y - 1 : y;
			}
			float2 xAndy = float2(x,y);
			return xAndy;
		}


		float2 LegacyRead( int channel, int sector )
		{
			channel = channel - 1;
			float x = 0.02000;
			float y = 0.02000;
			float ymod = floor(sector / 2.0); 
			float xmod = sector % 2.0;
			x+= (xmod * 0.50);
			y+= (ymod * 0.04);
			x+= (channel * 0.04);
			return float2(x,y);
		}


		float2 IndustryRead( float4 _OSCGridRenderTextureRAW_TexelSize, int x, int y )
		{
			y = y + 1;
			float resMultiplierX = (_OSCGridRenderTextureRAW_TexelSize.z / 13);
			float2 xyUV = float2(0.0,0.0);
			xyUV.x = ((x * resMultiplierX) * _OSCGridRenderTextureRAW_TexelSize.x);
			xyUV.y = (y * resMultiplierX) * _OSCGridRenderTextureRAW_TexelSize.y;
			xyUV.y -= 0.001;
			xyUV.x -= 0.015;
			return xyUV;
		}


		float SampleDMX88_g357( float4 c )
		{
			    float3 cRGB = float3(c.r, c.g, c.b);
			    float value = LinearRgbToLuminance(cRGB);
			    value = LinearToGammaSpaceExact(value);
			    return value;
		}


		float GetStrobe35_g356( float phase, float status, int toggle )
		{
			half strobe = (sin(phase));   //Get sin wave
			strobe = strobe > 0.0 ? 1.0 : 0.0; //turn to square wave
			strobe = status > 0.2 ? strobe : 1; //minimum channel threshold set
			strobe = toggle == 1 ? strobe : 1; //if toggle is not on, just output only 1
			return strobe;
		}


		float SampleDMX88_g351( float4 c )
		{
			    float3 cRGB = float3(c.r, c.g, c.b);
			    float value = LinearRgbToLuminance(cRGB);
			    value = LinearToGammaSpaceExact(value);
			    return value;
		}


		float SampleDMX88_g355( float4 c )
		{
			    float3 cRGB = float3(c.r, c.g, c.b);
			    float value = LinearRgbToLuminance(cRGB);
			    value = LinearToGammaSpaceExact(value);
			    return value;
		}


		float SampleDMX88_g353( float4 c )
		{
			    float3 cRGB = float3(c.r, c.g, c.b);
			    float value = LinearRgbToLuminance(cRGB);
			    value = LinearToGammaSpaceExact(value);
			    return value;
		}


		float SampleDMX88_g354( float4 c )
		{
			    float3 cRGB = float3(c.r, c.g, c.b);
			    float value = LinearRgbToLuminance(cRGB);
			    value = LinearToGammaSpaceExact(value);
			    return value;
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 _NormalMap_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(_NormalMap_ST_arr, _NormalMap_ST);
			float2 uv_NormalMap = i.uv_texcoord * _NormalMap_ST_Instance.xy + _NormalMap_ST_Instance.zw;
			o.Normal = UnpackNormal( SAMPLE_TEXTURE2D( _NormalMap, sampler_linear_repeat, uv_NormalMap ) );
			float4 _AlbedoMap_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(_AlbedoMap_ST_arr, _AlbedoMap_ST);
			float2 uv_AlbedoMap = i.uv_texcoord * _AlbedoMap_ST_Instance.xy + _AlbedoMap_ST_Instance.zw;
			o.Albedo = SAMPLE_TEXTURE2D( _AlbedoMap, sampler_linear_repeat, uv_AlbedoMap ).rgb;
			float4 _EmissionMask_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(_EmissionMask_ST_arr, _EmissionMask_ST);
			float2 uv_EmissionMask = i.uv_texcoord * _EmissionMask_ST_Instance.xy + _EmissionMask_ST_Instance.zw;
			int temp_output_17_0_g356 = _LegacyMode;
			int _DMXChannel_Instance = UNITY_ACCESS_INSTANCED_PROP(_DMXChannel_arr, _DMXChannel);
			int temp_output_6_0_g356 = ( _DMXChannel_Instance + 4 );
			int DMXChannel78_g358 = abs( temp_output_6_0_g356 );
			float2 localBaseRead78_g358 = BaseRead( DMXChannel78_g358 );
			float2 break95_g358 = localBaseRead78_g358;
			int channel79_g358 = (int)break95_g358.x;
			int sector79_g358 = (int)break95_g358.y;
			float2 localLegacyRead79_g358 = LegacyRead( channel79_g358 , sector79_g358 );
			float4 _OSCGridRenderTextureRAW_TexelSize84_g358 = _DMXStrobeTimer_TexelSize;
			int x84_g358 = (int)break95_g358.x;
			int y84_g358 = (int)break95_g358.y;
			float2 localIndustryRead84_g358 = IndustryRead( _OSCGridRenderTextureRAW_TexelSize84_g358 , x84_g358 , y84_g358 );
			float phase35_g356 = SAMPLE_TEXTURE2D( _DMXStrobeTimer, sampler_DMXStrobeTimer, ( temp_output_17_0_g356 == 1 ? localLegacyRead79_g358 : localIndustryRead84_g358 ) ).r;
			int DMXChannel78_g357 = abs( temp_output_6_0_g356 );
			float2 localBaseRead78_g357 = BaseRead( DMXChannel78_g357 );
			float2 break95_g357 = localBaseRead78_g357;
			int channel79_g357 = (int)break95_g357.x;
			int sector79_g357 = (int)break95_g357.y;
			float2 localLegacyRead79_g357 = LegacyRead( channel79_g357 , sector79_g357 );
			float4 _OSCGridRenderTextureRAW_TexelSize84_g357 = _DMXGrid_TexelSize;
			int x84_g357 = (int)break95_g357.x;
			int y84_g357 = (int)break95_g357.y;
			float2 localIndustryRead84_g357 = IndustryRead( _OSCGridRenderTextureRAW_TexelSize84_g357 , x84_g357 , y84_g357 );
			float4 c88_g357 = SAMPLE_TEXTURE2D( _DMXGrid, sampler_DMXGrid, ( temp_output_17_0_g356 == 1 ? localLegacyRead79_g357 : localIndustryRead84_g357 ) );
			float localSampleDMX88_g357 = SampleDMX88_g357( c88_g357 );
			float status35_g356 = localSampleDMX88_g357;
			int _EnableStrobe_Instance = UNITY_ACCESS_INSTANCED_PROP(_EnableStrobe_arr, _EnableStrobe);
			int toggle35_g356 = _EnableStrobe_Instance;
			float localGetStrobe35_g356 = GetStrobe35_g356( phase35_g356 , status35_g356 , toggle35_g356 );
			int DMXChannel78_g351 = abs( _DMXChannel_Instance );
			float2 localBaseRead78_g351 = BaseRead( DMXChannel78_g351 );
			float2 break95_g351 = localBaseRead78_g351;
			int channel79_g351 = (int)break95_g351.x;
			int sector79_g351 = (int)break95_g351.y;
			float2 localLegacyRead79_g351 = LegacyRead( channel79_g351 , sector79_g351 );
			float4 _OSCGridRenderTextureRAW_TexelSize84_g351 = _DMXGrid_TexelSize;
			int x84_g351 = (int)break95_g351.x;
			int y84_g351 = (int)break95_g351.y;
			float2 localIndustryRead84_g351 = IndustryRead( _OSCGridRenderTextureRAW_TexelSize84_g351 , x84_g351 , y84_g351 );
			float4 c88_g351 = SAMPLE_TEXTURE2D( _DMXGrid, sampler_DMXGrid, ( _LegacyMode == 1 ? localLegacyRead79_g351 : localIndustryRead84_g351 ) );
			float localSampleDMX88_g351 = SampleDMX88_g351( c88_g351 );
			float temp_output_173_0 = localSampleDMX88_g351;
			int temp_output_6_0_g352 = _LegacyMode;
			int temp_output_8_0_g352 = ( _DMXChannel_Instance + 1 );
			int DMXChannel78_g355 = abs( temp_output_8_0_g352 );
			float2 localBaseRead78_g355 = BaseRead( DMXChannel78_g355 );
			float2 break95_g355 = localBaseRead78_g355;
			int channel79_g355 = (int)break95_g355.x;
			int sector79_g355 = (int)break95_g355.y;
			float2 localLegacyRead79_g355 = LegacyRead( channel79_g355 , sector79_g355 );
			float4 _OSCGridRenderTextureRAW_TexelSize84_g355 = _DMXGrid_TexelSize;
			int x84_g355 = (int)break95_g355.x;
			int y84_g355 = (int)break95_g355.y;
			float2 localIndustryRead84_g355 = IndustryRead( _OSCGridRenderTextureRAW_TexelSize84_g355 , x84_g355 , y84_g355 );
			float4 c88_g355 = SAMPLE_TEXTURE2D( _DMXGrid, sampler_DMXGrid, ( temp_output_6_0_g352 == 1 ? localLegacyRead79_g355 : localIndustryRead84_g355 ) );
			float localSampleDMX88_g355 = SampleDMX88_g355( c88_g355 );
			int temp_output_37_0_g352 = ( temp_output_8_0_g352 + 1 );
			int DMXChannel78_g353 = abs( temp_output_37_0_g352 );
			float2 localBaseRead78_g353 = BaseRead( DMXChannel78_g353 );
			float2 break95_g353 = localBaseRead78_g353;
			int channel79_g353 = (int)break95_g353.x;
			int sector79_g353 = (int)break95_g353.y;
			float2 localLegacyRead79_g353 = LegacyRead( channel79_g353 , sector79_g353 );
			float4 _OSCGridRenderTextureRAW_TexelSize84_g353 = _DMXGrid_TexelSize;
			int x84_g353 = (int)break95_g353.x;
			int y84_g353 = (int)break95_g353.y;
			float2 localIndustryRead84_g353 = IndustryRead( _OSCGridRenderTextureRAW_TexelSize84_g353 , x84_g353 , y84_g353 );
			float4 c88_g353 = SAMPLE_TEXTURE2D( _DMXGrid, sampler_DMXGrid, ( temp_output_6_0_g352 == 1 ? localLegacyRead79_g353 : localIndustryRead84_g353 ) );
			float localSampleDMX88_g353 = SampleDMX88_g353( c88_g353 );
			int DMXChannel78_g354 = abs( ( temp_output_37_0_g352 + 1 ) );
			float2 localBaseRead78_g354 = BaseRead( DMXChannel78_g354 );
			float2 break95_g354 = localBaseRead78_g354;
			int channel79_g354 = (int)break95_g354.x;
			int sector79_g354 = (int)break95_g354.y;
			float2 localLegacyRead79_g354 = LegacyRead( channel79_g354 , sector79_g354 );
			float4 _OSCGridRenderTextureRAW_TexelSize84_g354 = _DMXGrid_TexelSize;
			int x84_g354 = (int)break95_g354.x;
			int y84_g354 = (int)break95_g354.y;
			float2 localIndustryRead84_g354 = IndustryRead( _OSCGridRenderTextureRAW_TexelSize84_g354 , x84_g354 , y84_g354 );
			float4 c88_g354 = SAMPLE_TEXTURE2D( _DMXGrid, sampler_DMXGrid, ( temp_output_6_0_g352 == 1 ? localLegacyRead79_g354 : localIndustryRead84_g354 ) );
			float localSampleDMX88_g354 = SampleDMX88_g354( c88_g354 );
			float4 appendResult9_g352 = (float4(localSampleDMX88_g355 , localSampleDMX88_g353 , localSampleDMX88_g354 , 1.0));
			float _FinalIntensity_Instance = UNITY_ACCESS_INSTANCED_PROP(_FinalIntensity_arr, _FinalIntensity);
			float _GlobalIntensity_Instance = UNITY_ACCESS_INSTANCED_PROP(_GlobalIntensity_arr, _GlobalIntensity);
			float lerpResult190 = lerp( 0.0 , _EmissionStrengthMultiplier , temp_output_173_0);
			o.Emission = ( ( SAMPLE_TEXTURE2D( _EmissionMask, sampler_linear_repeat, uv_EmissionMask ).r * ( localGetStrobe35_g356 * ( temp_output_173_0 * appendResult9_g352 ) ) ) * ( _UniversalIntensity * ( _FinalIntensity_Instance * ( _GlobalIntensity_Instance * lerpResult190 ) ) ) ).rgb;
			float4 _MetallicSmoothnessMap_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(_MetallicSmoothnessMap_ST_arr, _MetallicSmoothnessMap_ST);
			float2 uv_MetallicSmoothnessMap = i.uv_texcoord * _MetallicSmoothnessMap_ST_Instance.xy + _MetallicSmoothnessMap_ST_Instance.zw;
			float4 tex2DNode10 = SAMPLE_TEXTURE2D( _MetallicSmoothnessMap, sampler_linear_repeat, uv_MetallicSmoothnessMap );
			o.Metallic = ( tex2DNode10.r * _MetallicStrength );
			o.Smoothness = ( tex2DNode10.a * _SmoothnessStrength );
			float4 _AmbientOcclusionMap_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(_AmbientOcclusionMap_ST_arr, _AmbientOcclusionMap_ST);
			float2 uv_AmbientOcclusionMap = i.uv_texcoord * _AmbientOcclusionMap_ST_Instance.xy + _AmbientOcclusionMap_ST_Instance.zw;
			float lerpResult70 = lerp( SAMPLE_TEXTURE2D( _AmbientOcclusionMap, sampler_linear_repeat, uv_AmbientOcclusionMap ).r , 1.0 , _AmbientOcclusionStrength);
			o.Occlusion = lerpResult70;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18935
245;301;1658;983;2012.546;326.2515;1.787992;True;False
Node;AmplifyShaderEditor.CommentaryNode;81;-964.9736,83.32082;Inherit;False;1223.884;667.8798;5 Channel Fixture Setup. Intensity, RGB, and Strobe. ;12;80;79;43;21;16;173;175;177;176;163;39;181;5 Channel Fixture DMX Reading;1,1,1,1;0;0
Node;AmplifyShaderEditor.IntNode;16;-870.2498,453.5368;Inherit;False;InstancedProperty;_DMXChannel;DMX Channel;7;0;Create;True;0;0;0;False;0;False;0;0;False;0;1;INT;0
Node;AmplifyShaderEditor.IntNode;21;-874.9551,384.8933;Inherit;False;Property;_LegacyMode;Legacy Mode?;8;0;Create;True;0;0;0;False;1;Toggle;False;0;0;False;0;1;INT;0
Node;AmplifyShaderEditor.TexturePropertyNode;176;-946.5815,549.8076;Inherit;True;Property;_DMXGrid;DMX Grid;2;0;Create;True;0;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.IntNode;175;-870.8794,313.3403;Inherit;False;InstancedProperty;_EnableStrobe;Can Strobe?;9;0;Create;False;0;0;0;False;1;Toggle;False;1;0;False;0;1;INT;0
Node;AmplifyShaderEditor.IntNode;163;-691.8258,683.2285;Inherit;False;Constant;_Int0;Int 0;19;0;Create;True;0;0;0;False;0;False;1;0;False;0;1;INT;0
Node;AmplifyShaderEditor.TexturePropertyNode;177;-912.5815,121.8076;Inherit;True;Property;_DMXStrobeTimer;DMX Strobe Timer;4;0;Create;True;0;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.CommentaryNode;82;-578.9083,-1176.978;Inherit;False;1188.889;1049.79;Standard Surface Shader Items;17;7;12;49;10;11;9;6;64;63;65;66;67;61;70;84;183;182;Standard Surface Shader;1,1,1,1;0;0
Node;AmplifyShaderEditor.FunctionNode;173;-565.9862,338.5289;Inherit;False;VRSL-ReadDMX;-1;;351;daf3802ef6ad79c4f8b45fd9600401f1;0;3;96;SAMPLER2D;0;False;80;INT;0;False;77;INT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;83;-161.8089,-82.04681;Inherit;False;Property;_EmissionStrengthMultiplier;Emission Strength Multiplier;13;0;Create;True;0;0;0;False;0;False;1;0;1;200;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;172;-529.5178,545.8111;Inherit;False;VRSL-GetRGBValues;-1;;352;6ff3fb2f25dfde442a3d454ce5bfa464;0;4;53;SAMPLER2D;0;False;4;INT;0;False;6;INT;0;False;7;INT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;190;204.0121,-175.8546;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;181;-604.018,136.0169;Inherit;True;VRSL-GetStrobeValue;-1;;356;67bfbade731e9bf479d532da3afed0e6;0;6;43;SAMPLER2D;0;False;31;INT;4;False;39;INT;0;False;17;INT;0;False;1;INT;0;False;4;SAMPLER2D;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerStateNode;7;-528.9083,-482.0835;Inherit;False;0;0;0;1;-1;None;1;0;SAMPLER2D;;False;1;SAMPLERSTATE;0
Node;AmplifyShaderEditor.CommentaryNode;80;-210.2069,120.7118;Inherit;False;132;132;CH 5;1;45;Strobe value;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;61;37.43338,-391.7389;Inherit;False;InstancedProperty;_GlobalIntensity;Global Intensity;11;0;Create;False;0;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;34.03101,433.2328;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;49;30.46555,-259.1884;Inherit;False;132;132;Emission Mask;1;48;;1,1,1,1;0;0
Node;AmplifyShaderEditor.WireNode;45;-160.2069,170.7118;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;12;-267.2777,-377.7147;Inherit;True;Property;_EmissionMask;Emission Mask;6;1;[SingleLineTexture];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;84;299.1382,-356.8202;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;183;250.6088,-543.0911;Inherit;False;InstancedProperty;_FinalIntensity;Final Intensity;12;0;Create;False;0;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;48;80.46545,-209.1885;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;44;293.0238,125.7325;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;184;456.4612,-613.6317;Inherit;False;Property;_UniversalIntensity;Universal Intensity;10;0;Create;False;0;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;182;393.3136,-467.1724;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;10;-263.1878,-752.1567;Inherit;True;Property;_MetallicSmoothnessMap;Metallic Smoothness Map;3;1;[SingleLineTexture];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;63;68.82088,-772.7103;Inherit;False;Property;_MetallicStrength;Metallic Strength;14;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;67;32.24033,-467.6367;Inherit;False;Property;_AmbientOcclusionStrength;Ambient Occlusion Strength;16;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;185;550.4612,-492.6317;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;47;397.5565,-38.39049;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;65;55.59845,-663.7995;Inherit;False;Property;_SmoothnessStrength;Smoothness Strength;15;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;11;-266.9187,-564.269;Inherit;True;Property;_AmbientOcclusionMap;Ambient Occlusion Map;5;1;[SingleLineTexture];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;66;333.8387,-688.778;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;64;331.0926,-827.6705;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;9;-265.6089,-937.0826;Inherit;True;Property;_NormalMap;Normal Map;0;2;[Normal];[SingleLineTexture];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;bump;Auto;True;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;6;-264.2169,-1126.978;Inherit;True;Property;_AlbedoMap;Albedo Map;1;1;[SingleLineTexture];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;70;80.69359,-591.0894;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;62;567.209,-362.4876;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;740.6802,-879.0976;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;VRSL/Amplify/VRSL-AmplifySurface;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;True;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.CommentaryNode;79;-81.01895,582.2235;Inherit;False;132;132;CH 2, 3, 4;0;RGB Value;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;43;-153.1387,365.0143;Inherit;False;132;132;CH 1;0;Intensity/Dimmer Value;1,1,1,1;0;0
WireConnection;173;96;176;0
WireConnection;173;80;21;0
WireConnection;173;77;16;0
WireConnection;172;53;176;0
WireConnection;172;4;16;0
WireConnection;172;6;21;0
WireConnection;172;7;163;0
WireConnection;190;1;83;0
WireConnection;190;2;173;0
WireConnection;181;43;176;0
WireConnection;181;39;175;0
WireConnection;181;17;21;0
WireConnection;181;1;16;0
WireConnection;181;4;177;0
WireConnection;39;0;173;0
WireConnection;39;1;172;0
WireConnection;45;0;181;0
WireConnection;12;7;7;0
WireConnection;84;0;61;0
WireConnection;84;1;190;0
WireConnection;48;0;12;1
WireConnection;44;0;45;0
WireConnection;44;1;39;0
WireConnection;182;0;183;0
WireConnection;182;1;84;0
WireConnection;10;7;7;0
WireConnection;185;0;184;0
WireConnection;185;1;182;0
WireConnection;47;0;48;0
WireConnection;47;1;44;0
WireConnection;11;7;7;0
WireConnection;66;0;10;4
WireConnection;66;1;65;0
WireConnection;64;0;10;1
WireConnection;64;1;63;0
WireConnection;9;7;7;0
WireConnection;6;7;7;0
WireConnection;70;0;11;1
WireConnection;70;2;67;0
WireConnection;62;0;47;0
WireConnection;62;1;185;0
WireConnection;0;0;6;0
WireConnection;0;1;9;0
WireConnection;0;2;62;0
WireConnection;0;3;64;0
WireConnection;0;4;66;0
WireConnection;0;5;70;0
ASEEND*/
//CHKSM=71E9F27070FDDF8DD27236D124D7D95BA55D58C0