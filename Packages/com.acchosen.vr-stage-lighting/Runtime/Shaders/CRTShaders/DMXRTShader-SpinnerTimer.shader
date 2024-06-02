Shader "VRSL/DMX CRTs/Spinner Timer"
{
    //THIS IS A TIMER, TO KEEP TRACK OF HOW MUCH TIME HAS PASSED FOR THE NEXT ROTATION
    Properties
    {
        [NoScaleOffset]_DMXTexture("DMX Grid Texture", 2D) = "white" {}
        [Toggle] _NineUniverseMode ("Extended Universe Mode", Int) = 0
       // _MaxSpinSpeed("Maximum Spin Speed", Range(0.0,1.0)) = 0.0
     }

     SubShader
     {
        Lighting Off
        Blend One Zero
        Name "Gobo Spin Pass"
        Pass
        {
            CGPROGRAM
            #include "UnityCustomRenderTexture.cginc"
            #include "UnityCG.cginc"
            #pragma warning (error : 3206) // implicit truncation of vector type
            #pragma vertex CustomRenderTextureVertexShader
            #pragma fragment frag
            #pragma target 4.0

            #define VRSL_DMX

            sampler2D   _Tex;
            Texture2D _DMXTexture;
         //   half _MaxSpinSpeed;
            uniform half4 _DMXTexture_TexelSize;
            SamplerState sampler_point_repeat;
            uint _NineUniverseMode;
           // half _MaxStrobeFreq;
            #define PI 3.14159265
            #define IF(a, b, c) lerp(b, c, step((fixed) (a), 0));

            half GetDMXValue(half4 c)
            {
                half3 cRGB = half3(c.r, c.g, c.b);
                half value = LinearRgbToLuminance(cRGB);
                
                return value;
            }

            float4 frag(v2f_customrendertexture IN) : COLOR

            {
                //CHILL FOR 1 SECOND TO ALLOW DATA TO COME IN
                if (_Time.y > 1.0)
                {
                    float4 previousFrame = tex2D(_SelfTexture2D, IN.localTexcoord.xy);
                    float4 currentFrame = _DMXTexture.SampleLevel(sampler_point_repeat, IN.localTexcoord.xy, 0);

                    float dt = unity_DeltaTime.x;
                    if(_NineUniverseMode)
                    {
                        float3 dmx = currentFrame.rgb;
                        float3 t = previousFrame.rgb;
                        float3 spin = float3(dmx.r > 0.5 ? (dmx.r - 0.5) : dmx.r, dmx.g > 0.5 ? (dmx.g - 0.5) : dmx.g, dmx.b > 0.5 ? (dmx.b - 0.5) : dmx.b);

                        t+= dt * spin;
                        return float4(t, 1);
                        // return half4(clamp(t.rgb, half3(0.0, 0.0, 0.0), half3(1000000.0,1000000.0,1000000.0)), currentFrame.a);
                            
                    }
                    else
                    {
                        float dmx = GetDMXValue(currentFrame);
                        if(dmx < 0.01)
                        {
                            return 0;
                        }
                        //T = CURRENT PHASE
                        float t = previousFrame.r;
                        //INCREMENT CURRENT PHASE CLOSER TO 2PI
                        // float spin;

                        float spin = dmx > 0.5 ? (dmx - 0.5) : dmx;
                        t+= dt * spin;
                        return t;
                        //return clamp(t, 0.0, 1000000.0);
                    }
                }

                else
                {
                    return _DMXTexture.SampleLevel(sampler_point_repeat, IN.localTexcoord.xy, 0);
                }
            }
            ENDCG
         }
    }
    //CustomEditor "VRSLInspector"
}
