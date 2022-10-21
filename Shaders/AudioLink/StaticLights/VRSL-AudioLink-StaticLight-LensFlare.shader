﻿Shader "VRSL/AudioLink/Standard Static/Lens Flare"
{
    //Modified from https://github.com/ColinLeung-NiloCat/UnityURP-BillboardLensFlareShader by ColinLeung-NiloCat from URP to SRP
    Properties
    {
        [Header(Audio Section)]
         [Toggle]_EnableAudioLink("Enable Audio Link", Float) = 0
         _Band("Band", Float) = 0
         _BandMultiplier("Band Multiplier", Range(1, 15)) = 1
         _Delay("Delay", Float) = 0
         _NumBands("Num Bands", Float) = 4
         _AudioSpectrum("AudioSpectrum", 2D) = "black" {}
        [Toggle] _EnableColorChord ("Enable Color Chord Tinting", Int) = 0
        _FinalIntensity("Final Intensity", Range(0,1)) = 1
        _GlobalIntensity("Global Intensity", Range(0,1)) = 1
        _UniversalIntensity ("Universal Intensity", Range (0,1)) = 1
        [HDR]_Emission("Light Color Tint", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _FadeAmt ("Fade Strength", Range(0,1)) = 1
        _ColorSat ("Color Saturtation Strength", Range(0,1)) = 1
        _ScaleFactor ("Scale Factor", Range(0,0.01)) = 1
        _ReferenceDistance("Reference Distance", Float) = 2
        _UVScale ("UV Scale Test", Range(0.001,2)) = 1
        _FixtureMaxIntensity ("Maximum Light Intensity",Range (0,15)) = 1
        _CurveMod ("Light Intensity Curve Modifier", Range (-3,8)) = 5.0
        _FixutreIntensityMultiplier ("Intensity Multipler (For Bloom Scaling)", Range(1,5)) = 1

        _RemoveTextureArtifact("RemoveTextureArtifact", Range(0,0.1)) = 0

        [Toggle] _EnableColorTextureSample ("Enable Color Texture Sampling", Int) = 0
		 _SamplingTexture ("Texture To Sample From for Color", 2D) = "white" {}
		 _TextureColorSampleX ("X coordinate to sample the texture from", Range(0,1)) = 0.5
		 _TextureColorSampleY ("Y coordinate to sample the texture from", Range(0,1)) = 0.5

        [Header(PreMultiply Alpha. Turn it ON only if your texture has correct alpha)]
        [Toggle]_UsePreMultiplyAlpha("UsePreMultiplyAlpha (recommend _BaseMap's alpha = 'From Gray Scale')", Float) = 0


        [Header(Depth Occlusion)]
        _LightSourceViewSpaceRadius("LightSourceViewSpaceRadius", range(0,1)) = 0.05
        _DepthOcclusionTestZBias("DepthOcclusionTestZBias", range(-1,1)) = -0.001

        [Header(If camera too close Auto fadeout)]
        _StartFadeinDistanceWorldUnit("StartFadeinDistanceWorldUnit",Float) = 0.05
        _EndFadeinDistanceWorldUnit("EndFadeinDistanceWorldUnit", Float) = 0.5

        // [Header(Optional Flicker animation)]
        // [Toggle]_ShouldDoFlicker("ShouldDoFlicker", FLoat) = 1
        // _FlickerAnimSpeed("FlickerAnimSpeed", Float) = 5
        // _FlickResultIntensityLowestPoint("FlickResultIntensityLowestPoint", range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent" }
        LOD 100

        Pass
        {
            Zwrite Off
            ZTest Off
            Blend One One
            Cull Off

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
                half4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float4 screenPos : TEXCOORD1;
                float4 worldDirection : TEXCOORD2;
                float4 vertexWorldPos : TEXCOORD3;
                half4 color : TEXCOORD4;
                float maskX : TEXCOORD5;

                UNITY_VERTEX_INPUT_INSTANCE_ID  // will turn into this in non OpenGL / non PSSL -> uint instanceID : SV_InstanceID;
                UNITY_VERTEX_OUTPUT_STEREO 

            };

            #define COUNT 8 //you can edit to any number(e.g. 1~32), the lower the faster. Keeping this number a const can enable many compiler optimizations

           // sampler2D _MainTex;
            //float4 _MainTex_ST;
            //half4 _Emission;
            half _ColorSat, _ScaleFactor, _ReferenceDistance, _UVScale;
            float _LightSourceViewSpaceRadius;
            float _DepthOcclusionTestZBias;
            
            float _StartFadeinDistanceWorldUnit;
            float _EndFadeinDistanceWorldUnit;

            float _UsePreMultiplyAlpha;

          //  float _FlickerAnimSpeed;
            float _FlickResultIntensityLowestPoint;
            //float _ShouldDoFlicker;
             half _RemoveTextureArtifact, _CurveMod;
            #include "../Shared/VRSL-AudioLink-Defines.cginc"
            #include "../Shared/VRSL-AudioLink-Functions.cginc"

            float4x4 GetWorldToViewMatrix()
            {
                return UNITY_MATRIX_V;
            }
            float4x4 GetObjectToWorldMatrix()
            {
                return UNITY_MATRIX_M;
            }

            float3 TransformWorldToView(float3 positionWS)
            {
                return mul(GetWorldToViewMatrix(), float4(positionWS, 1.0)).xyz;
            }

            float3 TransformObjectToWorld(float3 vertex)
            {
                return mul(GetObjectToWorldMatrix(), float4(vertex, 1.0)).xyz;
            }

            inline float4 CalculateFrustumCorrection()
            {
                float x1 = -UNITY_MATRIX_P._31/(UNITY_MATRIX_P._11*UNITY_MATRIX_P._34);
                float x2 = -UNITY_MATRIX_P._32/(UNITY_MATRIX_P._22*UNITY_MATRIX_P._34);
                return float4(x1, x2, 0, UNITY_MATRIX_P._33/UNITY_MATRIX_P._34 + x1*UNITY_MATRIX_P._13 + x2*UNITY_MATRIX_P._23);
            }

            //CREDIT TO DJ LUKIS FOR MIRROR DEPTH CORRECTION
            inline float CorrectedLinearEyeDepth(float z, float B)
            {
                return 1.0 / (z/UNITY_MATRIX_P._34 + B);
            }

            float3 RGB2HSV(float3 c)
            {
                float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
                float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));

                float d = q.x - min(q.w, q.y);
                float e = 1.0e-10;
                return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
            }

            float3 hsb2rgb( float3 c ){
                float3 rgb = clamp( abs(fmod(c.x*6.0+float3(0.0,4.0,2.0),6)-3.0)-1.0, 0, 1);
                rgb = rgb*rgb*(3.0-2.0*rgb);
                return c.z * lerp( float3(1,1,1), rgb, c.y);
            }
            // float4 getEmissionColor()
            // {
            //     return UNITY_ACCESS_INSTANCED_PROP(Props,_Emission);
            // }
            // float getGlobalIntensity()
            // {
            //     return UNITY_ACCESS_INSTANCED_PROP(Props, _GlobalIntensity);
            // }

            // float getFinalIntensity()
            // {
            //     return UNITY_ACCESS_INSTANCED_PROP(Props, _FinalIntensity);
            // }

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v); //Insert
                UNITY_INITIALIZE_OUTPUT(v2f, o); //Insert
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); //Insert

                half4 e = getEmissionColor();
                e = clamp(e, half4(0,0,0,1), half4(_FixtureMaxIntensity*2,_FixtureMaxIntensity*2,_FixtureMaxIntensity*2,1));
                e*= (_FixutreIntensityMultiplier);
                e*= GetAudioReactAmplitude();
                e = float4(((e.rgb * _FixtureMaxIntensity) * getGlobalIntensity()) * getFinalIntensity(), e.w);
                e*= _UniversalIntensity;
                float3 eHSV = RGB2HSV(e.rgb);
                if(eHSV.z <= 0.01)
                {
                    v.vertex = float4(0,0,0,0);
			        o.vertex = UnityObjectToClipPos(v.vertex);
                    return o;

                }

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color * e;

                // float2 scaleXY_WS = float2(
                // length(float3(unity_ObjectToWorld[0].x, unity_ObjectToWorld[1].x, unity_ObjectToWorld[2].x)), // scale x axis
                // length(float3(unity_ObjectToWorld[0].y, unity_ObjectToWorld[1].y, unity_ObjectToWorld[2].y)) // scale y axis  // scale z axis
                // );

                // float4 world_origin = mul(UNITY_MATRIX_M, float4(0,0,0,1));
                // float4 view_origin = float4(UnityObjectToViewPos(float3(0,0,0)),1);

                // float scale = (_ScaleFactor * (length(view_origin)/_ReferenceDistance)) + (1 - _ScaleFactor);



                // float4 worldPos = mul(UNITY_MATRIX_M, float4(scale, scale, scale, 1) * v.vertex);
                // float4 flippedWorldPos = float4(-1,1,-1,1) * (worldPos - world_origin) + world_origin;
                // //float4 viewPos = mul(UNITY_MATRIX_V, worldPos);
                // float4 viewPos = flippedWorldPos - world_origin + view_origin;
                // viewPos = float4(viewPos.x, viewPos.y, viewPos.z, viewPos.w);
                // float4 clipPos = mul(UNITY_MATRIX_P, viewPos);
                // o.vertex = clipPos;
                float3 quadPivotPosOS = float3(0,0,0);
                float3 quadPivotPosWS = TransformObjectToWorld(quadPivotPosOS);
                float3 quadPivotPosVS = TransformWorldToView(quadPivotPosWS);

                //get transform.lossyScale using:
                //https://forum.unity.com/threads/can-i-get-the-scale-in-the-transform-of-the-object-i-attach-a-shader-to-if-so-how.418345/
                float2 scaleXY_WS = float2(
                    length(float3(GetObjectToWorldMatrix()[0].x, GetObjectToWorldMatrix()[1].x, GetObjectToWorldMatrix()[2].x)), // scale x axis
                    length(float3(GetObjectToWorldMatrix()[0].y, GetObjectToWorldMatrix()[1].y, GetObjectToWorldMatrix()[2].y)) // scale y axis
                    );

                float3 posVS = quadPivotPosVS + float3(v.vertex.xy * scaleXY_WS,0);//recontruct quad 4 points in view space

                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //complete SV_POSITION's view space to HClip space transformation
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                o.vertex = mul(UNITY_MATRIX_P,float4(posVS,1));

                
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //do smooth visibility test using brute force forloop (COUNT*2+1)^2 times inside a view space 2D grid area
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                float visibilityTestPassedCount = 0;
                float linearEyeDepthOfFlarePivot = -quadPivotPosVS.z;//view space's forward is pointing to -Z, but we want +Z, so negate it
                float testLoopSingleAxisWidth = COUNT*2+1;
                float totalTestCount = testLoopSingleAxisWidth * testLoopSingleAxisWidth;
                float divider = 1.0 / totalTestCount;
                float maxSingleAxisOffset = _LightSourceViewSpaceRadius / testLoopSingleAxisWidth;

                //Test for n*n grid in view space, where quad pivot is grid's center.
                //For each iteration,
                //if that test point passed the scene depth occlusion test, we add 1 to visibilityTestPassedCount
                for(int x = -COUNT; x <= COUNT; x++)
                {
                    for(int y = -COUNT; y <= COUNT ; y++)
                    {
                        float3 testPosVS = quadPivotPosVS;
                        testPosVS.xy += float2(x,y) * maxSingleAxisOffset;//add 2D test grid offset, in const view space unit
                        float4 PivotPosCS = mul(UNITY_MATRIX_P,float4(testPosVS,1));
                        float4 PivotScreenPos = ComputeScreenPos(PivotPosCS);
                        float2 screenUV = PivotScreenPos.xy/PivotScreenPos.w;

                        //if screenUV out of bound, treat it as occluded, because no correct depth texture data can be used to compare
                        if(screenUV.x > 1 || screenUV.x < 0 || screenUV.y > 1 || screenUV.y < 0)
                            continue; //exit means occluded

                        //we don't have tex2D() in vertex shader, because rasterization is not done by GPU, so we use tex2Dlod() with mip0 instead
                        float4 ssd = SAMPLE_DEPTH_TEXTURE_LOD(_CameraDepthTexture, float4(screenUV, 0.0, 0.0));//(uv.x,uv.y,0,mipLevel)
                        float sampledSceneDepth = ssd.x;
                        float linearEyeDepthFromSceneDepthTexture = LinearEyeDepth(sampledSceneDepth);
                        float linearEyeDepthFromSelfALU = PivotPosCS.w; //clip space .w is view space z, = linear eye depth

                        //do the actual depth comparision test
                        //+1 means flare test point is visible in screen space
                        //+0 means flare test point blocked by other objects in screen space, not visible
                        visibilityTestPassedCount += linearEyeDepthFromSelfALU + _DepthOcclusionTestZBias < linearEyeDepthFromSceneDepthTexture ? 1 : 0; 
                    }
                }
                float visibilityResult01 = visibilityTestPassedCount * divider;//0~100% visiblility result 

                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //if camera too close to flare , smooth fade out to prevent flare blocking camera too much (usually for fps games)
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                visibilityResult01 *= smoothstep(_StartFadeinDistanceWorldUnit,_EndFadeinDistanceWorldUnit,linearEyeDepthOfFlarePivot);

                // if(_ShouldDoFlicker)
                // {
                //     float flickerMul = 0;
                //     //TODO: expose more control to noise? (send me an issue in GitHub, if anyone need this)
                //     flickerMul += saturate(sin(_Time.y * _FlickerAnimSpeed * 1.0000)) * (1-_FlickResultIntensityLowestPoint) + _FlickResultIntensityLowestPoint;
                //     flickerMul += saturate(sin(_Time.y * _FlickerAnimSpeed * 0.6437)) * (1-_FlickResultIntensityLowestPoint) + _FlickResultIntensityLowestPoint;   
                //     visibilityResult01 *= saturate(flickerMul/2);
                // }
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //apply all combinations(visibilityResult01) to vertex color
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                o.color.a *= visibilityResult01;

                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //premultiply alpha to rgb after alpha's calculation is done
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
                o.color.rgb *= o.color.a;                 
                o.color.a = _UsePreMultiplyAlpha? o.color.a : 0;

                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                //pure optimization:
                //if flare is invisible or nearly invisible,
                //invalid this vertex (and all connected vertices).
                //This 100% early exit at clipping stage will prevent any rasterization & fragment shader cost at all
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                o.vertex = visibilityResult01 < divider ? 0 : o.vertex;

                
                float3 hsvFC = RGB2HSV(o.color.xyz);
                hsvFC.y = 0.0;
                float4 e2 = float4(1,1,1,o.color.w) * hsvFC.z;
                o.maskX = lerp(1, 0, pow(distance(half2(0.5, 0.5), o.uv), _FadeAmt));
                float satMask = lerp(1, 0, pow(distance(half2(0.5, 0.5), o.uv), _ColorSat));
                o.color = lerp(o.color, e2, satMask);
                



                
              //  UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }





            fixed4 frag (v2f i) : SV_Target
            {
                // float2 screenposUV = i.screenPos.xy / i.screenPos.w;
                //     //CREDIT TO DJ LUKIS FOR MIRROR DEPTH CORRECTION
                // float perspectiveDivide = 1.0f / i.vertex.w;
                // float4 depthdirect = i.worldDirection * perspectiveDivide;
                // float sceneZ = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, screenposUV);
                // #if UNITY_REVERSED_Z
                //     if (sceneZ == 0)
                // #else
                //     if (sceneZ == 1)
                // #endif
                // return float4(0,0,0,1);
                // float depth = CorrectedLinearEyeDepth(sceneZ, depthdirect.w);
                // //Convert from Corrected Linear Eye Depth to Linear01Depth
                // //Credit: https://www.cyanilux.com/tutorials/depth/#eye-depth
                // depth = (1.0 - (depth * _ZBufferParams.w)) / (depth * _ZBufferParams.z);
                // depth = Linear01Depth(depth);
                fixed4 col = saturate(tex2D(_MainTex, i.uv )-_RemoveTextureArtifact) * i.color;
               // float2 p = i.uv * 2.0 - 1.0;
               // col *= (lerp(1, 0, abs(p.x))) * (lerp(1, 0, abs(p.y)));
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                
                col *= i.maskX;
                return col;
            }
            ENDCG
        }
    }
}
