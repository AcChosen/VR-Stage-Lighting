Shader "VRSL/DMX CRTs/Spinner Timer"
{
    //THIS IS A TIMER, TO KEEP TRACK OF HOW MUCH TIME HAS PASSED FOR THE NEXT ROTATION
    Properties
    {
        [NoScaleOffset]_OSCGridRenderTexture("DMX Grid Texture", 2D) = "white" {}
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
            #pragma vertex CustomRenderTextureVertexShader
            #pragma fragment frag
            #pragma target 4.0

            sampler2D   _Tex;
            Texture2D _OSCGridRenderTexture;
            uniform float4 _OSCGridRenderTexture_TexelSize;
            SamplerState sampler_point_repeat;
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

            float4 frag(v2f_customrendertexture IN) : COLOR

            {
                //CHILL FOR 1 SECOND TO ALLOW DATA TO COME IN
                if (_Time.y > 1.0)
                {
                    float4 previousFrame = tex2D(_SelfTexture2D, IN.localTexcoord.xy);
                    float4 currentFrame = _OSCGridRenderTexture.SampleLevel(sampler_point_repeat, IN.localTexcoord.xy, 0);

                    float dt = clamp(unity_DeltaTime.x, 0.0, 2.0);
                    //T = CURRENT PHASE
                    float t = previousFrame.r;
                    //INCREMENT CURRENT PHASE CLOSER TO 2PI
                    float dmx = GetDMXValue(currentFrame);
                    if(dmx < 0.015)
                    {
                        return 0;
                    }
                    float spin;

                    if(dmx > 0.5)
                    {
                      spin = -(dmx - 0.5);
                      t = t + (dt * spin * 10.0);
                    }
                    else
                    {
                        spin = dmx;
                        t = t - (dt * spin * 10.0);
                    }

                    //IF PHASE IS GREATER THAN OR EQUAL TO 2PI, RETURN TO 0, CAUSE SIN(2PI) == SIN(0)

                    if (t >= 2*PI && dmx > 0.5) 
                    {
                        t -= 2*PI; 
                    }
                    else if(t <= 0.0)
                    {
                        t += 2*PI; 
                    }
                    //EZ CLAP
                    return clamp(t, 0.0, 1000000.0);
                }

                else
                {
                    return _OSCGridRenderTexture.SampleLevel(sampler_point_repeat, IN.localTexcoord.xy, 0);
                }
            }
            ENDCG
         }
    }
    //CustomEditor "VRSLInspector"
}
