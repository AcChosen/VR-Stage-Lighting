﻿Shader "VRSL/DMX CRTs/Strobe Timer"
{
    //THIS IS A TIMER, TO KEEP TRACK OF HOW MUCH TIME HAS PASSED FOR THE STROBE
    Properties
    {
        [NoScaleOffset]_OSCGridRenderTexture("OSC Grid Render Texture (To Control Lights)", 2D) = "white" {}
        _MaxStrobeFreq("Maximum Strobe Frequency", Range(1,100)) = 25
        [Toggle] _NineUniverseMode ("Extended Universe Mode", Int) = 0
     }

     SubShader
     {
        Lighting Off
        Blend One Zero
        Pass
        {
            Name "Strobe Pass"
            CGPROGRAM
            #include "UnityCustomRenderTexture.cginc"
            #include "UnityCG.cginc"
            #pragma vertex CustomRenderTextureVertexShader
            #pragma fragment frag
            #pragma target 4.5

            sampler2D   _Tex;
            Texture2D _OSCGridRenderTexture;
            uniform float4 _OSCGridRenderTexture_TexelSize;
            SamplerState sampler_point_repeat;
            half _MaxStrobeFreq;
            uint _NineUniverseMode;

            

            #define IF(a, b, c) lerp(b, c, step((fixed) (a), 0));



            // float calculateStrobe(float2 coords)
            // {
            //     float freq = GetStrobeValue(coords);
            //     return 2.0;
            // }

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
            //NOTE
            //THIS IS OUTPUTTING THE PHASE OF A SINE WAVE
            //IT READS THE INPUT TEXTURE TO GET THE FREQUENCY AND ADJUSTS THE PHASE TO THAT FREQUENCY FOR THAT CHANNEL
            //IT IS ASSUMING ALL CHANNELS ARE STROBE CHANNELS
            //THE ACTUAL SINE WAVE IS CALCULATED ON THE FIXTURE AS SIN(PHASE)
            //DON'T FORGET NEXT TIME PLZ THX
            //SERIOUSLY, TRIG IS IMPORTANT

            {
                //CHILL FOR 1 SECOND TO ALLOW DATA TO COME IN
                if (_Time.y > 1.0)
                {
                    float4 previousFrame = tex2D(_SelfTexture2D, IN.localTexcoord.xy);
                    float4 currentFrame = _OSCGridRenderTexture.SampleLevel(sampler_point_repeat, IN.localTexcoord.xy, 0);

                    float dt = clamp(unity_DeltaTime.x, 0.0, 2.0);
                    //T = CURRENT PHASE
                    if(_NineUniverseMode)
                    {
                        float3 t = float3(previousFrame.r, previousFrame.g, previousFrame.b);
                        //INCREMENT CURRENT PHASE CLOSER TO 2PI
                        t = t + (float3(dt, dt, dt) * (GetDMXValueRGB(currentFrame) * float3(_MaxStrobeFreq, _MaxStrobeFreq, _MaxStrobeFreq)));

                        //IF PHASE IS GREATER THAN OR EQUAL TO 2PI, RETURN TO 0, CAUSE SIN(2PI) == SIN(0)
                        // if (t >= 2*3.14159265) 
                        // {
                        //     t -= 2*3.14159265; 
                        // }
                        t.r -= t.r >= 2*3.14159265 ? 2*3.14159265 : 0.0;
                        t.g -= t.g >= 2*3.14159265 ? 2*3.14159265 : 0.0;
                        t.b -= t.b >= 2*3.14159265 ? 2*3.14159265 : 0.0;

                        t = float3(clamp(t.r, 0.0, 1000000.0), clamp(t.g, 0.0, 1000000.0), clamp(t.b, 0.0, 1000000.0));
                        //EZ CLAP
                        return float4(t, currentFrame.a);
                    }
                    else
                    {
                        float t = previousFrame.r;
                        //INCREMENT CURRENT PHASE CLOSER TO 2PI
                        t = t + (dt * (GetDMXValue(currentFrame) * _MaxStrobeFreq));

                        //IF PHASE IS GREATER THAN OR EQUAL TO 2PI, RETURN TO 0, CAUSE SIN(2PI) == SIN(0)
                        // if (t >= 2*3.14159265) 
                        // {
                        //     t -= 2*3.14159265; 
                        // }
                        t -= t >= 2*3.14159265 ? 2*3.14159265 : 0.0;
                        //EZ CLAP
                        return clamp(t, 0.0, 1000000.0);
                    }
                }

                else
                {
                    return _OSCGridRenderTexture.SampleLevel(sampler_point_repeat, IN.localTexcoord.xy, 0);
                }
            }
            ENDCG
         }
    }
    CustomEditor "VRSLInspector"
}
