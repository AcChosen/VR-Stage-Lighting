Shader "VRSL/Basic Laser/AudioLink"
{
    Properties
    {
        //Color Texture Sampling Properties
        [Toggle] _EnableColorChord ("Enable Color Chord Tinting", Int) = 0
		 [Toggle] _EnableColorTextureSample ("Enable Color Texture Sampling", Int) = 0
		 _SamplingTexture ("Texture To Sample From for Color", 2D) = "white" {}
		 _TextureColorSampleX ("X coordinate to sample the texture from", Range(0,1)) = 0.5
		 _TextureColorSampleY ("Y coordinate to sample the texture from", Range(0,1)) = 0.5
         [Toggle]_EnableAudioLink("Enable Audio Link", Int) = 0
         _Band("AudioLink Band", Int) = 0
         _BandMultiplier("AudioLink Multiplier", Float) = 1.0
         _Delay("Audio Link Delay", Int) = 0
         _RenderTextureMultiplier("Render Texture Multiplier", Range(1,10)) = 1

        _UniversalIntensity ("Universal Intensity", Range (0,1)) = 1
        _FinalIntensity("Final Intensity", Range(0,1)) = 1
        _GlobalIntensity ("Global Intensity", Range(0,1)) = 1
        _GlobalIntensityBlend("Global Intensity Blend", Range(0,1)) = 1
        [HideInInspector]_MainTex ("Texture", 2D) = "white" {}
        [HDR]_Emission ("Emission Color" , Color) = (1.0, 1.0, 1.0, 1.0)
        _Multiplier("Emission Multiplier", Range(1,10)) = 1
        _VertexConeWidth ("Cone Width", Range(-3.75,20)) = 0
        _VertexConeLength("Cone Length", Range(-0.5,5)) = 0
        _ZConeFlatness("Z Flatness", Range(0,1.999)) = 0
        _ZConeFlatnessAlt("Z Flatness Alt", Range(0,1.999)) = 0
     //  _XConeFlatness("X Flatness", Range(0,1.99)) = 0
        _ZRotation ("Z Rotation", Range (-90, 90)) = 0
        _XRotation ("X Rotation", Range (-90, 90)) = 0
        _YRotation ("Y Rotation", Range (-180, 180)) = 0
        _AltZRotation ("Alt Z Rotation", Range (-90, 90)) = 0
        _AltXRotation ("Alt X Rotation", Range (-90, 90)) = 0
        _AltYRotation ("Alt Y Rotation", Range (-180, 180)) = 0
        [IntRange] _LaserCount ("Laser Beam Count", Range(4, 68)) = 1
        _LaserThickness ("Laser Beam Thickenss", Range(0.003, 0.25)) = 1
        _EndFade ("End Fade", Range(0,3)) = 2.2
        _FadeStrength ("Cone Edge Fade", Range(1,2)) = 0
        _LaserSoftening ("Laser Softness", Range(0.05,5)) = 5
        _InternalShine ("Internal Shine Strength", Range(0,5)) = 1
        _InternalShineLength ("Internal Shine Length", Range(0.001,500)) = 12.1
        _BlackOut ("Global Blackout Slider", Range (0,1)) = 1

        _Scroll ("Scroll", Range(-1, 1)) = 1

        [Toggle] _EnableThemeColorSampling ("Enable Theme Color Sampling", Int) = 0
        [Toggle]_UseTraditionalSampling("Use Traditional Texture Sampling", Int) = 0
		 _ThemeColorTarget ("Choose Theme Color", Int) = 0

    }
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent" "Queue" = "Transparent+4" "RenderingPipeline" = "UniversalPipeline"
        }
        Cull Off
        Blend One One
        Zwrite Off
        LOD 100

        Pass
        {

            Stencil
            {
                Ref 142
                Comp NotEqual
                Pass Keep
            }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #define VRSL_AUDIOLINK

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
                float3 normal : TEXCOORD2;
                UNITY_FOG_COORDS(9)
                float4 vertex : SV_POSITION;
                float4 worldPos : TEXCOORD3;
                float3 viewDir : TEXCOORD4;
                float4 panTiltLengthWidth : TEXCOORD5; //ch 1,2,3,4
                float4 flatnessBeamCountSpinThickness : TEXCOORD6; //ch 5,6,7,12
                float4 rgbIntensity : TEXCOORD7; // ch 8,9,10,11
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            sampler2D _MainTex, _SamplingTexture;
            float4 _MainTex_ST;
            half _XConeFlatness, _ZRotation, _UniversalIntensity;
            half _EndFade, _FadeStrength, _InternalShine, _LaserSoftening, _InternalShineLength, _Multiplier;
            uint _EnableCompatibilityMode;
            uniform const float compatSampleYAxis = 0.019231;
            uniform const float standardSampleYAxis = 0.00762;
            #include "Packages/com.llealloo.audiolink/Runtime/Shaders/AudioLink.cginc"


            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float, _EnableAudioLink)
                UNITY_DEFINE_INSTANCED_PROP(float, _EnableColorChord)
                UNITY_DEFINE_INSTANCED_PROP(float, _NumBands)
                UNITY_DEFINE_INSTANCED_PROP(float, _Band)
                UNITY_DEFINE_INSTANCED_PROP(float, _BandMultiplier)
                UNITY_DEFINE_INSTANCED_PROP(float, _Delay)
                UNITY_DEFINE_INSTANCED_PROP(uint, _LaserCount)
                UNITY_DEFINE_INSTANCED_PROP(uint, _EnableColorTextureSample)
                UNITY_DEFINE_INSTANCED_PROP(float, _Scroll)
                UNITY_DEFINE_INSTANCED_PROP(float, _XRotation)
                UNITY_DEFINE_INSTANCED_PROP(float, _YRotation)
                UNITY_DEFINE_INSTANCED_PROP(float, _AltZRotation)
                UNITY_DEFINE_INSTANCED_PROP(float, _AltXRotation)
                UNITY_DEFINE_INSTANCED_PROP(float, _AltYRotation)
                UNITY_DEFINE_INSTANCED_PROP(float, _LaserThickness)
                UNITY_DEFINE_INSTANCED_PROP(float4, _Emission)
                UNITY_DEFINE_INSTANCED_PROP(float, _ZConeFlatness)
                UNITY_DEFINE_INSTANCED_PROP(float, _ZConeFlatnessAlt)
                UNITY_DEFINE_INSTANCED_PROP(float, _VertexConeWidth)
                UNITY_DEFINE_INSTANCED_PROP(float, _VertexConeLength)
                UNITY_DEFINE_INSTANCED_PROP(float, _GlobalIntensity)
                UNITY_DEFINE_INSTANCED_PROP(float, _GlobalIntensityBlend)
                UNITY_DEFINE_INSTANCED_PROP(float, _FinalIntensity)
                UNITY_DEFINE_INSTANCED_PROP(float, _TextureColorSampleX)
                UNITY_DEFINE_INSTANCED_PROP(float, _TextureColorSampleY)
                UNITY_DEFINE_INSTANCED_PROP(float, _BlackOut)
                UNITY_DEFINE_INSTANCED_PROP(float, _ThemeColorTarget)
                UNITY_DEFINE_INSTANCED_PROP(uint, _EnableThemeColorSampling)
                UNITY_DEFINE_INSTANCED_PROP(uint, _UseTraditionalSampling)
            UNITY_INSTANCING_BUFFER_END(Props)

            inline float AudioLinkLerp3_g5(int Band, float Delay)
            {
                return AudioLinkLerp(ALPASS_AUDIOLINK + float2(Delay, Band)).r;
            }

            float getNumBands()
            {
                return UNITY_ACCESS_INSTANCED_PROP(Props, _NumBands);
            }

            float getBand()
            {
                return UNITY_ACCESS_INSTANCED_PROP(Props, _Band);
            }

            float getDelay()
            {
                return UNITY_ACCESS_INSTANCED_PROP(Props, _Delay);
            }

            float getBandMultiplier()
            {
                return UNITY_ACCESS_INSTANCED_PROP(Props, _BandMultiplier);
            }

            float checkIfAudioLink()
            {
                return UNITY_ACCESS_INSTANCED_PROP(Props, _EnableAudioLink);
            }

            uint checkIfColorChord()
            {
                return UNITY_ACCESS_INSTANCED_PROP(Props, _EnableColorChord);
            }

            #define IF(a, b, c) lerp(b, c, step((fixed) (a), 0));

            float getPan()
            {
                return UNITY_ACCESS_INSTANCED_PROP(Props, _XRotation);
            }

            float getAltPan()
            {
                return UNITY_ACCESS_INSTANCED_PROP(Props, _AltXRotation);
            }

            float getTilt()
            {
                return UNITY_ACCESS_INSTANCED_PROP(Props, _YRotation);
            }

            float getAltTilt()
            {
                return UNITY_ACCESS_INSTANCED_PROP(Props, _AltYRotation);
            }

            float getAltTwist()
            {
                return UNITY_ACCESS_INSTANCED_PROP(Props, _AltZRotation);
            }

            uint getLaserCount()
            {
                return UNITY_ACCESS_INSTANCED_PROP(Props, _LaserCount);
            }

            float getConeWidth()
            {
                return UNITY_ACCESS_INSTANCED_PROP(Props, _VertexConeWidth);
            }

            float getConeLength()
            {
                return UNITY_ACCESS_INSTANCED_PROP(Props, _VertexConeLength);
            }

            float getLaserThickness()
            {
                return UNITY_ACCESS_INSTANCED_PROP(Props, _LaserThickness);
            }

            float getScrollSpeed()
            {
                return UNITY_ACCESS_INSTANCED_PROP(Props, _Scroll);
            }

            float getGlobalIntensity()
            {
                return lerp(1.0,UNITY_ACCESS_INSTANCED_PROP(Props, _GlobalIntensity),
                                        UNITY_ACCESS_INSTANCED_PROP(Props, _GlobalIntensityBlend));
            }

            float getFinalIntensity()
            {
                return UNITY_ACCESS_INSTANCED_PROP(Props, _FinalIntensity);
            }

            float getConeFlatness()
            {
                return clamp(
                    UNITY_ACCESS_INSTANCED_PROP(Props, _ZConeFlatness) + UNITY_ACCESS_INSTANCED_PROP(
                        Props, _ZConeFlatnessAlt), 0, 1.999);
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

            float3 hsb2rgb(float3 c)
            {
                float3 rgb = clamp(abs(fmod(c.x * 6.0 + float3(0.0, 4.0, 2.0), 6) - 3.0) - 1.0, 0, 1);
                rgb = rgb * rgb * (3.0 - 2.0 * rgb);
                return c.z * lerp(float3(1, 1, 1), rgb, c.y);
            }

            float4 GetTextureSampleColor()
            {
                float4 rawColor = tex2Dlod(_SamplingTexture,
                    float4(
                        UNITY_ACCESS_INSTANCED_PROP(Props, _TextureColorSampleX),
                        UNITY_ACCESS_INSTANCED_PROP(Props, _TextureColorSampleY), 0, 0));
                float3 h = (RGB2HSV(rawColor.rgb));
                h.z = 1.0;
                //return float4(hsb2rgb(h),1);
                return UNITY_ACCESS_INSTANCED_PROP(Props, _UseTraditionalSampling) > 0
                     ? rawColor
                     : float4(hsb2rgb(h), 1);
            }

            float GetAudioReactAmplitude()
            {
                if (checkIfAudioLink() > 0)
                {
                    return AudioLinkLerp3_g5(getBand(), getDelay()) * getBandMultiplier();
                }
                else
                {
                    return 1;
                }
            }

            float4 GetColorChordLight()
            {
                return AudioLinkData(ALPASS_CCLIGHTS).rgba;
            }

            float4 GetThemeSampleColor()
            {
                switch (UNITY_ACCESS_INSTANCED_PROP(Props, _ThemeColorTarget))
                {
                case 1:
                    return AudioLinkData(ALPASS_THEME_COLOR0);
                case 2:
                    return AudioLinkData(ALPASS_THEME_COLOR1);
                case 3:
                    return AudioLinkData(ALPASS_THEME_COLOR2);
                case 4:
                    return AudioLinkData(ALPASS_THEME_COLOR3);
                default:
                    return float4(0, 0, 0, 1);
                }
            }

            float4 getEmissionColor()
            {
                float4 emissiveColor = UNITY_ACCESS_INSTANCED_PROP(Props, _Emission);
                float4 col = UNITY_ACCESS_INSTANCED_PROP(Props, _EnableColorTextureSample) > 0
                               ? ((emissiveColor.r + emissiveColor.g + emissiveColor.b) / 3.0) * GetTextureSampleColor()
                               : emissiveColor;
                col = UNITY_ACCESS_INSTANCED_PROP(Props, _EnableThemeColorSampling) > 0
              ? ((emissiveColor.r + emissiveColor.g + emissiveColor.b) / 3.0) *
              GetThemeSampleColor()
              : col;
                return checkIfColorChord() == 1 ? GetColorChordLight() : col;
            }

            float4 CalculateRotations(appdata v, float4 input, float pan, float tilt, float roll)
            {
                float angleY = radians(pan);
                float c = cos(angleY);
                float s = sin(angleY);
                float4x4 rotateYMatrix = float4x4(c, -s, 0, 0,
                        s, c, 0, 0,
                        0, 0, 1, 0,
                        0, 0, 0, 1);
                float4 BaseAndFixturePos = input;
                float4 localRotY = mul(rotateYMatrix, BaseAndFixturePos);


                float angleX = radians(tilt);
                c = cos(angleX);
                s = sin(angleX);
                float4x4 rotateXMatrix = float4x4(1, 0, 0, 0,
                              0, c, -s, 0,
                              0, s, c, 0,
                              0, 0, 0, 1);

                float4x4 rotateXYMatrix = mul(rotateYMatrix, rotateXMatrix);
                //float4 localRotXY = mul(rotateXYMatrix, input);


                float angleZ = radians(roll);
                c = cos(angleZ);
                s = sin(angleZ);
                float4x4 rotateZMatrix = float4x4(c, 0, s, 0,
                0, 1, 0, 0,
                -s, 0, c, 0,
                0, 0, 0, 1);
                float4x4 rotateXYZMatrix = mul(rotateZMatrix, rotateXYMatrix);
                float4 localRotXYZ = mul(rotateXYZMatrix, input);
                input.xyz = localRotXYZ.xyz;
                return input;
                //LOCALROTXY IS COMBINED ROTATION
            }

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                o.rgbIntensity.w = 1;
                if (getGlobalIntensity() <= 0.05 || getFinalIntensity() <= 0.05 || _UniversalIntensity <= 0.05 || o.
      rgbIntensity.w <= 0.05)
                {
                    o.vertex = UnityObjectToClipPos(float4(0, 0, 0, 0));
                    return o;
                }
                //replacement for _WorldSpaceCameraPos
                float3 wpos;
                wpos.x = unity_CameraToWorld[0][3];
                wpos.y = unity_CameraToWorld[1][3];
                wpos.z = unity_CameraToWorld[2][3];

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv2 = TRANSFORM_TEX(v.uv2, _MainTex);


                //Cone Length
                float length = getConeLength();
                v.vertex.y = lerp(v.vertex.y, v.vertex.y * 2, length);


                //Cone Width
                float width = getConeWidth();
                float4 vert = lerp(v.vertex, float4(v.vertex.xyz + v.normal * width, 1), v.uv2.y);
                vert.y = v.vertex.y; //Prevent the cone from elongating when changing width.

                // Cone Flatness for X and Z
                float flatness = getConeFlatness();
                vert.z = lerp(vert.z, vert.z / 2, flatness);
                //  vert.x = lerp(vert.x, vert.x/2, _XConeFlatness);
                float xRot = getPan() + getAltPan();
                float yRot = getTilt() + getAltTilt();
                vert = CalculateRotations(v, vert, _ZRotation + getAltTwist(), xRot, yRot);

                o.viewDir = normalize(wpos - mul(unity_ObjectToWorld, vert).xyz);
                v.normal = CalculateRotations(v, float4(v.normal, 1), _ZRotation + getAltTwist(), xRot, yRot).xyz;
                o.normal = normalize(mul(float4(v.normal, 0.0), unity_WorldToObject).xyz);
                o.worldPos = mul(unity_ObjectToWorld, vert);
                o.vertex = UnityObjectToClipPos(vert);
                return o;
            }


            fixed4 frag(v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                i.rgbIntensity.w = GetAudioReactAmplitude();
                if (getGlobalIntensity() <= 0.05 || getFinalIntensity() <= 0.05 || _UniversalIntensity <= 0.05 || i.
     rgbIntensity.w <= 0.05)
                {
                    return half4(0, 0, 0, 0);
                }
                float fade = pow(saturate(dot(normalize(i.normal), i.viewDir)), _FadeStrength);


                // fade = pow(fade, pow(_FadeStrength, _FadeAmt)) * fade;
                // sample the texture
                float4 actualcolor = getEmissionColor() * _Multiplier;
                float4 color = lerp(float4(0, 0, 0, 0), actualcolor, getGlobalIntensity());
                float3 newColor = RGB2HSV(color.rgb);
                newColor.y -= 0.1;
                color.rgb = hsb2rgb(newColor);

                color = color * getFinalIntensity();
                color = color * _UniversalIntensity;
                color = color * i.rgbIntensity.w;
                fixed4 col = float4(1, 1, 1, 1);
                fixed4 alphaMask = tex2D(_MainTex, i.uv2);
                float2 laserUV = i.uv2;
                col *= color;

                //Draw Beams
                float scroll = getScrollSpeed();
                laserUV.x = laserUV.x += _Time.y * scroll;
                float beamcount = getLaserCount();
                laserUV.x = frac(laserUV.x * beamcount);
                laserUV.x = laserUV.x - 0.5;
                float thiknes = getLaserThickness();


                // Transparency (with gradation)
                //float dist = normalize(distance(i.worldPos, float4(0,0,0,0))); 
                // fade = lerp(1,fade, pow(dist, _FadeAmt));


                float beams = (beamcount * thiknes * 0.1) / abs(laserUV.x);
                float transv = pow((-i.uv2.y + 1), _EndFade);
                col = col * beams;

                col += pow(-laserUV.y + 1, _InternalShineLength) * 3 * _InternalShine * color;

                // float beamGradCorrect = .2 / (1.-laserUV.y);
                // col *= beamGradCorrect;
                // Clamping here softens the depiction
                col = clamp(0, _LaserSoftening, col);

                // col = lerp(col, col*10, _ZConeFlatness/1.999);
                // float edgeFadeL = distance(i.uv2.x, 0.75);
                // float edgeFadeR = distance(i.uv2.x, 0.25);
                // float4 edgee = lerp(float4(0,0,0,0), col, pow(edgeFadeR * edgeFadeL, _FadeStrength));
                // col = lerp(col, edgee, _ZConeFlatness/1.999);

                //                col.xyz += rimlight.xyz;
                //col *= fade;
                col = lerp(float4(0, 0, 0, 0), col, transv);
                float pi = 3.14159;
                float edgeMask = clamp((((sin((12.5 * i.uv2.x) + 4.75)))) / pi * 6, 0, 1);
                edgeMask = 1 - edgeMask;
                if (i.uv2.y > 0.99)
                {
                    discard;
                    return float4(0, 0, 0, 0);
                }

                // col = col * col *;
                // float4 flatCol = col * flatEdgeMask.r;
                float flatness = getConeFlatness();

                float4 flatCol = col * edgeMask;
                col = lerp(flatCol, col, pow(((flatness / 2.0) - 1.0) * -1, 0.95));
                col *= getFinalIntensity() * _UniversalIntensity * i.rgbIntensity.w;
                col *= UNITY_ACCESS_INSTANCED_PROP(Props, _BlackOut);
                return col;
            }
            ENDCG
        }
        // Used for handling Depth Buffer (DBuffer) and Depth Priming
        UsePass "Universal Render Pipeline/Lit/DepthOnly"
        UsePass "Universal Render Pipeline/Lit/DepthNormals"
    }
    SubShader
    {
		Tags { "RenderType"="Transparent" "Queue" = "Transparent+4" }
		Cull Off
		Blend One One
		Zwrite Off
		LOD 100

        Pass
        {

            Stencil
			{
				Ref 142
				Comp NotEqual
				Pass Keep
			}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
            #define VRSL_AUDIOLINK

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
                float3 normal : TEXCOORD2;
                UNITY_FOG_COORDS(9)
                float4 vertex : SV_POSITION;
                float4 worldPos : TEXCOORD3;
                float3 viewDir : TEXCOORD4;
                float4 panTiltLengthWidth : TEXCOORD5; //ch 1,2,3,4
                float4 flatnessBeamCountSpinThickness : TEXCOORD6; //ch 5,6,7,12
                float4 rgbIntensity : TEXCOORD7;// ch 8,9,10,11
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            sampler2D _MainTex, _SamplingTexture;
            float4 _MainTex_ST;
            half _XConeFlatness, _ZRotation, _UniversalIntensity;
            half _EndFade, _FadeStrength, _InternalShine, _LaserSoftening, _InternalShineLength, _Multiplier;
            uint _EnableCompatibilityMode;
            uniform const float compatSampleYAxis = 0.019231;
            uniform const float standardSampleYAxis = 0.00762;
            #include "Packages/com.llealloo.audiolink/Runtime/Shaders/AudioLink.cginc"


            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float, _EnableAudioLink)
                UNITY_DEFINE_INSTANCED_PROP(float, _EnableColorChord)
                UNITY_DEFINE_INSTANCED_PROP(float, _NumBands)
                UNITY_DEFINE_INSTANCED_PROP(float, _Band)
                UNITY_DEFINE_INSTANCED_PROP(float, _BandMultiplier)
                UNITY_DEFINE_INSTANCED_PROP(float, _Delay)
                UNITY_DEFINE_INSTANCED_PROP(uint, _LaserCount)
                UNITY_DEFINE_INSTANCED_PROP(uint, _EnableColorTextureSample)
                UNITY_DEFINE_INSTANCED_PROP(float, _Scroll)
                UNITY_DEFINE_INSTANCED_PROP(float, _XRotation)
                UNITY_DEFINE_INSTANCED_PROP(float, _YRotation)
                UNITY_DEFINE_INSTANCED_PROP(float, _AltZRotation)
                UNITY_DEFINE_INSTANCED_PROP(float, _AltXRotation)
                UNITY_DEFINE_INSTANCED_PROP(float, _AltYRotation)
                UNITY_DEFINE_INSTANCED_PROP(float, _LaserThickness)
                UNITY_DEFINE_INSTANCED_PROP(float4, _Emission)
                UNITY_DEFINE_INSTANCED_PROP(float, _ZConeFlatness)
                UNITY_DEFINE_INSTANCED_PROP(float, _ZConeFlatnessAlt)
                UNITY_DEFINE_INSTANCED_PROP(float, _VertexConeWidth)
                UNITY_DEFINE_INSTANCED_PROP(float, _VertexConeLength)
                UNITY_DEFINE_INSTANCED_PROP(float, _GlobalIntensity)
                UNITY_DEFINE_INSTANCED_PROP(float, _GlobalIntensityBlend)
                UNITY_DEFINE_INSTANCED_PROP(float, _FinalIntensity)
                UNITY_DEFINE_INSTANCED_PROP(float, _TextureColorSampleX)
                UNITY_DEFINE_INSTANCED_PROP(float, _TextureColorSampleY)
                UNITY_DEFINE_INSTANCED_PROP(float, _BlackOut)
                UNITY_DEFINE_INSTANCED_PROP(float, _ThemeColorTarget)
                UNITY_DEFINE_INSTANCED_PROP(uint, _EnableThemeColorSampling)
                UNITY_DEFINE_INSTANCED_PROP(uint, _UseTraditionalSampling)
            UNITY_INSTANCING_BUFFER_END(Props)

             inline float AudioLinkLerp3_g5( int Band, float Delay )
            {
                return AudioLinkLerp( ALPASS_AUDIOLINK + float2( Delay, Band ) ).r;
            }

            float getNumBands()
            {
                return UNITY_ACCESS_INSTANCED_PROP(Props, _NumBands);
            }

            float getBand()
            {
                return UNITY_ACCESS_INSTANCED_PROP(Props, _Band);
            }

            float getDelay()
            {
                return UNITY_ACCESS_INSTANCED_PROP(Props, _Delay);
            }

            float getBandMultiplier()
            {
                return UNITY_ACCESS_INSTANCED_PROP(Props, _BandMultiplier);
            }

            float checkIfAudioLink()
            {
                return UNITY_ACCESS_INSTANCED_PROP(Props, _EnableAudioLink);
            }

            uint checkIfColorChord()
            {
                return UNITY_ACCESS_INSTANCED_PROP(Props, _EnableColorChord);
            }

            #define IF(a, b, c) lerp(b, c, step((fixed) (a), 0));

            float getPan()
            {
                return UNITY_ACCESS_INSTANCED_PROP(Props,_XRotation);
            }
            float getAltPan()
            {
                return UNITY_ACCESS_INSTANCED_PROP(Props,_AltXRotation);
            }
            float getTilt()
            {
                return UNITY_ACCESS_INSTANCED_PROP(Props,_YRotation);
            }
            float getAltTilt()
            {
                return UNITY_ACCESS_INSTANCED_PROP(Props,_AltYRotation);
            }
            float getAltTwist()
            {
                return UNITY_ACCESS_INSTANCED_PROP(Props,_AltZRotation);
            }

            uint getLaserCount()
            {
                return UNITY_ACCESS_INSTANCED_PROP(Props,_LaserCount);
            }
            float getConeWidth()
            {
                return UNITY_ACCESS_INSTANCED_PROP(Props,_VertexConeWidth);
            }
            float getConeLength()
            {
                return UNITY_ACCESS_INSTANCED_PROP(Props,_VertexConeLength);
            }

            float getLaserThickness()
            {
                return UNITY_ACCESS_INSTANCED_PROP(Props,_LaserThickness);
            }
            float getScrollSpeed()
            {
                return UNITY_ACCESS_INSTANCED_PROP(Props,_Scroll);
            }
            float getGlobalIntensity()
            {
                return lerp(1.0,UNITY_ACCESS_INSTANCED_PROP(Props, _GlobalIntensity), UNITY_ACCESS_INSTANCED_PROP(Props, _GlobalIntensityBlend));
            }

            float getFinalIntensity()
            {
                return UNITY_ACCESS_INSTANCED_PROP(Props, _FinalIntensity);
            }
            float getConeFlatness()
            {
                return clamp(UNITY_ACCESS_INSTANCED_PROP(Props, _ZConeFlatness) + UNITY_ACCESS_INSTANCED_PROP(Props, _ZConeFlatnessAlt),0, 1.999);
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

            float3 hsb2rgb( float3 c )
            {
                float3 rgb = clamp( abs(fmod(c.x*6.0+float3(0.0,4.0,2.0),6)-3.0)-1.0, 0, 1);
                rgb = rgb*rgb*(3.0-2.0*rgb);
                return c.z * lerp( float3(1,1,1), rgb, c.y);
            }

            float4 GetTextureSampleColor()
            {
                float4 rawColor = tex2Dlod(_SamplingTexture, float4(UNITY_ACCESS_INSTANCED_PROP(Props,_TextureColorSampleX), UNITY_ACCESS_INSTANCED_PROP(Props,_TextureColorSampleY), 0, 0));
                float3 h = (RGB2HSV(rawColor.rgb));
                h.z = 1.0;
                //return float4(hsb2rgb(h),1);
                return UNITY_ACCESS_INSTANCED_PROP(Props, _UseTraditionalSampling) > 0 ? rawColor : float4(hsb2rgb(h),1);
            }

            float GetAudioReactAmplitude()
            {
                if(checkIfAudioLink() > 0)
                {
                    return AudioLinkLerp3_g5(getBand(), getDelay()) * getBandMultiplier();
                }
                else
                {
                    return 1;
                }
            }

            float4 GetColorChordLight()
            {
                return AudioLinkData(ALPASS_CCLIGHTS).rgba;
            }
            float4 GetThemeSampleColor()
            {
                switch(UNITY_ACCESS_INSTANCED_PROP(Props, _ThemeColorTarget))
                {
                    case 1:
                        return AudioLinkData(ALPASS_THEME_COLOR0);
                    case 2:
                        return AudioLinkData(ALPASS_THEME_COLOR1);
                    case 3:
                        return AudioLinkData(ALPASS_THEME_COLOR2);
                    case 4:
                        return AudioLinkData(ALPASS_THEME_COLOR3);
                    default:
                        return float4(0,0,0,1);
                }
            }

            float4 getEmissionColor()
            {
                float4 emissiveColor = UNITY_ACCESS_INSTANCED_PROP(Props,_Emission);
                float4 col =  UNITY_ACCESS_INSTANCED_PROP(Props,_EnableColorTextureSample) > 0 ? ((emissiveColor.r + emissiveColor.g + emissiveColor.b)/3.0) * GetTextureSampleColor() : emissiveColor;
                col =  UNITY_ACCESS_INSTANCED_PROP(Props,_EnableThemeColorSampling) > 0 ? ((emissiveColor.r + emissiveColor.g + emissiveColor.b)/3.0) * GetThemeSampleColor() : col;
                return  checkIfColorChord() == 1 ? GetColorChordLight() : col;
            }
            float4 CalculateRotations(appdata v, float4 input, float pan, float tilt, float roll)
            {
                float angleY = radians(pan);
                float c = cos(angleY);
                float s = sin(angleY);
                float4x4 rotateYMatrix = float4x4(c, -s, 0, 0,
                    s, c, 0, 0,
                    0, 0, 1, 0,
                    0, 0, 0, 1);
                float4 BaseAndFixturePos = input;
                float4 localRotY = mul(rotateYMatrix, BaseAndFixturePos);


                float angleX = radians(tilt);
                c = cos(angleX);
                s = sin(angleX);
                float4x4 rotateXMatrix = float4x4(1, 0, 0, 0,
                    0, c, -s, 0,
                    0, s, c, 0,
                    0, 0, 0, 1);

                float4x4 rotateXYMatrix = mul(rotateYMatrix, rotateXMatrix);
                //float4 localRotXY = mul(rotateXYMatrix, input);
                
                
                float angleZ = radians(roll);
                c = cos(angleZ);
                s = sin(angleZ);
                float4x4 rotateZMatrix = float4x4(c, 0, s, 0,
                    0, 1, 0, 0,
                    -s, 0, c, 0,
                    0, 0, 0, 1);
                float4x4 rotateXYZMatrix = mul(rotateZMatrix, rotateXYMatrix);
                float4 localRotXYZ = mul(rotateXYZMatrix, input);
                input.xyz = localRotXYZ.xyz;
                return input;
                //LOCALROTXY IS COMBINED ROTATION
            }

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                o.rgbIntensity.w = 1;
                if(getGlobalIntensity() <= 0.05 || getFinalIntensity() <= 0.05 || _UniversalIntensity <= 0.05 || o.rgbIntensity.w <= 0.05)
                {
                    o.vertex = UnityObjectToClipPos(float4(0,0,0,0));
                    return o;
                }
                //replacement for _WorldSpaceCameraPos
                float3 wpos;
                wpos.x = unity_CameraToWorld[0][3];
                wpos.y = unity_CameraToWorld[1][3];
                wpos.z = unity_CameraToWorld[2][3];

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv2 = TRANSFORM_TEX(v.uv2, _MainTex);
                 
                
                //Cone Length
                float length = getConeLength();
                v.vertex.y = lerp(v.vertex.y, v.vertex.y *2, length);
                

                //Cone Width
                float width = getConeWidth();
                float4 vert = lerp(v.vertex ,float4(v.vertex.xyz + v.normal * width, 1), v.uv2.y);
                vert.y = v.vertex.y; //Prevent the cone from elongating when changing width.

                // Cone Flatness for X and Z
                float flatness = getConeFlatness();
                vert.z = lerp(vert.z, vert.z/2, flatness);
              //  vert.x = lerp(vert.x, vert.x/2, _XConeFlatness);
                float xRot = getPan() + getAltPan();
                float yRot = getTilt() + getAltTilt();
                vert = CalculateRotations(v, vert, _ZRotation + getAltTwist(), xRot, yRot);

                o.viewDir = normalize(wpos - mul(unity_ObjectToWorld, vert).xyz);
                v.normal = CalculateRotations(v, float4(v.normal, 1), _ZRotation + getAltTwist(), xRot, yRot).xyz;
                o.normal = normalize(mul(float4(v.normal, 0.0), unity_WorldToObject).xyz);
                o.worldPos = mul(unity_ObjectToWorld, vert);
                o.vertex = UnityObjectToClipPos(vert);
                return o;
            }
            

            fixed4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                i.rgbIntensity.w = GetAudioReactAmplitude();
                if(getGlobalIntensity() <= 0.05 || getFinalIntensity() <= 0.05 || _UniversalIntensity <= 0.05 || i.rgbIntensity.w <= 0.05)
                {
                    return half4(0,0,0,0);
                }
                float fade = pow(saturate(dot(normalize(i.normal), i.viewDir)), _FadeStrength);

                
                
                
               // fade = pow(fade, pow(_FadeStrength, _FadeAmt)) * fade;
                // sample the texture
                float4 actualcolor = getEmissionColor() * _Multiplier;
                float4 color = lerp(float4(0,0,0,0), actualcolor, getGlobalIntensity());
                float3 newColor = RGB2HSV(color.rgb);
                newColor.y -= 0.1;
                color.rgb = hsb2rgb(newColor);
                
                color = color * getFinalIntensity();
                color = color * _UniversalIntensity;
                color = color * i.rgbIntensity.w;
                fixed4 col = float4(1,1,1,1);
                fixed4 alphaMask = tex2D(_MainTex, i.uv2);
                float2 laserUV = i.uv2;
                col *= color;

                //Draw Beams
                float scroll = getScrollSpeed();
                laserUV.x = laserUV.x += _Time.y * scroll;  
                float beamcount = getLaserCount();
                laserUV.x = frac(laserUV.x * beamcount);
                laserUV.x = laserUV.x - 0.5;
                float thiknes = getLaserThickness();
                
                
                // Transparency (with gradation)
                //float dist = normalize(distance(i.worldPos, float4(0,0,0,0))); 
               // fade = lerp(1,fade, pow(dist, _FadeAmt));

                
                float beams = (beamcount * thiknes * 0.1) / abs(laserUV.x);
                float transv = pow((-i.uv2.y + 1), _EndFade);
                col = col * beams;

                col += pow(-laserUV.y+1,_InternalShineLength) * 3 * _InternalShine * color;

                // float beamGradCorrect = .2 / (1.-laserUV.y);
                // col *= beamGradCorrect;
                // Clamping here softens the depiction
                col = clamp(0, _LaserSoftening, col);

                // col = lerp(col, col*10, _ZConeFlatness/1.999);
                // float edgeFadeL = distance(i.uv2.x, 0.75);
                // float edgeFadeR = distance(i.uv2.x, 0.25);
                // float4 edgee = lerp(float4(0,0,0,0), col, pow(edgeFadeR * edgeFadeL, _FadeStrength));
                // col = lerp(col, edgee, _ZConeFlatness/1.999);

//                col.xyz += rimlight.xyz;
                //col *= fade;
                col = lerp(float4(0,0,0,0), col, transv);
                float pi = 3.14159;
                float edgeMask = clamp((((sin((12.5 * i.uv2.x) + 4.75)) ))/pi * 6, 0,1);
                edgeMask = 1 - edgeMask;
                if(i.uv2.y > 0.99)
                {
                    discard;
                    return float4(0,0,0,0);
                }
                
                // col = col * col *;
               // float4 flatCol = col * flatEdgeMask.r;
               float flatness = getConeFlatness();
              
               float4 flatCol = col * edgeMask;
                col = lerp(flatCol, col, pow(((flatness/2.0) - 1.0)*-1, 0.95));
                col *= getFinalIntensity() * _UniversalIntensity * i.rgbIntensity.w;
                col *= UNITY_ACCESS_INSTANCED_PROP(Props, _BlackOut);
                return col;
            }
            ENDCG
        }
    }
}
