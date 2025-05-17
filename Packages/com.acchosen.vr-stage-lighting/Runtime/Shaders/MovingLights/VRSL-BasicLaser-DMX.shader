Shader "VRSL/Basic Laser/DMX"
{
    Properties
    {
        [Toggle] _EnableDMX ("Enable Stream DMX/DMX Control", Int) = 0
        [Toggle] _EnableCompatibilityMode ("Enable Compatibility Mode", Int) = 0
        [Toggle] _EnableVerticalMode ("Enable Vertical Mode", Int) = 0
        _DMXChannel ("DMX Channel Number)", Int) = 0
        [HideInInspector][Toggle] _NineUniverseMode ("Extended Universe Mode", Int) = 0
        _UniversalIntensity ("Universal Intensity", Range (0,1)) = 1
        _FinalIntensity("Final Intensity", Range(0,1)) = 1
        _GlobalIntensity ("Global Intensity", Range(0,1)) = 1
        _GlobalIntensityBlend("Global Intensity Blend", Range(0,1)) = 1
        [HideInInspector]_MainTex ("Texture", 2D) = "white" {}
        [HDR] _MainColor ("Main Color" , Color) = (1.0, 1.0, 1.0, 1.0)
        _Emission ("Instanced Emission Color" , Color) = (1.0, 1.0, 1.0, 1.0)
        _VertexConeWidth ("Cone Width", Range(-3.75,20)) = 0
        _VertexConeLength("Cone Length", Range(-0.5,5)) = 0
        _ZConeFlatness("Z Flatness", Range(0,1.999)) = 0
     //  _XConeFlatness("X Flatness", Range(0,1.99)) = 0
        _ZRotation ("Z Rotation", Range (-90, 90)) = 0
        _XRotation ("X Rotation", Range (-90, 90)) = 0
        _YRotation ("Y Rotation", Range (-180, 180)) = 0
        [IntRange] _LaserCount ("Laser Beam Count", Range(4, 68)) = 1
        _LaserThickness ("Laser Beam Thickenss", Range(0.003, 0.1)) = 1
        _EndFade ("End Fade", Range(0,3)) = 2.2
        _FadeStrength ("Cone Edge Fade", Range(1,2)) = 0
        _LaserSoftening ("Laser Softness", Range(0.05,10)) = 0
        _InternalShine ("Internal Shine Strength", Range(0,5)) = 1
         _InternalShineLength ("Internal Shine Length", Range(0.001,500)) = 12.1
        _Scroll ("Scroll", Range(-1, 1)) = 1
        _ScrollOffset ("Scroll Offset", Range(0.00001, 0.00003)) = 0.00001
        
        
        // [NoScaleOffset] _Udon_DMXGridRenderTexture("DMX Grid Render Texture (RAW Unsmoothed)", 2D) = "white" {}
		// [NoScaleOffset] _Udon_DMXGridRenderTextureMovement("DMX Grid Render Texture (To Control Lights)", 2D) = "white" {}


    }
    SubShader
    {
		Tags { "RenderType"="Transparent" "Queue" = "Transparent+1" "RenderingPipeline" = "UniversalPipeline" }
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
            //#pragma multi_compile_fog
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"
            #define LASER

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
               centroid float2 uv : TEXCOORD0;
               centroid float2 uv2 : TEXCOORD1;
                float3 normal : TEXCOORD2;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float4 worldPos : TEXCOORD3;
                float3 viewDir : TEXCOORD4;
                nointerpolation float4 panTiltLengthWidth : TEXCOORD5; //ch 1,2,3,4
                nointerpolation float4 flatnessBeamCountSpinThickness : TEXCOORD6; //ch 5,6,7,12
                nointerpolation float4 rgbIntensity : TEXCOORD7;// ch 8,9,10,11
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_ST, _MainColor;
            half _XConeFlatness, _ZRotation, _UniversalIntensity;
            half _EndFade, _FadeStrength, _InternalShine, _LaserSoftening, _InternalShineLength;
            float _ScrollOffset;
            uint _EnableCompatibilityMode, _EnableVerticalMode;
            sampler2D _Udon_DMXGridRenderTexture, _Udon_DMXGridRenderTextureMovement, _Udon_DMXGridSpinTimer;
            uniform float4 _Udon_DMXGridRenderTexture_TexelSize;
            uniform const half compatSampleYAxis = 0.019231;
            uniform const half standardSampleYAxis = 0.00762;

            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(uint, _DMXChannel)
                UNITY_DEFINE_INSTANCED_PROP(uint, _NineUniverseMode)
                UNITY_DEFINE_INSTANCED_PROP(uint, _EnableColorTextureSample)
                UNITY_DEFINE_INSTANCED_PROP(uint, _LaserCount)
                UNITY_DEFINE_INSTANCED_PROP(uint, _EnableDMX)
                UNITY_DEFINE_INSTANCED_PROP(half, _Scroll)
                UNITY_DEFINE_INSTANCED_PROP(half, _XRotation)
                UNITY_DEFINE_INSTANCED_PROP(half, _YRotation)
                UNITY_DEFINE_INSTANCED_PROP(half, _LaserThickness)
                UNITY_DEFINE_INSTANCED_PROP(half4, _Emission)
                UNITY_DEFINE_INSTANCED_PROP(half, _ZConeFlatness)
                UNITY_DEFINE_INSTANCED_PROP(half, _VertexConeWidth)
                UNITY_DEFINE_INSTANCED_PROP(half, _VertexConeLength)
                UNITY_DEFINE_INSTANCED_PROP(half, _GlobalIntensity)
                UNITY_DEFINE_INSTANCED_PROP(half, _GlobalIntensityBlend)
                UNITY_DEFINE_INSTANCED_PROP(half, _FinalIntensity)
                UNITY_DEFINE_INSTANCED_PROP(half, _TextureColorSampleX)
                UNITY_DEFINE_INSTANCED_PROP(half, _TextureColorSampleY)
            UNITY_INSTANCING_BUFFER_END(Props)

            #include "../Shared/VRSL-DMXFunctions.cginc"

            //#define IF(a, b, c) lerp(b, c, step((fixed) (a), 0));

            half getPan()
            {
                return UNITY_ACCESS_INSTANCED_PROP(Props,_XRotation);
            }
            half getTilt()
            {
                return UNITY_ACCESS_INSTANCED_PROP(Props,_YRotation);
            }
            uint getLaserCount()
            {
                return UNITY_ACCESS_INSTANCED_PROP(Props,_LaserCount);
            }
            half getConeWidth()
            {
                return UNITY_ACCESS_INSTANCED_PROP(Props,_VertexConeWidth);
            }
            half getConeLength()
            {
                return UNITY_ACCESS_INSTANCED_PROP(Props,_VertexConeLength);
            }
            half getLaserThickness()
            {
                return UNITY_ACCESS_INSTANCED_PROP(Props,_LaserThickness);
            }
            half getScrollSpeed()
            {
                return UNITY_ACCESS_INSTANCED_PROP(Props,_Scroll);
            }
            half getConeFlatness()
            {
                return UNITY_ACCESS_INSTANCED_PROP(Props, _ZConeFlatness);
            }

            half3 RGB2HSV(half3 c)
            {
                half4 K = half4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                half4 p = lerp(half4(c.bg, K.wz), half4(c.gb, K.xy), step(c.b, c.g));
                half4 q = lerp(half4(p.xyw, c.r), half4(c.r, p.yzx), step(p.x, c.r));

                half d = q.x - min(q.w, q.y);
                half e = 1.0e-10;
                return half3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
            }

            half3 hsb2rgb( half3 c ){
                half3 rgb = clamp( abs(fmod(c.x*6.0+half3(0.0,4.0,2.0),6)-3.0)-1.0, 0, 1);
                rgb = rgb*rgb*(3.0-2.0*rgb);
                return c.z * lerp( half3(1,1,1), rgb, c.y);
            }

            half4 CalculateRotations(appdata v, half4 input, half pan, half tilt, half roll)
            {
                half angleY = radians(pan);
                half c = cos(angleY);
                half s = sin(angleY);
                half4x4 rotateYMatrix = half4x4(c, -s, 0, 0,
                    s, c, 0, 0,
                    0, 0, 1, 0,
                    0, 0, 0, 1);
                half4 BaseAndFixturePos = input;
                half4 localRotY = mul(rotateYMatrix, BaseAndFixturePos);

                half angleX = radians(tilt);
                c = cos(angleX);
                s = sin(angleX);
                half4x4 rotateXMatrix = half4x4(1, 0, 0, 0,
                    0, c, -s, 0,
                    0, s, c, 0,
                    0, 0, 0, 1);

                half4x4 rotateXYMatrix = mul(rotateYMatrix, rotateXMatrix);
                //half4 localRotXY = mul(rotateXYMatrix, input);

                half angleZ = radians(roll);
                c = cos(angleZ);
                s = sin(angleZ);
                half4x4 rotateZMatrix = half4x4(c, 0, s, 0,
                    0, 1, 0, 0,
                    -s, 0, c, 0,
                    0, 0, 0, 1);
                half4x4 rotateXYZMatrix = mul(rotateZMatrix, rotateXYMatrix);
                half4 localRotXYZ = mul(rotateXYZMatrix, input);
                input.xyz = localRotXYZ.xyz;
                return input;
                //LOCALROTXY IS COMBINED ROTATION
            }

            half GetSpin(uint DMXChannel)
            {
                half status = getValueAtCoords(DMXChannel, _Udon_DMXGridRenderTexture);
                half phase = getValueAtCoordsRaw(DMXChannel, _Udon_DMXGridSpinTimer);
                //phase = checkPanInvertY() == 1 ? -phase : phase;
                return status > 0.5 ? -phase : phase;
            }

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o)
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                uint dmx = getDMXChannel();
                o.rgbIntensity.w = getValueAtCoords(dmx + (uint) 10, _Udon_DMXGridRenderTexture);
                o.rgbIntensity.w = IF(isDMX() > 0, o.rgbIntensity.w, 1);
                if(getGlobalIntensity() <= 0.01 || getFinalIntensity() <= 0.05 || _UniversalIntensity <= 0.05 || o.rgbIntensity.w <= 0.05)
                {
                    o.vertex = UnityObjectToClipPos(half4(0,0,0,0));
                    return o;
                }
                o.panTiltLengthWidth.x = lerp(-90,90,clamp(getValueAtCoords(dmx, _Udon_DMXGridRenderTextureMovement), 0.0,0.9999)); // ch 1
                o.panTiltLengthWidth.y = lerp(-90,90,clamp(getValueAtCoords(dmx + (uint) 1, _Udon_DMXGridRenderTextureMovement), 0.0,0.9999)); // ch 2
                o.panTiltLengthWidth.z = lerp(-0.5,5,clamp(getValueAtCoords(dmx + (uint) 2, _Udon_DMXGridRenderTexture), 0.0,0.9999)); // ch 3
                o.panTiltLengthWidth.w = lerp(-3.75,20,clamp(getValueAtCoords(dmx + (uint) 3, _Udon_DMXGridRenderTexture), 0.0,0.9999)); // ch 4

                //replacement for _WorldSpaceCameraPos
                float3 wpos;
                wpos.x = unity_CameraToWorld[0][3];
                wpos.y = unity_CameraToWorld[1][3];
                wpos.z = unity_CameraToWorld[2][3];

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv2 = TRANSFORM_TEX(v.uv2, _MainTex);

                //Cone Length
                half length = IF(isDMX() > 0, o.panTiltLengthWidth.z, getConeLength());
                v.vertex.y = lerp(v.vertex.y, v.vertex.y *2, length);

                //Cone Width
                half width = IF(isDMX() > 0, o.panTiltLengthWidth.w, getConeWidth());
                half4 vert = lerp(v.vertex ,half4(v.vertex.xyz + v.normal * width, 1), v.uv2.y);
                vert.y = v.vertex.y; //Prevent the cone from elongating when changing width.

                // Cone Flatness for X and Z
                half flatness = lerp(0,1.999,getValueAtCoords(dmx+ (uint) 4, _Udon_DMXGridRenderTexture));
                flatness = IF(isDMX() > 0, flatness, getConeFlatness());
                vert.z = lerp(vert.z, vert.z/2, flatness);
              //  vert.x = lerp(vert.x, vert.x/2, _XConeFlatness);
                half xRot = IF(isDMX() > 0, o.panTiltLengthWidth.x, getPan());
                half yRot = IF(isDMX() > 0, o.panTiltLengthWidth.y, getTilt());
                vert = CalculateRotations(v, vert, _ZRotation, xRot, yRot);

                o.viewDir = normalize(wpos - mul(unity_ObjectToWorld, vert).xyz);
                v.normal = CalculateRotations(v, half4(v.normal, 1), _ZRotation, xRot, yRot).xyz;
                o.normal = normalize(mul(half4(v.normal, 0.0), unity_WorldToObject).xyz);
                o.worldPos = mul(unity_ObjectToWorld, vert);
                o.vertex = UnityObjectToClipPos(vert);

                o.flatnessBeamCountSpinThickness.x = lerp(0,1.999,getValueAtCoords(dmx+ (uint) 4, _Udon_DMXGridRenderTexture)); //5
                o.flatnessBeamCountSpinThickness.y = lerp(4,68,getValueAtCoords(dmx+ (uint) 5, _Udon_DMXGridRenderTexture));//6
                o.flatnessBeamCountSpinThickness.z = GetSpin(dmx+ (uint) 6) * _ScrollOffset; //7
                o.flatnessBeamCountSpinThickness.w = lerp(0.0005, 0.05,getValueAtCoords(dmx+ (uint) 11, _Udon_DMXGridRenderTexture)); //12
                o.rgbIntensity.x = getValueAtCoords(dmx+ (uint) 7, _Udon_DMXGridRenderTexture); //8 
                o.rgbIntensity.y = getValueAtCoords(dmx+ (uint) 8, _Udon_DMXGridRenderTexture); //9
                o.rgbIntensity.z = getValueAtCoords(dmx+ (uint) 9, _Udon_DMXGridRenderTexture); //10
                o.rgbIntensity.w = getValueAtCoords(dmx+ (uint) 10, _Udon_DMXGridRenderTexture); //11

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                UNITY_SETUP_INSTANCE_ID(i);
                uint dmx = getDMXChannel();

                i.rgbIntensity.w = IF(isDMX() > 0, i.rgbIntensity.w, 1);
                if(getGlobalIntensity() <= 0.001 || getFinalIntensity() <= 0.001 || _UniversalIntensity <= 0.001 || i.rgbIntensity.w <= 0.001)
                {
                    return half4(0,0,0,0);
                }
                half fade = pow(saturate(dot(normalize(i.normal), i.viewDir)), _FadeStrength);

                // fade = pow(fade, pow(_FadeStrength, _FadeAmt)) * fade;
                // sample the texture
                half4 dmxcol = half4(i.rgbIntensity.x,i.rgbIntensity.y, i.rgbIntensity.z, _MainColor.a);
                half4 actualcolor = IF(isDMX() > 0, dmxcol *_MainColor * getEmissionColor(), _MainColor * getEmissionColor());
                half4 color = lerp(half4(0,0,0,_MainColor.a), actualcolor, getGlobalIntensity());
                // half3 newColor = RGB2HSV(color.rgb);
                // newColor.y -= 0.1;
                // color.rgb = hsb2rgb(newColor);

                color = color * getFinalIntensity();
                color = color * _UniversalIntensity;
                color = color * i.rgbIntensity.w;
                fixed4 col = half4(1,1,1,1);
                fixed4 alphaMask = tex2D(_MainTex, i.uv2);
                half2 laserUV = i.uv2;
                col *= color;

                //Draw Beams
                half scroll = IF(isDMX() > 0, i.flatnessBeamCountSpinThickness.z, getScrollSpeed());
                laserUV.x += _Time.y * scroll;
                half beamcount = IF(isDMX() > 0, round(i.flatnessBeamCountSpinThickness.y), getLaserCount());
                laserUV.x = frac(laserUV.x * beamcount);
                laserUV.x = laserUV.x - 0.5;
                half thiknes = IF(isDMX() > 0, i.flatnessBeamCountSpinThickness.w, getLaserThickness());

                // Transparency (with gradation)
                //half dist = normalize(distance(i.worldPos, half4(0,0,0,0)));
                // fade = lerp(1,fade, pow(dist, _FadeAmt));

                half beams = (beamcount * thiknes * 0.1) / abs(laserUV.x);
                half transv = pow((-i.uv2.y + 1), _EndFade);
                col = col * beams;

                col += pow(-laserUV.y+1,_InternalShineLength*1.5) * _InternalShine * color;

                // half beamGradCorrect = .2 / (1.-laserUV.y);
                // col *= beamGradCorrect;
                // Clamping here softens the depiction
                col = clamp(0, _LaserSoftening, col);

                // col = lerp(col, col*10, _ZConeFlatness/1.999);
                // half edgeFadeL = distance(i.uv2.x, 0.75);
                // half edgeFadeR = distance(i.uv2.x, 0.25);
                // half4 edgee = lerp(half4(0,0,0,0), col, pow(edgeFadeR * edgeFadeL, _FadeStrength));
                // col = lerp(col, edgee, _ZConeFlatness/1.999);

                // col.xyz += rimlight.xyz;
                // col *= fade;
                col = lerp(half4(0,0,0,0), col, transv);
                half pi = 3.14159;
                half edgeMask = clamp((((sin((12.5 * i.uv2.x) + 4.75)) ))/pi * 6, 0,1);
                edgeMask = 1 - edgeMask;
                if(i.uv2.y > 0.99)
                {
                    discard;
                    return half4(0,0,0,0);
                }

                // col = col * col *;
                // half4 flatCol = col * flatEdgeMask.r;
                half flatness = IF(isDMX() > 0, i.flatnessBeamCountSpinThickness.x, getConeFlatness());

                half4 flatCol = col * edgeMask;
                col = lerp(flatCol, col, pow(((flatness/2.0) - 1.0)*-1, 0.95));
                col *= getFinalIntensity() * _UniversalIntensity * i.rgbIntensity.w;
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
		Tags { "RenderType"="Transparent" "Queue" = "Transparent+1" }
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
            //#pragma multi_compile_fog
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"
            #define LASER

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
               centroid float2 uv : TEXCOORD0;
               centroid float2 uv2 : TEXCOORD1;
                float3 normal : TEXCOORD2;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float4 worldPos : TEXCOORD3;
                float3 viewDir : TEXCOORD4;
                nointerpolation float4 panTiltLengthWidth : TEXCOORD5; //ch 1,2,3,4
                nointerpolation float4 flatnessBeamCountSpinThickness : TEXCOORD6; //ch 5,6,7,12
                nointerpolation float4 rgbIntensity : TEXCOORD7;// ch 8,9,10,11
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_ST, _MainColor;
            half _XConeFlatness, _ZRotation, _UniversalIntensity;
            half _EndFade, _FadeStrength, _InternalShine, _LaserSoftening, _InternalShineLength;
            float _ScrollOffset;
            uint _EnableCompatibilityMode, _EnableVerticalMode;
            sampler2D _Udon_DMXGridRenderTexture, _Udon_DMXGridRenderTextureMovement, _Udon_DMXGridSpinTimer;
            uniform float4 _Udon_DMXGridRenderTexture_TexelSize;
            uniform const half compatSampleYAxis = 0.019231;
            uniform const half standardSampleYAxis = 0.00762;

            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(uint, _DMXChannel)
                UNITY_DEFINE_INSTANCED_PROP(uint, _NineUniverseMode)
                UNITY_DEFINE_INSTANCED_PROP(uint, _EnableColorTextureSample)
                UNITY_DEFINE_INSTANCED_PROP(uint, _LaserCount)
                UNITY_DEFINE_INSTANCED_PROP(uint, _EnableDMX)
                UNITY_DEFINE_INSTANCED_PROP(half, _Scroll)
                UNITY_DEFINE_INSTANCED_PROP(half, _XRotation)
                UNITY_DEFINE_INSTANCED_PROP(half, _YRotation)
                UNITY_DEFINE_INSTANCED_PROP(half, _LaserThickness)
                UNITY_DEFINE_INSTANCED_PROP(half4, _Emission)
                UNITY_DEFINE_INSTANCED_PROP(half, _ZConeFlatness)
                UNITY_DEFINE_INSTANCED_PROP(half, _VertexConeWidth)
                UNITY_DEFINE_INSTANCED_PROP(half, _VertexConeLength)
                UNITY_DEFINE_INSTANCED_PROP(half, _GlobalIntensity)
                UNITY_DEFINE_INSTANCED_PROP(half, _GlobalIntensityBlend)
                UNITY_DEFINE_INSTANCED_PROP(half, _FinalIntensity)
                UNITY_DEFINE_INSTANCED_PROP(half, _TextureColorSampleX)
                UNITY_DEFINE_INSTANCED_PROP(half, _TextureColorSampleY)
            UNITY_INSTANCING_BUFFER_END(Props)

            #include "../Shared/VRSL-DMXFunctions.cginc"

            //#define IF(a, b, c) lerp(b, c, step((fixed) (a), 0));

            half getPan()
            {
                return UNITY_ACCESS_INSTANCED_PROP(Props,_XRotation);
            }
            half getTilt()
            {
                return UNITY_ACCESS_INSTANCED_PROP(Props,_YRotation);
            }
            uint getLaserCount()
            {
                return UNITY_ACCESS_INSTANCED_PROP(Props,_LaserCount);
            }
            half getConeWidth()
            {
                return UNITY_ACCESS_INSTANCED_PROP(Props,_VertexConeWidth);
            }
            half getConeLength()
            {
                return UNITY_ACCESS_INSTANCED_PROP(Props,_VertexConeLength);
            }
            half getLaserThickness()
            {
                return UNITY_ACCESS_INSTANCED_PROP(Props,_LaserThickness);
            }
            half getScrollSpeed()
            {
                return UNITY_ACCESS_INSTANCED_PROP(Props,_Scroll);
            }
            half getConeFlatness()
            {
                return UNITY_ACCESS_INSTANCED_PROP(Props, _ZConeFlatness);
            }

            half3 RGB2HSV(half3 c)
            {
                half4 K = half4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                half4 p = lerp(half4(c.bg, K.wz), half4(c.gb, K.xy), step(c.b, c.g));
                half4 q = lerp(half4(p.xyw, c.r), half4(c.r, p.yzx), step(p.x, c.r));

                half d = q.x - min(q.w, q.y);
                half e = 1.0e-10;
                return half3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
            }

            half3 hsb2rgb( half3 c ){
                half3 rgb = clamp( abs(fmod(c.x*6.0+half3(0.0,4.0,2.0),6)-3.0)-1.0, 0, 1);
                rgb = rgb*rgb*(3.0-2.0*rgb);
                return c.z * lerp( half3(1,1,1), rgb, c.y);
            }

            half4 CalculateRotations(appdata v, half4 input, half pan, half tilt, half roll)
            {
                half angleY = radians(pan);
                half c = cos(angleY);
                half s = sin(angleY);
                half4x4 rotateYMatrix = half4x4(c, -s, 0, 0,
                    s, c, 0, 0,
                    0, 0, 1, 0,
                    0, 0, 0, 1);
                half4 BaseAndFixturePos = input;
                half4 localRotY = mul(rotateYMatrix, BaseAndFixturePos);


                half angleX = radians(tilt);
                c = cos(angleX);
                s = sin(angleX);
                half4x4 rotateXMatrix = half4x4(1, 0, 0, 0,
                    0, c, -s, 0,
                    0, s, c, 0,
                    0, 0, 0, 1);

                half4x4 rotateXYMatrix = mul(rotateYMatrix, rotateXMatrix);
                //half4 localRotXY = mul(rotateXYMatrix, input);
                
                
                half angleZ = radians(roll);
                c = cos(angleZ);
                s = sin(angleZ);
                half4x4 rotateZMatrix = half4x4(c, 0, s, 0,
                    0, 1, 0, 0,
                    -s, 0, c, 0,
                    0, 0, 0, 1);
                half4x4 rotateXYZMatrix = mul(rotateZMatrix, rotateXYMatrix);
                half4 localRotXYZ = mul(rotateXYZMatrix, input);
                input.xyz = localRotXYZ.xyz;
                return input;
                //LOCALROTXY IS COMBINED ROTATION
            }

            half GetSpin(uint DMXChannel)
            {
                half status = getValueAtCoords(DMXChannel, _Udon_DMXGridRenderTexture);
                half phase = getValueAtCoordsRaw(DMXChannel, _Udon_DMXGridSpinTimer);
                //phase = checkPanInvertY() == 1 ? -phase : phase;
                return status > 0.5 ? -phase : phase;
            }

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o)
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                uint dmx = getDMXChannel();
                o.rgbIntensity.w = getValueAtCoords(dmx + (uint) 10, _Udon_DMXGridRenderTexture);
                o.rgbIntensity.w = IF(isDMX() > 0, o.rgbIntensity.w, 1);
                if(getGlobalIntensity() <= 0.01 || getFinalIntensity() <= 0.05 || _UniversalIntensity <= 0.05 || o.rgbIntensity.w <= 0.05)
                {
                    o.vertex = UnityObjectToClipPos(half4(0,0,0,0));
                    return o;
                }
                o.panTiltLengthWidth.x = lerp(-90,90,clamp(getValueAtCoords(dmx, _Udon_DMXGridRenderTextureMovement), 0.0,0.9999)); // ch 1
                o.panTiltLengthWidth.y = lerp(-90,90,clamp(getValueAtCoords(dmx + (uint) 1, _Udon_DMXGridRenderTextureMovement), 0.0,0.9999)); // ch 2
                o.panTiltLengthWidth.z = lerp(-0.5,5,clamp(getValueAtCoords(dmx + (uint) 2, _Udon_DMXGridRenderTexture), 0.0,0.9999)); // ch 3
                o.panTiltLengthWidth.w = lerp(-3.75,20,clamp(getValueAtCoords(dmx + (uint) 3, _Udon_DMXGridRenderTexture), 0.0,0.9999)); // ch 4


    
                //replacement for _WorldSpaceCameraPos
                float3 wpos;
                wpos.x = unity_CameraToWorld[0][3];
                wpos.y = unity_CameraToWorld[1][3];
                wpos.z = unity_CameraToWorld[2][3];

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv2 = TRANSFORM_TEX(v.uv2, _MainTex);
                 
                
                //Cone Length
                half length = IF(isDMX() > 0, o.panTiltLengthWidth.z, getConeLength());
                v.vertex.y = lerp(v.vertex.y, v.vertex.y *2, length);
                

                //Cone Width
                half width = IF(isDMX() > 0, o.panTiltLengthWidth.w, getConeWidth());
                half4 vert = lerp(v.vertex ,half4(v.vertex.xyz + v.normal * width, 1), v.uv2.y);
                vert.y = v.vertex.y; //Prevent the cone from elongating when changing width.

                // Cone Flatness for X and Z
                half flatness = lerp(0,1.999,getValueAtCoords(dmx+ (uint) 4, _Udon_DMXGridRenderTexture));
                flatness = IF(isDMX() > 0, flatness, getConeFlatness());
                vert.z = lerp(vert.z, vert.z/2, flatness);
              //  vert.x = lerp(vert.x, vert.x/2, _XConeFlatness);
                half xRot = IF(isDMX() > 0, o.panTiltLengthWidth.x, getPan());
                half yRot = IF(isDMX() > 0, o.panTiltLengthWidth.y, getTilt());
                vert = CalculateRotations(v, vert, _ZRotation, xRot, yRot);

                o.viewDir = normalize(wpos - mul(unity_ObjectToWorld, vert).xyz);
                v.normal = CalculateRotations(v, half4(v.normal, 1), _ZRotation, xRot, yRot).xyz;
                o.normal = normalize(mul(half4(v.normal, 0.0), unity_WorldToObject).xyz);
                o.worldPos = mul(unity_ObjectToWorld, vert);
                o.vertex = UnityObjectToClipPos(vert);

                o.flatnessBeamCountSpinThickness.x = lerp(0,1.999,getValueAtCoords(dmx+ (uint) 4, _Udon_DMXGridRenderTexture)); //5
                o.flatnessBeamCountSpinThickness.y = lerp(4,68,getValueAtCoords(dmx+ (uint) 5, _Udon_DMXGridRenderTexture));//6
                o.flatnessBeamCountSpinThickness.z = GetSpin(dmx+ (uint) 6) * _ScrollOffset; //7
                o.flatnessBeamCountSpinThickness.w = lerp(0.0005, 0.05,getValueAtCoords(dmx+ (uint) 11, _Udon_DMXGridRenderTexture)); //12
                o.rgbIntensity.x = getValueAtCoords(dmx+ (uint) 7, _Udon_DMXGridRenderTexture); //8 
                o.rgbIntensity.y = getValueAtCoords(dmx+ (uint) 8, _Udon_DMXGridRenderTexture); //9
                o.rgbIntensity.z = getValueAtCoords(dmx+ (uint) 9, _Udon_DMXGridRenderTexture); //10
                o.rgbIntensity.w = getValueAtCoords(dmx+ (uint) 10, _Udon_DMXGridRenderTexture); //11

                return o;
            }
            

            fixed4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                UNITY_SETUP_INSTANCE_ID(i);
                uint dmx = getDMXChannel();

                i.rgbIntensity.w = IF(isDMX() > 0, i.rgbIntensity.w, 1);
                if(getGlobalIntensity() <= 0.001 || getFinalIntensity() <= 0.001 || _UniversalIntensity <= 0.001 || i.rgbIntensity.w <= 0.001)
                {
                    return half4(0,0,0,0);
                }
                half fade = pow(saturate(dot(normalize(i.normal), i.viewDir)), _FadeStrength);
                
                
               // fade = pow(fade, pow(_FadeStrength, _FadeAmt)) * fade;
                // sample the texture
                half4 dmxcol = half4(i.rgbIntensity.x,i.rgbIntensity.y, i.rgbIntensity.z, _MainColor.a);
                half4 actualcolor = IF(isDMX() > 0, dmxcol *_MainColor * getEmissionColor(), _MainColor * getEmissionColor());
                half4 color = lerp(half4(0,0,0,_MainColor.a), actualcolor, getGlobalIntensity());
                // half3 newColor = RGB2HSV(color.rgb);
                // newColor.y -= 0.1;
                // color.rgb = hsb2rgb(newColor);
                
                color = color * getFinalIntensity();
                color = color * _UniversalIntensity;
                color = color * i.rgbIntensity.w;
                fixed4 col = half4(1,1,1,1);
                fixed4 alphaMask = tex2D(_MainTex, i.uv2);
                half2 laserUV = i.uv2;
                col *= color;

                //Draw Beams
                half scroll = IF(isDMX() > 0, i.flatnessBeamCountSpinThickness.z, getScrollSpeed());
                laserUV.x += _Time.y * scroll;  
                half beamcount = IF(isDMX() > 0, round(i.flatnessBeamCountSpinThickness.y), getLaserCount());
                laserUV.x = frac(laserUV.x * beamcount);
                laserUV.x = laserUV.x - 0.5;
                half thiknes = IF(isDMX() > 0, i.flatnessBeamCountSpinThickness.w, getLaserThickness());
                
                
                // Transparency (with gradation)
                //half dist = normalize(distance(i.worldPos, half4(0,0,0,0))); 
               // fade = lerp(1,fade, pow(dist, _FadeAmt));

                
                half beams = (beamcount * thiknes * 0.1) / abs(laserUV.x);
                half transv = pow((-i.uv2.y + 1), _EndFade) ;
                col = col * beams;

                col += pow(-laserUV.y+1,_InternalShineLength*1.5) * _InternalShine * color;

                // half beamGradCorrect = .2 / (1.-laserUV.y);
                // col *= beamGradCorrect;
                // Clamping here softens the depiction
                col = clamp(0, _LaserSoftening, col);

                // col = lerp(col, col*10, _ZConeFlatness/1.999);
                // half edgeFadeL = distance(i.uv2.x, 0.75);
                // half edgeFadeR = distance(i.uv2.x, 0.25);
                // half4 edgee = lerp(half4(0,0,0,0), col, pow(edgeFadeR * edgeFadeL, _FadeStrength));
                // col = lerp(col, edgee, _ZConeFlatness/1.999);

//                col.xyz += rimlight.xyz;
                //col *= fade;
                col = lerp(half4(0,0,0,0), col, transv);
                half pi = 3.14159;
                half edgeMask = clamp((((sin((12.5 * i.uv2.x) + 4.75)) ))/pi * 6, 0,1);
                edgeMask = 1 - edgeMask;
                if(i.uv2.y > 0.99)
                {
                    discard;
                    return half4(0,0,0,0);
                }
                
                // col = col * col *;
               // half4 flatCol = col * flatEdgeMask.r;
               half flatness = IF(isDMX() > 0, i.flatnessBeamCountSpinThickness.x, getConeFlatness());
              
               half4 flatCol = col * edgeMask;
                col = lerp(flatCol, col, pow(((flatness/2.0) - 1.0)*-1, 0.95));
                col *= getFinalIntensity() * _UniversalIntensity * i.rgbIntensity.w;
                return col;
            }
            ENDCG
        }
    }
}
