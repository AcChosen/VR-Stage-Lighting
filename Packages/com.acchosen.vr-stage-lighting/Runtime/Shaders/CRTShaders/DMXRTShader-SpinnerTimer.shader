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
            uniform float4 _DMXTexture_TexelSize;
            SamplerState sampler_point_repeat;
            uint _NineUniverseMode;
           // half _MaxStrobeFreq;
            #define PI 3.14159265
            #define IF(a, b, c) lerp(b, c, step((fixed) (a), 0));

            float GetDMXValue(float4 c)
            {
                float3 cRGB = float3(c.r, c.g, c.b);
                float value = LinearRgbToLuminance(cRGB);
                value = LinearToGammaSpaceExact(value);
                
                return value;
            }
            float3 GetDMXValueRGB(float4 c)
            {     
                return float3(LinearToGammaSpaceExact(c.r), LinearToGammaSpaceExact(c.g), LinearToGammaSpaceExact(c.b));
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
                        float3 dmx = GetDMXValueRGB(currentFrame);
                        float3 t = previousFrame.rgb;
                        float3 spin;
                        spin *= 2.0;
                        spin.rgb = dmx.rgb > float3(0.5, 0.5, 0.5) ? -(dmx.rgb - float3(0.5, 0.5, 0.5)) : dmx.rgb;
                        t.rgb = dmx.rgb > float3(0.5, 0.5, 0.5) ? t.rgb + (dt * spin * 10.0) : t.rgb - (dt * spin * 10.0);

                        t.rgb-= t.rgb >= float3(2*PI,2*PI,2*PI) && dmx.rgb > float3(0.5, 0.5, 0.5) ? float3(2*PI,2*PI,2*PI) : float3(0.0, 0.0, 0.0);
                        t.rgb+= t.rgb <= float3(0.0, 0.0, 0.0) ? float3(2*PI,2*PI,2*PI) : float3(0.0, 0.0, 0.0);


                        // dmx.r = dmx.r < 0.015 ? 0.0 : dmx.r;
                        // dmx.g = dmx.g < 0.015 ? 0.0 : dmx.g;
                        // dmx.b = dmx.b < 0.015 ? 0.0 : dmx.b;
                        dmx.rgb = dmx.rgb < float3(0.015, 0.015, 0.015) ? float3(0,0,0) : dmx.rgb;
                        return float4(clamp(t.rgb, float3(0.0, 0.0, 0.0), float3(1000000.0,1000000.0,1000000.0)), currentFrame.a);

                    }
                    else
                    {
                        float dmx = GetDMXValue(currentFrame);
                        if(dmx < 0.015)
                        {
                            return 0;
                        }
                        //T = CURRENT PHASE
                        float t = previousFrame.r;
                        //INCREMENT CURRENT PHASE CLOSER TO 2PI
                        float spin;

                        spin = dmx > 0.5 ? -(dmx - 0.5) : dmx;
                        spin *= 2.0;
                        t = dmx > 0.5 ? t + (dt * spin * 10.0) : t - (dt * spin * 10.0);

                        t-= t >= 2*PI && dmx > 0.5 ? 2*PI : 0.0;
                        t+= t <= 0.0 ? 2*PI : 0.0;
                        return clamp(t, 0.0, 1000000.0);
                    }


                    // if(dmx > 0.5)
                    // {
                    //   spin = -(dmx - 0.5);
                    //   t = t + (dt * spin * 10.0);
                    // }
                    // else
                    // {
                    //     spin = dmx;
                    //     t = t - (dt * spin * 10.0);
                    // }

                    //IF PHASE IS GREATER THAN OR EQUAL TO 2PI, RETURN TO 0, CAUSE SIN(2PI) == SIN(0)

                    // if (t >= 2*PI && dmx > 0.5) 
                    // {
                    //     t -= 2*PI; 
                    // }

                    // else if(t <= 0.0)
                    // {
                    //     t += 2*PI; 
                    // }
                    //EZ CLAP
                   // return clamp(t, 0.0, 1000000.0);
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
