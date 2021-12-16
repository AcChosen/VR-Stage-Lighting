Shader "Unlit/VRSL-LaserRAW"
{
    Properties
    {
        _GlobalIntensity ("Global Intensity", Range(0,1)) = 1
        _MainTex ("Texture", 2D) = "white" {}
        _MainColor ("Main Color" , Color) = (1.0, 1.0, 1.0, 1.0)
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
        _LaserSoftening ("Laser Softness", Range(0,10)) = 0
        _SourceIntensity ("Source Intensity", Range(0,0.5)) = 1
        _FlatEdgeFadeStrength ("Flat Edge Fade Strength", Range (0, 5)) = 1
        _Scroll ("Scroll", Range(-1, 1)) = 1
        // _Modx ("Mod x", Range(0,50)) = 0
        // _Mody ("Mod y", Range(0,10)) = 0
        // _Modz ("Mod z", Range(1,10)) = 1
        // [Toggle]_Switch ("Switch", Int) = 0
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
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
                float3 normal : TEXCOORD2;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float4 worldPos : TEXCOORD3;
                float3 viewDir : TEXCOORD4;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST, _MainColor;
            half _VertexConeWidth, _VertexConeLength, _ZConeFlatness, _XConeFlatness, _XRotation, _YRotation, _ZRotation, _LaserThickness;
            half _EndFade, _FadeStrength, _SourceIntensity, _LaserSoftening, _GlobalIntensity, _FlatEdgeFadeStrength, _Scroll;
            uint _LaserCount;

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
                if(_GlobalIntensity <= 0.0)
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
                v.vertex.y = lerp(v.vertex.y, v.vertex.y *2, _VertexConeLength);
                

                //Cone Width
                float4 vert = lerp(v.vertex ,float4(v.vertex.xyz + v.normal * _VertexConeWidth, 1), v.uv2.y);
                vert.y = v.vertex.y; //Prevent the cone from elongating when changing width.

                // Cone Flatness for X and Z
                vert.z = lerp(vert.z, vert.z/2, _ZConeFlatness);
              //  vert.x = lerp(vert.x, vert.x/2, _XConeFlatness);
                vert = CalculateRotations(v, vert, _ZRotation, _XRotation, _YRotation);

                o.viewDir = normalize(wpos - mul(unity_ObjectToWorld, vert).xyz);
                v.normal = CalculateRotations(v, float4(v.normal, 1), _ZRotation, _XRotation, _YRotation).xyz;
                o.normal = normalize(mul(float4(v.normal, 0.0), unity_WorldToObject).xyz);
                o.worldPos = mul(unity_ObjectToWorld, vert);
                o.vertex = UnityObjectToClipPos(vert);
                return o;
            }
            

            fixed4 frag (v2f i) : SV_Target
            {
                if(_GlobalIntensity <= 0.0)
                {
                    return half4(0,0,0,0);
                }
                float fade = pow(saturate(dot(normalize(i.normal), i.viewDir)), _FadeStrength);
                
               // fade = pow(fade, pow(_FadeStrength, _FadeAmt)) * fade;
                // sample the texture
                float4 color = lerp(float4(0,0,0,_MainColor.a), _MainColor, _GlobalIntensity);
                float3 newColor = RGB2HSV(color.rgb);
                newColor.y -= 0.1;
                color.rgb = hsb2rgb(newColor);
                fixed4 col = float4(1,1,1,1);
                fixed4 alphaMask = tex2D(_MainTex, i.uv2);
                float2 laserUV = i.uv2;
                col *= color;

                //Draw Beams
                laserUV.x = laserUV.x += _Time.y * _Scroll;  
                laserUV.x = frac(laserUV.x * _LaserCount);
                laserUV.x = laserUV.x - 0.5;
                
                
                // Transparency (with gradation)
                //float dist = normalize(distance(i.worldPos, float4(0,0,0,0))); 
               // fade = lerp(1,fade, pow(dist, _FadeAmt));

                
                float beams = (_LaserCount * _LaserThickness * 0.1) / abs(laserUV.x);
                float transv = pow((-i.uv2.y + 1), _EndFade) ;
                col = col * beams;

                col += pow(-laserUV.y+1,12.1) * 3 * _SourceIntensity * color;

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
                const float PI = 3.14159265;
                float edgeMask = clamp((((sin((12.5 * i.uv2.x) + 4.75)) ))/PI * 6, 0,1);
                edgeMask = 1 - edgeMask;
                if(i.uv2.y > 0.99)
                {
                    discard;
                    return float4(0,0,0,0);
                }
                
                // col = col * col *;
               // float4 flatCol = col * flatEdgeMask.r;
              
               float4 flatCol = col * edgeMask;
                col = lerp(flatCol, col, pow(((_ZConeFlatness/2.0) - 1.0)*-1, 0.95));
                return col;
            }
            ENDCG
        }
    }
}
