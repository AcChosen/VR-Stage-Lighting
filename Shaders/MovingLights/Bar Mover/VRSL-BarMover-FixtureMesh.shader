Shader "VRSL/Bar Mover/Fixture"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _MetallicMap("Metallic Map", 2D) = "black" {}
        _EmissionMask("Emission Mask", 2D) = "black" {}
        _BumpMap ("Bumpmap", 2D) = "bump" {}
        [HideInInspector]_DMXChannel ("Starting Channel", Int) = 0
		 [HideInInspector][Toggle] _PanInvert ("Invert Mover Pan", Int) = 0

		 //[HideInInspector]_FinalStrobeFreq ("Final Strobe Frequency", Float) = 0
		 //[HideInInspector]_NewTimer("New Timer From Udon For Strobe", Float) = 0
		 [Toggle] _EnableCompatibilityMode ("Enable Compatibility Mode", Int) = 0
		 [Toggle] _EnableVerticalMode ("Enable Vertical Mode", Int) = 0
		 [Toggle] _EnableStrobe ("Enable Strobe", Int) = 0
		 [Toggle] _EnableOSC ("Enable Stream OSC/DMX Control", Int) = 0
		 [Toggle] _LegacyGoboRange ("Enable Legacy GOBO Range", Int) = 0
		 _FixtureRotationX("Mover Tilt Offset (Blue)", Range(-94,4)) = 0
         _FinalIntensity("Final Intensity", Range(0,1)) = 1
		_GlobalIntensity("Global Intensity", Range(0,1)) = 1
		_UniversalIntensity ("Universal Intensity", Range (0,1)) = 1
		[HDR]_Emission("Light Color Tint", Color) = (1,1,1,1)
       [HDR] _Emission1("Light Color Tint 1", Color) = (1,1,1,1)
        _EStart1("Start Range For Color 1", Range(0,1)) = 0
        [HDR]_Emission2("Light Color Tint 2", Color) = (1,1,1,1)
        _EStart2("Start Range For Color 2", Range(0,1)) = 0
        [HDR]_Emission3("Light Color Tint 3", Color) = (1,1,1,1)
        _EStart3("Start Range For Color 3", Range(0,1)) = 0
        [HDR]_Emission4("Light Color Tint 4", Color) = (1,1,1,1)
        _EStart4("Start Range For Color 4", Range(0,1)) = 0
       [HDR] _Emission5("Light Color Tint 5", Color) = (1,1,1,1)
        _EStart5("Start Range For Color 5", Range(0,1)) = 0
       [HDR] _Emission6("Light Color Tint 6", Color) = (1,1,1,1)
        _EStart6("Start Range For Color 6", Range(0,1)) = 0
       [HDR] _Emission7("Light Color Tint 7", Color) = (1,1,1,1)
        _EStart7("Start Range For Color 7", Range(0,1)) = 0
       [HDR] _Emission8("Light Color Tint 8", Color) = (1,1,1,1)
        _EStart8("Start Range For Color 8", Range(0,1)) = 0
      [HDR]  _Emission9("Light Color Tint 9", Color) = (1,1,1,1)
        _EStart9("Start Range For Color 9", Range(0,1)) = 0
       [HDR] _Emission10("Light Color Tint 10", Color) = (1,1,1,1)
        _Offset("Mix Amount Between Colors", Range(0,0.1)) = 0.01
        _LensMaxBrightness("Lens Max Brightness", Range(0.01, 20)) = 5
        _FixutreIntensityMultiplier ("Intensity Multipler (For Bloom Scaling)", Range(0,500)) = 1
        _FixtureRotationOrigin("Fixture Pivot Origin", Float) = (0, 0.014709, -1.02868, 0)
		[Toggle] _UseRawGrid("Use Raw Grid For Light Intensity And Color", Int) = 0
		[NoScaleOffset] _OSCGridRenderTextureRAW("OSC Grid Render Texture (RAW Unsmoothed)", 2D) = "white" {}
		[NoScaleOffset] _OSCGridRenderTexture("OSC Grid Render Texture (To Control Lights)", 2D) = "white" {}
		[NoScaleOffset] _OSCGridStrobeTimer ("OSC Grid Render Texture (For Strobe Timings", 2D) = "white" {}
		_MaxMinPanAngle("Max/Min Pan Angle (-x, x)", Float) = 180
		_MaxMinTiltAngle("Max/Min Tilt Angle (-y, y)", Float) = 180
		_FixtureMaxIntensity ("Maximum Cone Intensity",Range (0,0.5)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex, _MetallicMap, _BumpMap, _EmissionMask;
        sampler2D _OSCGridRenderTexture, _OSCGridRenderTextureRAW, _OSCGridStrobeTimer;
        uniform float4 _OSCGridRenderTextureRAW_TexelSize;
        float4 _FixtureRotationOrigin;
        float _FixtureMaxIntensity, _FixutreIntensityMultiplier;
        float _MaxMinPanAngle;
        float _MaxMinTiltAngle;
         uint _EnableCompatibilityMode, _EnableVerticalMode;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_BumpMap;
            float2 uv_MetallicMap;
            float2 uv_EmissionMask;
        };
        fixed4 _Color;
        float4 _Emission1, _Emission2, _Emission3, _Emission4, _Emission5,
        _Emission6, _Emission7, _Emission8, _Emission9, _Emission10;
        half _EStart1, _EStart2, _EStart3, _EStart4, _EStart5, _EStart6, _EStart7, _EStart8, _EStart9, _Offset;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        #pragma instancing_options assumeuniformscaling
    UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_DEFINE_INSTANCED_PROP(uint, _DMXChannel)
        UNITY_DEFINE_INSTANCED_PROP(uint, _PanInvert)
        UNITY_DEFINE_INSTANCED_PROP(uint, _TiltInvert)
        UNITY_DEFINE_INSTANCED_PROP(uint, _EnableOSC)
        UNITY_DEFINE_INSTANCED_PROP(uint, _EnableStrobe)
        UNITY_DEFINE_INSTANCED_PROP(uint, _EnableSpin)
        UNITY_DEFINE_INSTANCED_PROP(float, _StrobeFreq)
        UNITY_DEFINE_INSTANCED_PROP(float, _FixtureRotationX)
        UNITY_DEFINE_INSTANCED_PROP(float, _FixtureBaseRotationY)
        UNITY_DEFINE_INSTANCED_PROP(uint, _ProjectionSelection)
        UNITY_DEFINE_INSTANCED_PROP(float4, _Emission)
        UNITY_DEFINE_INSTANCED_PROP(float, _ConeWidth)
        UNITY_DEFINE_INSTANCED_PROP(float, _ConeLength)
        UNITY_DEFINE_INSTANCED_PROP(float, _GlobalIntensity)
        UNITY_DEFINE_INSTANCED_PROP(float, _FinalIntensity)
        UNITY_DEFINE_INSTANCED_PROP(float, _MaxConeLength)
        UNITY_DEFINE_INSTANCED_PROP(uint, _LegacyGoboRange)
    UNITY_INSTANCING_BUFFER_END(Props)
    #include "../../Shared/VRSL-DMXFunctions.cginc"

    float4 calculateRotations(appdata_full v, float4 input, int normalsCheck, float pan, float tilt)
    {
        
        //LOCALROTY IS NEW ROTATION
        //CALCULATE FIXTURE ROTATION. WOO FUN MATH. THIS IS FOR TILT.
        //set new origin to do transform
        float c, s;
        float3 newOrigin = input.w * _FixtureRotationOrigin.xyz;
        //if input.w is 1 (vertex), origin changes
        //if input.w is 0 (normal/tangent), origin doesn't change

        //subtract new origin from original origin for blue vertexes
        input.xyz = v.color.b == 1.0 ? input.xyz - newOrigin : input.xyz;
        //DO ROTATION

        //#if defined(PROJECTION_YES)
        //buffer[3] = GetTiltValue(sector);
        //#endif
        float angleX = radians(getOffsetX() + tilt);
        sincos(angleX, s, c);
        float3x3 rotateXMatrix = float3x3(1, 0, 0,
                                        0, c, -s,
                                        0, s, c);
            
        //float4 fixtureVertexPos = input;		
        //INVERSION CHECK
        rotateXMatrix = checkTiltInvertZ() == 1 ? transpose(rotateXMatrix) : rotateXMatrix;

        //float4 localRotX = mul(rotateXMatrix, fixtureVertexPos);
        //LOCALROTX IS NEW ROTATION

        //COMBINED ROTATION FOR FIXTURE

        //float3x3 rotateXYMatrix = mul(rotateYMatrix, rotateXMatrix);
        float3 localRotXY = mul(rotateXMatrix, input.xyz);
        //LOCALROTXY IS COMBINED ROTATION

        //Apply fixture rotation ONLY to those with blue vertex colors

        //apply LocalRotXY rotation then add back old origin
        input.xyz = v.color.b == 1.0 ? localRotXY : input.xyz;
        input.xyz = v.color.b == 1.0 ? input.xyz + newOrigin : input.xyz;
        
        //appy LocalRotY rotation to lightfixture base;
        //input.xyz = v.color.g == 1.0 ? localRotY : input.xyz;

        return input;
    }

    float4 testEmission(float2 uv)
    {
        float4 black = float4(0,0,0,0);
        float offset = _Offset;
        float4 result = lerp(_Emission1, _Emission2, smoothstep(_EStart1,_EStart1 + offset , uv.x));
        result = lerp(result, _Emission3, smoothstep(_EStart2,_EStart2 + offset ,uv.x));
        result = lerp(result, _Emission4, smoothstep(_EStart3,_EStart3 + offset ,uv.x));
        result = lerp(result, _Emission5, smoothstep(_EStart4,_EStart4 + offset ,uv.x));
        result = lerp(result, _Emission6, smoothstep(_EStart5,_EStart5 + offset ,uv.x));
        result = lerp(result, _Emission7, smoothstep(_EStart6,_EStart6 + offset ,uv.x));
        result = lerp(result, _Emission8, smoothstep(_EStart7,_EStart7 + offset ,uv.x));
        result = lerp(result, _Emission9, smoothstep(_EStart8,_EStart8 + offset ,uv.x));
        result = lerp(result, _Emission10, smoothstep(_EStart9,_EStart9 + offset,uv.x));
        // result = lerp(result, _Emission9, smoothstep(_EStart8,_EStart9 ,uv.x));
        // result = lerp(result, _Emission10, smoothstep(_EStart9, 1.0 ,uv.x));
        return result;
    }

        void vert (inout appdata_full v) 
        {
          //v.vertex.xyz += v.normal * _Amount;
          v.vertex = calculateRotations(v, v.vertex, 0, 0.0, getOffsetX());
            float4 newNormals = float4(v.normal.x, v.normal.y, v.normal.z, 0);
            newNormals = calculateRotations(v, newNormals, 1, 0.0, getOffsetX());
            v.normal = newNormals.xyz;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            float4 mask =  tex2D (_EmissionMask, IN.uv_EmissionMask).r;
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color *(1 - mask);
            o.Albedo = c.rgb;
            o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));
            
            // Metallic and smoothness come from slider variables
            o.Metallic = tex2D (_MetallicMap, IN.uv_MetallicMap).r * (1 - mask);
            o.Smoothness = tex2D (_MetallicMap, IN.uv_MetallicMap).a * (1 - mask);
            o.Emission = mask * testEmission(IN.uv_EmissionMask) * _FixutreIntensityMultiplier;
            o.Alpha = c.a;
        }
        ENDCG
    }
    //FallBack "Diffuse"
}
