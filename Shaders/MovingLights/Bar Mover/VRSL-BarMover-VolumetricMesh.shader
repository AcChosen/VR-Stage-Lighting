Shader "VRSL/Bar Mover/Volumetric"
{
    Properties
    {
        _MainTex ("Mask Texture", 2D) = "white" {}
        _gRayTex ("Mask 2 Texture", 2D) = "white" {}
        _RayTexture ("God Ray Texture", 2D) = "white" {}
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
        [Space(20)]
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

        [Space(20)]
        _LensMaxBrightness("Lens Max Brightness", Range(0.01, 20)) = 5
        _FixutreIntensityMultiplier ("Intensity Multipler (For Bloom Scaling)", Range(0,500)) = 1
        _FixtureRotationOrigin("Fixture Pivot Origin", Float) = (0, 0.014709, -1.02868, 0)
        _FixtureLensCenter("Fixture Lens Center", Float) = (-0.001864, 0.258346, -0.159662, 0)
		[Toggle] _UseRawGrid("Use Raw Grid For Light Intensity And Color", Int) = 0
		[NoScaleOffset] _OSCGridRenderTextureRAW("OSC Grid Render Texture (RAW Unsmoothed)", 2D) = "white" {}
		[NoScaleOffset] _OSCGridRenderTexture("OSC Grid Render Texture (To Control Lights)", 2D) = "white" {}
		[NoScaleOffset] _OSCGridStrobeTimer ("OSC Grid Render Texture (For Strobe Timings", 2D) = "white" {}
		_MaxMinPanAngle("Max/Min Pan Angle (-x, x)", Float) = 180
		_MaxMinTiltAngle("Max/Min Tilt Angle (-y, y)", Float) = 180
		_FixtureMaxIntensity ("Maximum Cone Intensity",Range (0,0.5)) = 0.5
        _InnerIntensityCurve("Inner Intensity Curve", Range(0.00001,5)) = 2.25
        _CamAngleFadeStrength("Camera Angle Fade Strength", Range(0.00001,1)) = 0.1
        _FadeStrength("Edge Fade", Range(0.00001,10)) = 1
		_InnerFadeStrength("Inner Fade Strength", Range(0.00001,10)) = 0
        _IntersectionFadeStrength("Intersection Fade Length", Range(0.00001, 10)) = 1
        _EdgeFadeCurve("Edge Fade Length", Range(0.00001, 0.1)) = 1
        _VolumectricCornerOutside("Volumetric Corner Outside", Range(0.0, 1)) = 1
        _VolumectricCornerInside("Volumetric Corner Inside", Range(0.0, 1)) = 1
    }
    SubShader
    {
        Tags{ "Queue" = "Transparent+2" "IgnoreProjector"="True" "RenderType" = "Transparent" }
        Blend One One
        Cull Off
        ZWrite Off
        Lighting Off
        Tags{ "LightMode" = "Always" }
        Stencil
        {
            Ref 148
            Comp NotEqual
            Pass Keep
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"


            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
                float4 color : COLOR;
                float3 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                centroid float2 uv : TEXCOORD0;
                centroid float2 uv2 : TEXCOORD1;
                UNITY_FOG_COORDS(1)
                float4 pos : SV_POSITION;
                float4 worldPos : TEXCOORD2;
                float4 screenPos : TEXCOORD3;
                float4 worldDirection : TEXCOORD4;
                float2 camAngleCamfade : TEXCOORD5;
                centroid float3 objNormal : TEXCOORD6;
                float4 color : TEXCOORD7;
              //  float4 lensCenter : TEXCOORD7;
                
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO 
            };
        UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
            sampler2D _MainTex, _gRayTex, _RayTexture;
            float4 _MainTex_ST, _gRayTex_ST;
                        float4 _Emission1, _Emission2, _Emission3, _Emission4, _Emission5,
        _Emission6, _Emission7, _Emission8, _Emission9, _Emission10;
        half _EStart1, _EStart2, _EStart3, _EStart4, _EStart5, _EStart6, _EStart7, _EStart8, _EStart9, _Offset;
        sampler2D _OSCGridRenderTexture, _OSCGridRenderTextureRAW, _OSCGridStrobeTimer;
        uniform float4 _OSCGridRenderTextureRAW_TexelSize;
        float4 _FixtureLensCenter;
        float4 _FixtureRotationOrigin;
        float _FixtureMaxIntensity, _FixutreIntensityMultiplier;
        float _MaxMinPanAngle, _UniversalIntensity;
        float _MaxMinTiltAngle;
         uint _EnableCompatibilityMode, _EnableVerticalMode;
        #pragma instancing_options assumeuniformscaling
        half _InnerFadeStrength, _FadeStrength, _InnerIntensityCurve, _CamAngleFadeStrength, _IntersectionFadeStrength, _EdgeFadeCurve, _VolumectricCornerOutside, _VolumectricCornerInside;
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
            result = lerp(result, _Emission9, smoothstep(_EStart8,_EStart9 ,uv.x));
            result = lerp(result, _Emission10, smoothstep(_EStart9, 1.0 ,uv.x));
            return saturate(result);
        }

        float4 calculateRotations(appdata v, float4 input, int normalsCheck, float pan, float tilt)
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
            inline float4 CalculateFrustumCorrection()
            {
                float x1 = -UNITY_MATRIX_P._31/(UNITY_MATRIX_P._11*UNITY_MATRIX_P._34);
                float x2 = -UNITY_MATRIX_P._32/(UNITY_MATRIX_P._22*UNITY_MATRIX_P._34);
                return float4(x1, x2, 0, UNITY_MATRIX_P._33/UNITY_MATRIX_P._34 + x1*UNITY_MATRIX_P._13 + x2*UNITY_MATRIX_P._23);
            }

            #define IF(a, b, c) lerp(b, c, step((fixed) (a), 0));

            //CREDIT TO DJ LUKIS FOR MIRROR DEPTH CORRECTION
            inline float CorrectedLinearEyeDepth(float z, float B)
            {
                #if UNITY_REVERSED_Z
                if (z == 0)
                    return LinearEyeDepth(z);
                #endif
                return 1.0 / (z/UNITY_MATRIX_P._34 + B);
            }
            float Fresnel(float3 Normal, float3 ViewDir, float Power, float exp)
            {
                return pow(max(0, dot(Normal, -ViewDir)), pow(Power, exp));
            }


            v2f vert (appdata v)
            {
                v2f o;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o); //DON'T INITIALIZE OR IT WILL BREAK PROJECTION
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                float3 wpos;
                wpos.x = unity_CameraToWorld[0][3];
                wpos.y = unity_CameraToWorld[1][3];
                wpos.z = unity_CameraToWorld[2][3];
                v.vertex = calculateRotations(v, v.vertex, 0, 0.0, getOffsetX());
                float4 newNormals = float4(v.normal.x, v.normal.y, v.normal.z, 0);
                newNormals = calculateRotations(v, newNormals, 1, 0.0, getOffsetX());
                v.normal = newNormals.xyz;
               // o.lensCenter = calculateRotations(v, _FixtureLensCenter, 0, 0.0, getOffsetX());
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv2 = v.uv2;
                o.screenPos = ComputeScreenPos (o.pos);
                //o.uv = UnityStereoScreenSpaceUVAdjust(uv, sb)
                COMPUTE_EYEDEPTH(o.screenPos.z);
                UNITY_TRANSFER_FOG(o,o.pos);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.worldDirection.xyz = o.worldPos.xyz - wpos;
                // pack correction factor into direction w component to save space
                float4 originScreenPos = ComputeScreenPos(UnityObjectToClipPos(_FixtureRotationOrigin));
		        float2 originScreenUV = originScreenPos.xy / originScreenPos.w;
                o.worldDirection.w = dot(o.pos, CalculateFrustumCorrection());
                o.camAngleCamfade.y = saturate(distance(wpos, o.worldPos) - 0.5);
                o.camAngleCamfade.x = saturate((1-distance(float2(0.5, 0.5), originScreenUV))-0.5);
                o.objNormal = normalize(v.normal);
                o.color = v.color;


                return o;
            }

            fixed4 frag (v2f i, fixed facePos : VFACE) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);

                float gi = getGlobalIntensity();
                float fi = getFinalIntensity();
                // if(((all(i.rgbColor <= float4(0.005,0.005,0.005,1)) || i.intensityStrobeGOBOSpinSpeed.x <= 0.005) && isOSC() == 1) || gi <= 0.005 || fi <= 0.005)
                // {
                //     return half4(0,0,0,0);
                // } 
                half2 uvMap = half2(0.0,0.0);

                //replacement for _WorldSpaceCameraPos
                float3 wpos;
                wpos.x = unity_CameraToWorld[0][3];
                wpos.y = unity_CameraToWorld[1][3];
                wpos.z = unity_CameraToWorld[2][3];

                	//float perspectiveDivide = 1.0f / i.pos.w;
                float4 direction = i.worldDirection * (1.0f / i.pos.w);
                float2 screenUV = i.screenPos.xy / i.screenPos.w;
                //Sampling Depth
                float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, screenUV);
                //Correct for mirrors as Eye Depth
                depth = CorrectedLinearEyeDepth(depth, direction.w);
                //Convert to Raw Depth
                depth = (1.0 - (depth * _ZBufferParams.w)) / (depth * _ZBufferParams.z);
                //Convert to Linear01 Deppth
                depth = Linear01Depth(depth);
                float intersectionFade;
                fixed gradientTexture = saturate((pow(-i.uv.x + 1, _InnerIntensityCurve)));
                fixed4 col = gradientTexture.r;
                
                //intersectionFade = lerp(1.0 , saturate(((depth * _ProjectionParams.z) - i.screenPos.w)), saturate(pow(i.uv.x, pow(gradientTexture, _IntersectionFadeStrength))));
                intersectionFade = saturate(((depth * _ProjectionParams.z) - i.screenPos.w));
                i.camAngleCamfade.y = lerp(1.0,saturate(distance(wpos, i.worldPos) - 0.5) * intersectionFade, saturate(pow(i.uv.x, pow(gradientTexture, _IntersectionFadeStrength))));
                // if(i.uv2.x < 0.5)
                // {
                    
                //    // 
                //     float cornerMask = tex2D(_MainTex, i.uv).r * 0.50;
                //     cornerMask = lerp(1.0, cornerMask, _VolumectricCornerInside);
                //     col *= i.camAngleCamfade.y * cornerMask;
                // }
                // else
                // {
                //     //intersectionFade = lerp(1.0 , saturate(((depth * _ProjectionParams.z) - i.screenPos.w)), saturate(pow(i.uv.x, 0.00001) + 0.1));
                    
                //     col *= i.camAngleCamfade.y  * tex2D(_gRayTex, i.uv).r;
                // }
                i.camAngleCamfade.y = lerp(1.0, i.camAngleCamfade.y, saturate(pow(i.uv.x, _CamAngleFadeStrength)+0.01));
                col*= getEmissionColor() * i.camAngleCamfade.y;
                //Attempt to fade cone away when intersecting with the camera.
                //float cameraFade = i.camAngleCamfade.y;

                //Fade the camera less when you are closer to the source of the light.
                
                //fixed gradientTexture = saturate((pow(-i.uv.x + 1, _InnerIntensityCurve)));
                float3 viewDir = (wpos - i.worldPos.xyz);
                viewDir = normalize(mul(unity_WorldToObject,float4(viewDir,0.0)));


                // //Initialize Edge Fade
                 float edgeFade = 0.0;
                 float cornerMask = tex2D(_MainTex, i.uv).r;
                 cornerMask = i.uv.x <= 0.00379 ? 1.0 : cornerMask;
                 float exp = lerp(_VolumectricCornerOutside, 0.0, tex2D(_gRayTex, i.uv).r);
                if(facePos > 0)
                {
                   edgeFade = Fresnel(-i.objNormal, viewDir, _InnerFadeStrength, exp);
                    
                    //cornerMask *= _VolumectricCornerInside;
                    //cornerMask = lerp(1.0, cornerMask, _VolumectricCornerInside);
                  //  threeDNoiseScale = _Noise2Stretch;
                }
                else
                {
                    edgeFade = Fresnel(i.objNormal, viewDir, _FadeStrength, exp);
                }
                // else
                // {
                //     edgeFade = Fresnel(i.objNormal, viewDir, _FadeStrength);
                // }
                //     //Reduce fade strength if closer to source of light
                //     float fadeSTR = lerp(1.0, _FadeStrength, saturate(pow(i.uv.x, 0.01)));
                //     edgeFade = Fresnel(i.objNormal, viewDir, fadeSTR);
                     cornerMask = lerp(1.0, cornerMask, _VolumectricCornerInside);
                //     //threeDNoiseScale = _Noise2StretchInside;
                // }
                col*= lerp(1.0, edgeFade, saturate(pow(i.uv.x, _EdgeFadeCurve)));
                //col *= edgeFade;
                // edgeFade = lerp(1.0, edgeFade, saturate(pow(i.uv.x,_EdgeFadeCurve) - 0.10));
                // if(i.uv2.x < 0.5)
                // {
                //     col *= intersectionFade * edgeFade * getEmissionColor() * i.camAngleCamfade.y;
                //     col *= cornerMask;
                // }
                // else
                // {
                //     col *= tex2D(_gRayTex, i.uv).r * intersectionFade * getEmissionColor() * i.camAngleCamfade.y ;
                // }

                // sample the texture
               // fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                float ray = i.color.r < 0.1 ? tex2D(_RayTexture, i.uv).r : 1.0;
                col *= ray;
                UNITY_APPLY_FOG(i.fogCoord, col);
                col *= cornerMask * 3.0;
                col *= gi * fi;
                col *= testEmission(i.uv2);
                return col * _UniversalIntensity;
            }
            ENDCG
        }
    }
}
