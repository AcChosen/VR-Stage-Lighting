Shader "VRSL/Basic Laser/DMX"
{
    Properties
    {
        _DMXChannel ("DMX Channel Number)", Int) = 0
        _UniversalIntensity ("Universal Intensity", Range (0,1)) = 1
        _FinalIntensity("Final Intensity", Range(0,1)) = 1
        _GlobalIntensity ("Global Intensity", Range(0,1)) = 1
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
        _Scroll ("Scroll", Range(-1, 1)) = 1
        [Toggle] _EnableOSC ("Enable Stream OSC/DMX Control", Int) = 0
        
        [NoScaleOffset] _OSCGridRenderTextureRAW("OSC Grid Render Texture (RAW Unsmoothed)", 2D) = "white" {}
		[NoScaleOffset] _OSCGridRenderTexture("OSC Grid Render Texture (To Control Lights)", 2D) = "white" {}
        [Toggle] _EnableCompatibilityMode ("Enable Compatibility Mode", Int) = 0
        [Toggle] _EnableVerticalMode ("Enable Vertical Mode", Int) = 0

    }
    SubShader
    {
		Tags { "RenderType"="Transparent" "Queue" = "Transparent" }
		Cull Off
		Blend One One
		Zwrite Off
		LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            //#pragma multi_compile_fog
            #pragma multi_compile_instancing

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
               centroid float2 uv : TEXCOORD0;
               centroid float2 uv2 : TEXCOORD1;
                float3 normal : TEXCOORD2;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float4 worldPos : TEXCOORD3;
                float3 viewDir : TEXCOORD4;
                float4 panTiltLengthWidth : TEXCOORD5; //ch 1,2,3,4
                float4 flatnessBeamCountSpinThickness : TEXCOORD6; //ch 5,6,7,12
                float4 rgbIntensity : TEXCOORD7;// ch 8,9,10,11
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            float4 _MainTex_ST, _MainColor;
            half _XConeFlatness, _ZRotation, _UniversalIntensity;
            half _EndFade, _FadeStrength, _InternalShine, _LaserSoftening;
            uint _EnableCompatibilityMode, _EnableVerticalMode;
            sampler2D _OSCGridRenderTexture, _OSCGridRenderTextureRAW;
            uniform float4 _OSCGridRenderTextureRAW_TexelSize;
            uniform const half compatSampleYAxis = 0.019231;
            uniform const half standardSampleYAxis = 0.00762;

            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(uint, _DMXChannel)
                UNITY_DEFINE_INSTANCED_PROP(uint, _EnableColorTextureSample)
                UNITY_DEFINE_INSTANCED_PROP(uint, _LaserCount)
                UNITY_DEFINE_INSTANCED_PROP(uint, _EnableOSC)
                UNITY_DEFINE_INSTANCED_PROP(float, _Scroll)
                UNITY_DEFINE_INSTANCED_PROP(float, _XRotation)
                UNITY_DEFINE_INSTANCED_PROP(float, _YRotation)
                UNITY_DEFINE_INSTANCED_PROP(float, _LaserThickness)
                UNITY_DEFINE_INSTANCED_PROP(float4, _Emission)
                UNITY_DEFINE_INSTANCED_PROP(float, _ZConeFlatness)
                UNITY_DEFINE_INSTANCED_PROP(float, _VertexConeWidth)
                UNITY_DEFINE_INSTANCED_PROP(float, _VertexConeLength)
                UNITY_DEFINE_INSTANCED_PROP(float, _GlobalIntensity)
                UNITY_DEFINE_INSTANCED_PROP(float, _FinalIntensity)
                UNITY_DEFINE_INSTANCED_PROP(float, _TextureColorSampleX)
                UNITY_DEFINE_INSTANCED_PROP(float, _TextureColorSampleY)
            UNITY_INSTANCING_BUFFER_END(Props)

            #define IF(a, b, c) lerp(b, c, step((fixed) (a), 0));

            float getPan()
            {
                return UNITY_ACCESS_INSTANCED_PROP(Props,_XRotation);
            }
            float getTilt()
            {
                return UNITY_ACCESS_INSTANCED_PROP(Props,_YRotation);
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
            float4 getEmissionColor()
            {
                return UNITY_ACCESS_INSTANCED_PROP(Props,_Emission);
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
                return UNITY_ACCESS_INSTANCED_PROP(Props, _GlobalIntensity);
            }

            float getFinalIntensity()
            {
                return UNITY_ACCESS_INSTANCED_PROP(Props, _FinalIntensity);
            }
            uint getDMXChannel()
            {
                return (uint) round(UNITY_ACCESS_INSTANCED_PROP(Props, _DMXChannel));  
            }
            float getConeFlatness()
            {
                return UNITY_ACCESS_INSTANCED_PROP(Props, _ZConeFlatness);
            }
            uint isOSC()
            {
                return UNITY_ACCESS_INSTANCED_PROP(Props,_EnableOSC);
            }
            float2 LegacyRead(int channel, int sector)
            {
                // say we were on sector 6
                // we need to move over 2 sectors
                // and we need to move up 3 sectors

                //1 sector is every 13 channels
                    float x = 0.02000;
                    float y = 0.02000;
                    //TRAVERSING THE Y AXIS OF THE OSC GRID
                    float ymod = floor(sector / 2.0);       

                    //TRAVERSING THE X AXIS OF THE OSC GRID
                    float xmod = sector % 2.0;

                    x+= (xmod * 0.50);
                    y+= (ymod * 0.04);

                    x+= (channel * 0.04);
                    //we are now on the correct
                    return float2(x,y);

            }

            float2 IndustryRead(int x, int y, uint DMXChannel)
            {
                
                float resMultiplierX = (_OSCGridRenderTextureRAW_TexelSize.z / 13);
                float2 xyUV = float2(0.0,0.0);
                
                xyUV.x = ((x * resMultiplierX) * _OSCGridRenderTextureRAW_TexelSize.x);
                xyUV.y = (y * resMultiplierX) * _OSCGridRenderTextureRAW_TexelSize.y;
                xyUV.y -= 0.001;
                xyUV.x -= 0.015;
            // xyUV.x = DMXChannel == 15 ? xyUV.x + 0.0769 : xyUV.x;
                return xyUV;
            }

            //function for getting the value on the OSC Grid in the bottom right corner configuration
            float getValueAtCoords(uint DMXChannel, sampler2D _Tex)
            {
                //DMXChannel = DMXChannel == 15.0 ? DMXChannel + 1 : DMXChannel;
                uint x = DMXChannel % 13; // starts at 1 ends at 13
                x = x == 0.0 ? 13.0 : x;
                float y = DMXChannel / 13.0; // starts at 1 // doubles as sector
                y = frac(y)== 0.00000 ? y - 1 : y;

                float2 xyUV = _EnableCompatibilityMode == 1 ? LegacyRead(x-1.0,y) : IndustryRead(x,(y + 1.0), DMXChannel);
                    
                float4 uvcoords = float4(xyUV.x, xyUV.y, 0,0);
                float4 c = tex2Dlod(_Tex, uvcoords);
                float3 cRGB = float3(c.r, c.g, c.b);
                float value = LinearRgbToLuminance(cRGB);
                value = LinearToGammaSpaceExact(value);
                return value;
            }

            float getValueAtCoordsRaw(uint DMXChannel, sampler2D _Tex)
            {
            // DMXChannel = DMXChannel == 15.0 ? DMXChannel + 1 : DMXChannel;
                uint x = DMXChannel % 13; // starts at 1 ends at 13
                x = x == 0.0 ? 13.0 : x;
                float y = DMXChannel / 13.0; // starts at 1 // doubles as sector
                y = frac(y)== 0.00000 ? y - 1 : y;

                float2 xyUV = _EnableCompatibilityMode == 1 ? LegacyRead(x-1.0,y) : IndustryRead(x,(y + 1.0), DMXChannel);

                float4 uvcoords = float4(xyUV.x, xyUV.y, 0,0);
                float4 c = tex2Dlod(_Tex, uvcoords);
                
                return c.r;
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
                UNITY_INITIALIZE_OUTPUT(v2f, o)
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                UNITY_TRANSFER_INSTANCE_ID(v, o);

                uint dmx = getDMXChannel();
                o.rgbIntensity.w = getValueAtCoords(dmx + 10, _OSCGridRenderTexture);
                o.rgbIntensity.w = IF(isOSC() > 0, o.rgbIntensity.w, 1);
                if(getGlobalIntensity() <= 0.01 || getFinalIntensity() <= 0.05 || _UniversalIntensity <= 0.05 || o.rgbIntensity.w <= 0.05)
                {
                    o.vertex = UnityObjectToClipPos(float4(0,0,0,0));
                    return o;
                }
                o.panTiltLengthWidth.x = lerp(-90,90,clamp(getValueAtCoords(dmx, _OSCGridRenderTexture), 0.0,0.9999)); // ch 1
                o.panTiltLengthWidth.y = lerp(-90,90,clamp(getValueAtCoords(dmx + 1, _OSCGridRenderTexture), 0.0,0.9999)); // ch 2
                o.panTiltLengthWidth.z = lerp(-0.5,5,clamp(getValueAtCoords(dmx + 2, _OSCGridRenderTextureRAW), 0.0,0.9999)); // ch 3
                o.panTiltLengthWidth.w = lerp(-3.75,20,clamp(getValueAtCoords(dmx +3, _OSCGridRenderTextureRAW), 0.0,0.9999)); // ch 4


    
                //replacement for _WorldSpaceCameraPos
                float3 wpos;
                wpos.x = unity_CameraToWorld[0][3];
                wpos.y = unity_CameraToWorld[1][3];
                wpos.z = unity_CameraToWorld[2][3];

                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv2 = TRANSFORM_TEX(v.uv2, _MainTex);
                 
                
                //Cone Length
                float length = IF(isOSC() > 0, o.panTiltLengthWidth.z, getConeLength());
                v.vertex.y = lerp(v.vertex.y, v.vertex.y *2, length);
                

                //Cone Width
                float width = IF(isOSC() > 0, o.panTiltLengthWidth.w, getConeWidth());
                float4 vert = lerp(v.vertex ,float4(v.vertex.xyz + v.normal * width, 1), v.uv2.y);
                vert.y = v.vertex.y; //Prevent the cone from elongating when changing width.

                // Cone Flatness for X and Z
                float flatness = lerp(0,1.999,getValueAtCoords(dmx+4, _OSCGridRenderTextureRAW));
                flatness = IF(isOSC() > 0, flatness, getConeFlatness());
                vert.z = lerp(vert.z, vert.z/2, flatness);
              //  vert.x = lerp(vert.x, vert.x/2, _XConeFlatness);
                float xRot = IF(isOSC() > 0, o.panTiltLengthWidth.x, getPan());
                float yRot = IF(isOSC() > 0, o.panTiltLengthWidth.y, getTilt());
                vert = CalculateRotations(v, vert, _ZRotation, xRot, yRot);

                o.viewDir = normalize(wpos - mul(unity_ObjectToWorld, vert).xyz);
                v.normal = CalculateRotations(v, float4(v.normal, 1), _ZRotation, xRot, yRot).xyz;
                o.normal = normalize(mul(float4(v.normal, 0.0), unity_WorldToObject).xyz);
                o.worldPos = mul(unity_ObjectToWorld, vert);
                o.vertex = UnityObjectToClipPos(vert);
                return o;
            }
            

            fixed4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                UNITY_SETUP_INSTANCE_ID(i);
                uint dmx = getDMXChannel();
                i.flatnessBeamCountSpinThickness.x = lerp(0,1.999,getValueAtCoords(dmx+4, _OSCGridRenderTextureRAW)); //5
                i.flatnessBeamCountSpinThickness.y = lerp(4,68,getValueAtCoords(dmx+5, _OSCGridRenderTextureRAW));//6
                i.flatnessBeamCountSpinThickness.z = lerp(0,1,getValueAtCoords(dmx+6, _OSCGridRenderTextureRAW)); //7
                i.flatnessBeamCountSpinThickness.w = lerp(0.003, 0.1,getValueAtCoords(dmx+11, _OSCGridRenderTextureRAW)); //12
                i.rgbIntensity.x = getValueAtCoords(dmx+7, _OSCGridRenderTextureRAW); //8 
                i.rgbIntensity.y = getValueAtCoords(dmx+8, _OSCGridRenderTextureRAW); //9
                i.rgbIntensity.z = getValueAtCoords(dmx+9, _OSCGridRenderTextureRAW); //10
                i.rgbIntensity.w = getValueAtCoords(dmx+10, _OSCGridRenderTextureRAW); //11
                i.rgbIntensity.w = IF(isOSC() > 0, i.rgbIntensity.w, 1);
                if(getGlobalIntensity() <= 0.001 || getFinalIntensity() <= 0.001 || _UniversalIntensity <= 0.001 || i.rgbIntensity.w <= 0.001)
                {
                    return half4(0,0,0,0);
                }
                float fade = pow(saturate(dot(normalize(i.normal), i.viewDir)), _FadeStrength);
                
                
               // fade = pow(fade, pow(_FadeStrength, _FadeAmt)) * fade;
                // sample the texture
                float4 dmxcol = float4(i.rgbIntensity.x,i.rgbIntensity.y, i.rgbIntensity.z, _MainColor.a);
                float4 actualcolor = IF(isOSC() > 0, dmxcol *_MainColor * getEmissionColor(), _MainColor * getEmissionColor());
                float4 color = lerp(float4(0,0,0,_MainColor.a), actualcolor, getGlobalIntensity());
                // float3 newColor = RGB2HSV(color.rgb);
                // newColor.y -= 0.1;
                // color.rgb = hsb2rgb(newColor);
                
                color = color * getFinalIntensity();
                color = color * _UniversalIntensity;
                color = color * i.rgbIntensity.w;
                fixed4 col = float4(1,1,1,1);
                fixed4 alphaMask = tex2D(_MainTex, i.uv2);
                float2 laserUV = i.uv2;
                col *= color;

                //Draw Beams
                float scroll = IF(isOSC() > 0, i.flatnessBeamCountSpinThickness.z, getScrollSpeed());
                laserUV.x = laserUV.x += _Time.y * scroll;  
                float beamcount = IF(isOSC() > 0, round(i.flatnessBeamCountSpinThickness.y), getLaserCount());
                laserUV.x = frac(laserUV.x * beamcount);
                laserUV.x = laserUV.x - 0.5;
                float thiknes = IF(isOSC() > 0, i.flatnessBeamCountSpinThickness.w, getLaserThickness());
                
                
                // Transparency (with gradation)
                //float dist = normalize(distance(i.worldPos, float4(0,0,0,0))); 
               // fade = lerp(1,fade, pow(dist, _FadeAmt));

                
                float beams = (beamcount * thiknes * 0.1) / abs(laserUV.x);
                float transv = pow((-i.uv2.y + 1), _EndFade) ;
                col = col * beams;

                col += pow(-laserUV.y+1,12.1) * 3 * _InternalShine * color;

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
               float flatness = IF(isOSC() > 0, i.flatnessBeamCountSpinThickness.x, getConeFlatness());
              
               float4 flatCol = col * edgeMask;
                col = lerp(flatCol, col, pow(((flatness/2.0) - 1.0)*-1, 0.95));
                col *= getFinalIntensity() * _UniversalIntensity * i.rgbIntensity.w;
                return col;
            }
            ENDCG
        }
    }
}
