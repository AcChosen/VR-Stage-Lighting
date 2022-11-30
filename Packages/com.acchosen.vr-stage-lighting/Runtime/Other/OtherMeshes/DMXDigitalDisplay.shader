Shader "Unlit/DMXDigitalDisplay"
{
    Properties
    {
        _NumberLookUpTexture ("Number Look Up Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _DMXChannel ("Starting DMX Channel", Int) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #pragma multi_compile_instancing
			#pragma instancing_options assumeuniformscaling

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 tex : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO 
            };

            sampler2D _NumberLookUpTexture;
            float4 _NumberLookUpTexture_ST, _Color;
            
            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(uint, _DMXChannel)
            UNITY_INSTANCING_BUFFER_END(Props)

            uint GetUniverse(uint dmx)
            {
                return ceil(dmx/513);
            }

            uint GetCorrectDmx(uint dmx, inout uint universe)
            {
                uint d = dmx - (513 * universe);
                if(d > 512)
                {
                    d -= (512 * (universe));
                    universe += 1;
                }
                return d;
            }

            float2 GetNumUVs(uint number, float2 uv)
            {
                uint y = floor(number / 4);
                uint x = (number) % 4;
                uv.x += (0.25 * x);
                uv.y -= (0.25 * y);
                return uv;
            }

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_OUTPUT(v2f, o); //DON'T INITIALIZE OR IT WILL BREAK PROJECTION
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
      
                o.uv = TRANSFORM_TEX(v.uv, _NumberLookUpTexture);
                uint rawDMX = UNITY_ACCESS_INSTANCED_PROP(Props,_DMXChannel);
                uint universe = GetUniverse(rawDMX);
                //rawDMX = universe > 0 ? rawDMX + 1 : rawDMX;
                rawDMX += universe;
                uint dmx = GetCorrectDmx(rawDMX, universe);
                //dmx = universe > 0 ? dmx + 1 : dmx;

                if(o.uv.x < 0.062)
                {
                    o.uv = GetNumUVs(universe+1, o.uv);
                }
                else if(o.uv.x > 0.088 && o.uv.x < 0.143)
                {
                    o.uv = GetNumUVs(dmx/100 % 10, o.uv);
                }
                else if(o.uv.x > 0.143 && o.uv.x < 0.194)
                {
                    o.uv = GetNumUVs(dmx/10 % 10, o.uv);
                }
                else if(o.uv.x > 0.194)
                {
                    o.uv = GetNumUVs(dmx % 10, o.uv);
                }
                float4 uvcoords = float4(o.uv.x, o.uv.y, 0,0);
                o.tex = tex2Dlod(_NumberLookUpTexture, uvcoords);
                v.vertex = o.tex.r < 0.5 ? float4(0,0,0,0) : v.vertex;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _Color * i.tex;
            }
            ENDCG
        }
    }
}
